using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace StyleStar
{
    public class NoteCollection
    {
        public SongMetadata Metadata;

        public List<Note> Steps { get; private set; } = new List<Note>();
        public List<Hold> Holds { get; private set; } = new List<Hold>();
        public List<Note> Motions { get; private set; } = new List<Note>();
        public List<BeatMarker> Markers { get; private set; } = new List<BeatMarker>();

        public double CurrentScore { get; private set; }
        public int TotalNotes { get; private set; }

        public int CurrentCombo { get; private set; }
        public int MaxCombo { get; private set; }

        public int PerfectCount { get; private set; } = 0;
        public int GreatCount { get; private set; } = 0;
        public int GoodCount { get; private set; } = 0;
        public int MissCount { get; private set; } = 0;

        public SongEndReason SongEnd { get; set; }

        public NoteCollection(SongMetadata meta)
        {
            Metadata = meta;
        }

        public SongMetadata ParseFile()
        {
            string fileName = Metadata.ChartFullPath;

            // Used for v0.1
            Dictionary<int, SlideCollection> tempSlideDict = new Dictionary<int, SlideCollection>();
            // Used for v1.0
            Dictionary<int, Hold> tempHoldDict = new Dictionary<int, Hold>();

            // Temp parsing variables
            int beatsPerMeasure = 4;    // Ultimately this should be tied to time signature
            int measure = -1;
            double subDiv;
            int noteClass, lane, width, id, endLane, endWidth;
            bool inNoteData = false;

            try
            {
                using (StreamReader sr = new StreamReader(new FileStream(fileName, FileMode.Open, FileAccess.Read)))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();

                        // Whatever metadata stuff isn't covered in SongMetadata

                        if (Metadata.Version.Major == 1 && Metadata.Version.Minor == 0)
                        {
                            // Look for BPM change tags
                            if (Regex.Match(line, "^(#BPMC)").Success)
                            {
                                var subLine = line.Replace("#BPMC", "");
                                var bpmSplit = Regex.Split(subLine, @"[\s:.]+").Where(s => !String.IsNullOrEmpty(s));
                                if (bpmSplit.Count() == 3)
                                {
                                    id = bpmSplit.ElementAt(0).ParseBase36();
                                    measure = int.Parse(bpmSplit.ElementAt(1));
                                    subDiv = double.Parse(bpmSplit.ElementAt(2)) / 192.0;

                                    if (Metadata.BpmIndex.ContainsKey(id))
                                        Metadata.BpmEvents.Add(new BpmChangeEvent(Metadata.BpmIndex[id], beatsPerMeasure * (measure + subDiv)));
                                    else
                                        Console.WriteLine("Failed to parse BPM Change: {0}", line);
                                }
                                else
                                    Console.WriteLine("Failed to parse BPM Change: {0}", line);
                            }
                            else
                            {
                                // Must have seen the "#NOTES" tag before parsing notes
                                if (Regex.Match(line, "^(#NOTES)").Success)
                                    inNoteData = true;
                                else if (Regex.Match(line, "^(#ENDNOTES)").Success)
                                    inNoteData = false;

                                if(inNoteData)
                                {
                                    if(Regex.Match(line, "^[0-9]{3}$").Success)
                                    {
                                        measure = int.Parse(line);
                                    }
                                    else if (Regex.Match(line, "^[0-9]{3}:").Success)
                                    {
                                        var timeSplit = Regex.Split(line, ":").Where(s => !String.IsNullOrEmpty(s));
                                        if (timeSplit.Count() == 2) // It should only be 2
                                        {
                                            subDiv = double.Parse(timeSplit.ElementAt(0)) / 192.0;
                                            var noteSplit = Regex.Split(timeSplit.ElementAt(1), ",").Where(s => !String.IsNullOrEmpty(s));
                                            foreach (var note in noteSplit)
                                            {
                                                if (Regex.Match(note, "^[2-3]{1}").Success) // Motions
                                                {
                                                    noteClass = note[0].ToString().ParseBase36();
                                                    Motions.Add(new Note(beatsPerMeasure * (measure + subDiv), 0, 16, noteClass == 2 ? Motion.Up : Motion.Down));
                                                }
                                                else if (Regex.Match(note, "^[0-1]{1}[0-9a-fA-F]{2}$").Success) // Any step
                                                {
                                                    noteClass = note[0].ToString().ParseBase36();
                                                    lane = note[1].ToString().ParseBase36();
                                                    width = note[2].ToString().ParseBase36() + 1;

                                                    Steps.Add(new Note(beatsPerMeasure * (measure + subDiv), lane, width, noteClass == 0 ? Side.Left : Side.Right));
                                                }
                                                else if (Regex.Match(note, "^[4-9a-bA-B]{1}[0-9a-fA-F]{2}[0-9a-zA-Z]{1}").Success) // Hold/Slides
                                                {
                                                    noteClass = note[0].ToString().ParseBase36();
                                                    id = note[1].ToString().ParseBase36();
                                                    lane = note[2].ToString().ParseBase36();
                                                    width = note[3].ToString().ParseBase36() + 1;

                                                    switch (noteClass)
                                                    {
                                                        case 4: // Left hold start
                                                        case 5: // Right hold start
                                                                // Check to see if a hold is already active-- if so, commit it and start a new one
                                                            if (tempHoldDict.ContainsKey(id))
                                                            {
                                                                CommitHold(tempHoldDict[id]);
                                                                tempHoldDict.Remove(id);
                                                            }
                                                            // Create the new note
                                                            tempHoldDict.Add(id, new Hold(beatsPerMeasure * (measure + subDiv), lane, width, noteClass == 4 ? Side.Left : Side.Right));
                                                            break;
                                                        case 6: // Slide waypoint
                                                            // Check to see this slide is still active
                                                            if (tempHoldDict.ContainsKey(id))
                                                            {
                                                                tempHoldDict[id].AddNote(new Note(beatsPerMeasure * (measure + subDiv), lane, width) { Type = NoteType.Hold });
                                                            }
                                                            else
                                                                Console.WriteLine("Corresponding hold/slide for this waypoint is not active. Check file. Line: {0}", line);
                                                            break;
                                                        case 7: // Hold / Slide end
                                                            // Check to see this hold/slide is still active
                                                            if (tempHoldDict.ContainsKey(id))
                                                            {
                                                                tempHoldDict[id].AddNote(new Note(beatsPerMeasure * (measure + subDiv), lane, width) { Type = NoteType.Hold });
                                                                CommitHold(tempHoldDict[id]);
                                                                tempHoldDict.Remove(id);
                                                            }
                                                            else
                                                                Console.WriteLine("Corresponding hold/slide for this endpoint is not active. Check file. Line: {0}", line);
                                                            break;
                                                        case 8: // Simple Shuffle Waypoint
                                                            // Check to see this hold/slide is still active
                                                            if (tempHoldDict.ContainsKey(id))
                                                            {
                                                                tempHoldDict[id].AddNote(new Note(beatsPerMeasure * (measure + subDiv), lane, width) { Type = NoteType.Shuffle });
                                                            }
                                                            else
                                                                Console.WriteLine("Corresponding hold/slide for this shuffle is not active. Check file. Line: {0}", line);
                                                            break;
                                                        case 9: // Complex Shuffle waypoint
                                                            // Check to see this hold/slide is still active
                                                            if (tempHoldDict.ContainsKey(id))
                                                            {
                                                                // Check to see if there's enough parameters
                                                                if (note.Length == 6)
                                                                {
                                                                    endLane = note[4].ToString().ParseBase36();
                                                                    endWidth = note[5].ToString().ParseBase36() + 1;
                                                                    tempHoldDict[id].AddNote(new Note(beatsPerMeasure * (measure + subDiv), lane, width, endLane, endWidth) { Type = NoteType.Shuffle });
                                                                }
                                                                else
                                                                    Console.WriteLine("Not enough parameters for this complex shuffle. Line: {0}", line);
                                                            }
                                                            else
                                                                Console.WriteLine("Corresponding hold/slide for this shuffle is not active. Check file. Line: {0}", line);
                                                            break;
                                                        case 10:    // Simple Shuffle End
                                                            // Check to see this hold/slide is still active
                                                            if (tempHoldDict.ContainsKey(id))
                                                            {
                                                                tempHoldDict[id].AddNote(new Note(beatsPerMeasure * (measure + subDiv), lane, width) { Type = NoteType.Shuffle });
                                                                CommitHold(tempHoldDict[id]);
                                                                tempHoldDict.Remove(id);
                                                            }
                                                            else
                                                                Console.WriteLine("Corresponding hold/slide for this shuffle end is not active. Check file. Line: {0}", line);
                                                            break;
                                                        case 11:    // Complex shuffle end
                                                            // Check to see this hold/slide is still active
                                                            if (tempHoldDict.ContainsKey(id))
                                                            {
                                                                // Check to see if there's enough parameters
                                                                if (note.Length == 6)
                                                                {
                                                                    endLane = note[4].ToString().ParseBase36();
                                                                    endWidth = note[5].ToString().ParseBase36() + 1;
                                                                    tempHoldDict[id].AddNote(new Note(beatsPerMeasure * (measure + subDiv), lane, width, endLane, endWidth) { Type = NoteType.Shuffle });
                                                                    CommitHold(tempHoldDict[id]);
                                                                    tempHoldDict.Remove(id);
                                                                }
                                                                else
                                                                    Console.WriteLine("Not enough parameters for this complex shuffle end. Line: {0}", line);
                                                            }
                                                            else
                                                                Console.WriteLine("Corresponding hold/slide for this shuffle end is not active. Check file. Line: {0}", line);
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                }
                                                else
                                                    Console.WriteLine("Failed to parse note: {0}", line);
                                            }
                                        }
                                        else
                                            Console.WriteLine("Empty notestring found: {0}", line);
                                    }
                                }
                            }
                        }
                        else if (Metadata.Version.Major == 0)
                        {
                            // Regex match for standard steps
                            if (Regex.Match(line, "[#][0-9]{3}[1]").Success)
                            {
                                var parsed = ParseLine(line);
                                double noteSub = 1.0 / parsed.Notes.Count;
                                for (int i = 0; i < parsed.Notes.Count; i++)
                                {
                                    switch (parsed.Notes[i].Item1)
                                    {
                                        case 1: // Left Step
                                            Steps.Add(new Note(4 * (parsed.Measure + i * noteSub), parsed.LaneIndex, parsed.Notes[i].Item2, Side.Left));
                                            break;
                                        case 2: // Right Step
                                            Steps.Add(new Note(4 * (parsed.Measure + i * noteSub), parsed.LaneIndex, parsed.Notes[i].Item2, Side.Right));
                                            break;
                                        case 3: // Motion Up
                                            Motions.Add(new Note(4 * (parsed.Measure + i * noteSub), 0, 16, Motion.Up));
                                            break;
                                        case 4: // Motion Down
                                            Motions.Add(new Note(4 * (parsed.Measure + i * noteSub), 0, 16, Motion.Down));
                                            break;
                                        default:    // Rest notes / spacers (0) are ignored
                                            break;
                                    }
                                }
                            }
                            // Regex match for hold/slides
                            else if (Regex.IsMatch(line, "[#][0-9]{3}[2-3]"))
                            {
                                var parsed = ParseLine(line);
                                double noteSub = 1.0 / parsed.Notes.Count;
                                for (int i = 0; i < parsed.Notes.Count; i++)
                                {
                                    Side side = parsed.NoteClass == 2 ? Side.Left : Side.Right;

                                    switch (parsed.Notes[i].Item1)
                                    {
                                        case 1: // Start a new note
                                                // Check to see if a hold is already active-- if so, commit it and start a new one
                                            if (tempSlideDict.ContainsKey(parsed.NoteIdentifier))
                                            {
                                                CommitHold(tempSlideDict[parsed.NoteIdentifier]);
                                                tempSlideDict.Remove(parsed.NoteIdentifier);
                                            }
                                            // Create a collection
                                            tempSlideDict.Add(parsed.NoteIdentifier, new SlideCollection());
                                            // Add this note to it
                                            tempSlideDict[parsed.NoteIdentifier].Notes.Add(new Tuple<NoteParse, int>(parsed, i));
                                            break;
                                        case 2: // End a hold note with no shuffle
                                        case 3: // End a hold note with a shuffle
                                        case 4: // Add a midpoint with no shuffle
                                        case 5: // Add a midpoint with a shuffle
                                            if (!tempSlideDict.ContainsKey(parsed.NoteIdentifier))
                                                tempSlideDict.Add(parsed.NoteIdentifier, new SlideCollection());    // Add a new one (it will fuck up shit if this happens)
                                                                                                                    // Add this note to it
                                            tempSlideDict[parsed.NoteIdentifier].Notes.Add(new Tuple<NoteParse, int>(parsed, i));
                                            break;
                                        default:    // Rest notes / spacers (0) are ignored
                                            break;
                                    }
                                }

                            }
                            // Parse BPM changes
                            else if (Regex.IsMatch(line, "[#][0-9]{3}(08:)"))
                            {
                                //var parsed = ParseLine(line);
                                var split = line.Replace(" ", string.Empty).Split(':');
                                var parsed = new NoteParse();
                                parsed.Measure = Convert.ToDouble(line.Substring(1, 3));
                                parsed.Notes = new List<Tuple<int, int>>();
                                for (int i = 0; i < split[1].Length; i += 2)
                                {
                                    string idStr = split[1][i].ToString() + split[1][i + 1].ToString();
                                    parsed.Notes.Add(new Tuple<int, int>(idStr.ParseBase36(), 0));
                                }

                                double noteSub = 1.0 / parsed.Notes.Count;
                                for (int i = 0; i < parsed.Notes.Count; i++)
                                {
                                    if (parsed.Notes[i].Item1 == 0)
                                        break;
                                    else
                                        Metadata.BpmEvents.Add(new BpmChangeEvent(Metadata.BpmIndex[parsed.Notes[i].Item1], 4 * (parsed.Measure + i * noteSub)));
                                }
                            }
                        }
                    }
                }

                // Close out any remaining hold notes
                foreach (var tempHold in tempSlideDict)
                {
                    CommitHold(tempHold.Value);
                }
                // Sort holds for drawing later
                Holds = Holds.OrderBy(x => x.StartNote.BeatLocation).ToList();

                // Add Beat Markers
                var noteLast = Steps.Count > 0 ? Steps.Max(x => x.BeatLocation) : 0;
                var holdLast = Holds.Count > 0 ? Holds.Max(x => x.Notes.Max(y => y.BeatLocation)) : 0;
                var motionLast = Motions.Count > 0 ? Motions.Max(x => x.BeatLocation) : 0;
                double lastBeat = Math.Max(noteLast, holdLast);
                lastBeat = Math.Ceiling(Math.Max(lastBeat, motionLast));
                for (int i = 0; i <= (int)lastBeat; i += 4)
                    Markers.Add(new BeatMarker(i));

                TotalNotes += Steps.Count();
                TotalNotes += Holds.Count();
                foreach (var hold in Holds)
                {
                    TotalNotes += hold.GradePoints.Count;   // Additional beat counters for holds/slides
                    TotalNotes += hold.Notes.Count(x => x.Type == NoteType.Shuffle);
                }
                TotalNotes += Motions.Count();
            }
            catch (Exception e)
            {
                StyleStarLogger.WriteEntry("Exception in NoteCollection.ParseFile() => Input: " + fileName + ", Exception: " + e.Message + ", Stack Trace: " + e.StackTrace + (e.InnerException != null ? ", Inner Exception: " + e.InnerException.Message : ""));
            }

            return Metadata;
        }

        private NoteParse ParseLine(string line)
        {
            NoteParse parse;
            parse.Notes = new List<Tuple<int, int>>();

            string[] split = line.Split(':');
            string meta = split[0];
            string notes = split[1].Replace(" ", "");

            parse.Measure = Convert.ToDouble(meta.Substring(1, 3));
            parse.NoteClass = Convert.ToInt32(meta.Substring(4, 1));
            parse.LaneIndex = Convert.ToInt32(meta.Substring(5, 1), 16);
            if (meta.Length == 7)
                parse.NoteIdentifier = line.Substring(6, 1).ParseBase36();
            else
                parse.NoteIdentifier = -1;

            for (int i = 0; i < notes.Length; i += 2)
            {
                parse.Notes.Add(new Tuple<int, int>(Convert.ToInt32(notes.Substring(i, 1)), ParseNoteWidth(notes.Substring(i + 1, 1))));
            }

            return parse;
        }

        struct NoteParse
        {
            public double Measure;
            public int NoteClass;
            public int LaneIndex;
            public int NoteIdentifier;
            public List<Tuple<int, int>> Notes;
        }

        private int ParseNoteWidth(string s)
        {
            if (Regex.IsMatch(s, "[0-9]"))
                return Convert.ToInt32(s);
            else
                return Convert.ToInt32(s[0]) - 'a' + 10;
        }

        class SlideCollection
        {
            public List<Tuple<NoteParse, int>> Notes = new List<Tuple<NoteParse, int>>();
            public bool containsStart { get { return Notes.FirstOrDefault(x => x.Item1.NoteClass == 1) == null ? false : true; } }
            public bool containsEnd { get { return Notes.FirstOrDefault(x => x.Item1.NoteClass == 2) == null ? false : true; } }
        }

        // Used for v0.1
        void CommitHold(SlideCollection col)
        {
            Hold tempHold = new Hold(0, 0, 0, Side.NotSet);
            List<Note> tempNotes = new List<Note>();
            for (int i = 0; i < col.Notes.Count; i++)
            {
                var parsed = col.Notes[i].Item1;
                double noteSub = 1.0 / parsed.Notes.Count;
                var j = col.Notes[i].Item2;
                Side side = parsed.NoteClass == 2 ? Side.Left : Side.Right;

                if (i == 0)
                    tempHold = new Hold(4 * (parsed.Measure + j * noteSub), parsed.LaneIndex, parsed.Notes[j].Item2, side);
                else
                {
                    switch (parsed.Notes[j].Item1)
                    {
                        case 1: // Shouldn't happen because it was already added above
                            throw new Exception("Unexpected start note.");
                        case 2: // End a hold note with no shuffle
                        case 4: // Add a midpoint with no shuffle
                            tempNotes.Add(new Note(4 * (parsed.Measure + j * noteSub), parsed.LaneIndex, parsed.Notes[j].Item2, side) { Type = NoteType.Hold });
                            break;
                        case 3: // End a hold note with a shuffle
                        case 5: // Add a midpoint with a shuffle
                            tempNotes.Add(new Note(4 * (parsed.Measure + j * noteSub), parsed.LaneIndex, parsed.Notes[j].Item2, side) { Type = NoteType.Shuffle });
                            break;
                    }
                }
            }
            tempNotes.Sort((x, y) => x.BeatLocation.CompareTo(y.BeatLocation));
            foreach (var note in tempNotes)
                tempHold.AddNote(note);

            Holds.Add(tempHold);
        }

        // Used for v1.0
        private void CommitHold(Hold hold)
        {
            Holds.Add(hold);
        }

        public void AddToScore(NoteType type, float diff)
        {
            switch (type)
            {
                case NoteType.Step:
                case NoteType.Hold:
                    if (diff == Timing.MissFlag)
                    {
                        MaxCombo = Math.Max(MaxCombo, CurrentCombo);
                        CurrentCombo = 0;
                        MissCount++;
                        return;
                    }
                    else
                    {
                        if (Math.Abs(diff) <= NoteTiming.Perfect)
                        {
                            CurrentScore += 1.0;
                            CurrentCombo++;
                            PerfectCount++;
                        }
                        else if (Math.Abs(diff) <= NoteTiming.Good)
                        {
                            CurrentScore += 0.9;
                            CurrentCombo++;
                            GreatCount++;
                        }
                        else if (Math.Abs(diff) <= NoteTiming.Bad)
                        {
                            CurrentScore += 0.5;
                            CurrentCombo++;
                            GoodCount++;
                        }
                    }
                    break;
                case NoteType.Slide:
                    break;
                case NoteType.Shuffle:
                    break;
                case NoteType.Motion:
                    if (diff != Timing.MissFlag)
                    {
                        CurrentScore += 1.0;
                        CurrentCombo++;
                        PerfectCount++;
                    }
                    else
                    {
                        MaxCombo = Math.Max(MaxCombo, CurrentCombo);
                        CurrentCombo = 0;
                        MissCount++;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
