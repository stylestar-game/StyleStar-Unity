using System;
using System.Collections.Generic;
using System.Linq;

namespace StyleStar
{
    public class Hold
    {
        public Note StartNote { get; private set; }
        public List<Note> Notes { get; private set; }

        public bool IsLoaded { get; private set; }

        public bool IsPlayerHolding { get; private set; }
        //private QuadTexture HitTexture;

        // Start note is not included since it already has its own grade
        public List<HoldGrade> GradePoints { get; private set; } = new List<HoldGrade>();

        public Hold(double beatLoc, int laneIndex, int width, Side side)
        {
            StartNote = new Note(beatLoc, laneIndex, width, side);
            Notes = new List<Note>();
        }

        public void AddNote(Note note)
        {
            var lastNote = Notes.Count == 0 ? StartNote : Notes.Last();

            // If this note is marked as hold, determine if it should be reclassified as a slide
            if (note.Type == NoteType.Hold)
            {
                if (lastNote.LaneIndex != note.LaneIndex || lastNote.Width != note.Width)
                    note.Type = NoteType.Slide;
            }

            // Determine if the note-to-be-added needs extra grade points
            if (note.Type == NoteType.Hold || note.Type == NoteType.Slide)
            {
                // Holds and slides are measured on n-beats after the start note (except the last one)
                for (double beat = (StartNote.BeatLocation + 1); beat < note.BeatLocation; beat += 1.0)
                {
                    // If a point doesn't already exist at this time
                    if (GradePoints.Find(x => x.GradeBeat == beat) == null)
                    {
                        // And if a shuffle doesn't exist at this time
                        if (Notes.Find(x => x.BeatLocation == beat && x.Type == NoteType.Shuffle) == null)
                            GradePoints.Add(new HoldGrade() { GradeBeat = beat });  // Add a grade point
                    }
                }
            }

            Notes.Add(note);
        }

        public void Draw(double currentBeat)
        {
            // Draw order
            // 1. Hold bodies
            // 2. Slides
            // 3. Shuffles
            // 4. Notes
            // 5. Start note
            // 6. Hit texture

            
            for (int i = 0; i < Notes.Count; i++)
            {
                var prevNote = i == 0 ? StartNote : Notes[i - 1];
                // Hold bodies
                if(Notes[i].Type == NoteType.Hold)
                    Notes[i].Draw(currentBeat, prevNote, NoteType.Hold);

                // Slides
                if(Notes[i].Type == NoteType.Slide)
                    Notes[i].Draw(currentBeat, prevNote, NoteType.Slide);

                // Shuffles
                if (Notes[i].Type == NoteType.Shuffle)
                    Notes[i].Draw(currentBeat, prevNote, NoteType.Shuffle);

                // Notes
            }

            // Start note
            StartNote.Draw(currentBeat);

            // Hit texture
        }

        //public void Draw(double currentBeat, Matrix view, Matrix projection, int overlapIndex = 0)
        //{
        //    // Draw order
        //    // Hold bodies
        //    // Slides
        //    // Then shuffles
        //    // The Notes

        //    // Draw hold bodies first
        //    for (int i = 0; i < Notes.Count; i++)
        //    {
        //        var prevNote = i == 0 ? StartNote : Notes[i - 1];
        //        if (Notes[i].Type == NoteType.Hold || Notes[i].Type == NoteType.Shuffle)
        //            Notes[i].Draw(currentBeat, view, projection, prevNote, overlapIndex, NoteType.Hold);
        //    }
        //    // Then draw slides
        //    for (int i = 0; i < Notes.Count; i++)
        //    {
        //        var prevNote = i == 0 ? StartNote : Notes[i - 1];
        //        if (Notes[i].Type == NoteType.Slide)
        //            Notes[i].Draw(currentBeat, view, projection, prevNote, overlapIndex); ;
        //    }
        //    // Then draw shuffles
        //    for (int i = 0; i < Notes.Count; i++)
        //    {
        //        var prevNote = i == 0 ? StartNote : Notes[i - 1];
        //        if (Notes[i].Type == NoteType.Shuffle)
        //            Notes[i].Draw(currentBeat, view, projection, prevNote, overlapIndex, NoteType.Shuffle); ;
        //    }
        //    // Then draw notes
        //    for (int i = 0; i < Notes.Count; i++)
        //    {
        //        var prevNote = i == 0 ? StartNote : Notes[i - 1];
        //        if (Notes[i].Type == NoteType.Step)
        //            Notes[i].Draw(currentBeat, view, projection, prevNote, overlapIndex); ;
        //    }

        //    // Draw start note
        //    StartNote.Draw(currentBeat, view, projection);

        //    // Draw hit texture if necessary
        //    if (Notes.Last().BeatLocation < currentBeat)    // Sanity check first
        //        IsPlayerHolding = false;
        //    if (IsPlayerHolding)
        //        HitTexture.Draw(view, projection);
        //}

        //public void PreloadTexture(UserSettings settings)
        //{
        //    for (int i = 0; i < Notes.Count; i++)
        //    {
        //        var prevNote = i == 0 ? StartNote : Notes[i - 1];
        //        Notes[i].PreloadTexture(settings, prevNote);
        //    }
        //    StartNote.PreloadTexture(settings);
        //    HitTexture = new QuadTexture(Globals.Textures["HitTexture"]);
        //}

        //public HitState CheckHold(TouchCollection tc, double currentBeat)
        //{
        //    if (currentBeat < StartNote.BeatLocation)
        //    {
        //        IsPlayerHolding = false;
        //        return HitState.Unknown;
        //    }

        //    Note firstNote, secondNote;
        //    if (Notes[0].BeatLocation > currentBeat)
        //    {
        //        firstNote = StartNote;
        //        secondNote = Notes[0];
        //    }
        //    else if (Notes.Count < 2 || Notes.Last().BeatLocation < currentBeat)
        //    {
        //        IsPlayerHolding = false;
        //        return HitState.Unknown;
        //    }
        //    else
        //    {
        //        firstNote = Notes.Reverse<Note>().First(x => x.BeatLocation <= currentBeat);
        //        secondNote = Notes.First(x => x.BeatLocation >= currentBeat);
        //    }

        //    if (firstNote == null || secondNote == null)
        //    {
        //        IsPlayerHolding = false;
        //        return HitState.Unknown;
        //    }

        //    bool useFirstNote = false;
        //    double noteMin = 0, noteMax = 0;

        //    // Interpolate based on currentBeat
        //    switch (secondNote.Type)
        //    {
        //        case NoteType.Step:         // Shouldn't be possible
        //        case NoteType.Motion:
        //            IsPlayerHolding = false;
        //            return HitState.Unknown;
        //        case NoteType.Hold:         // Lane index and width is the same as the first note
        //        case NoteType.Shuffle:      // Actual shuffle motion is not calculated here (but perhaps we need to figure in some dead space?)
        //            useFirstNote = true;
        //            break;
        //        case NoteType.Slide:        // Lane index and width must be interpolated
        //            double fNoteMin = Globals.CalcTransX(firstNote, Side.Left);
        //            double fNoteMax = Globals.CalcTransX(firstNote, Side.Right);
        //            double sNoteMin = Globals.CalcTransX(secondNote, Side.Left);
        //            double sNoteMax = Globals.CalcTransX(secondNote, Side.Right);
        //            double ratio = (currentBeat - firstNote.BeatLocation) / (secondNote.BeatLocation - firstNote.BeatLocation);
        //            noteMin = (sNoteMin - fNoteMin) * ratio + fNoteMin;
        //            noteMax = (sNoteMax - fNoteMax) * ratio + fNoteMax;
        //            break;
        //        default:
        //            break;
        //    }

        //    if (useFirstNote)
        //    {
        //        noteMin = Globals.CalcTransX(firstNote, Side.Left);
        //        noteMax = Globals.CalcTransX(firstNote, Side.Right);
        //    }

        //    var validPoints = tc.Points.Where(x => x.Value.MinX < noteMax && x.Value.MaxX > noteMin).ToList();
        //    if (validPoints.Count == 0 && !(Globals.AutoMode == GameSettingsScreen.AutoMode.Auto))
        //    {
        //        IsPlayerHolding = false;
        //    }
        //    else
        //    {
        //        IsPlayerHolding = true;
        //        HitTexture.SetVerts((float)noteMax, (float)noteMin, (float)-Globals.StepNoteHeightOffset, (float)Globals.StepNoteHeightOffset, 0.1f);
        //    }

        //    var gradeBeat = GradePoints.Find(x => Math.Abs(x.GradeBeat - currentBeat) < NoteTiming.BeatTolerance && x.State == HitState.Unknown);
        //    if (gradeBeat != null)
        //    {
        //        if (IsPlayerHolding)
        //            gradeBeat.State = HitState.Hit;
        //        else
        //            gradeBeat.State = HitState.Miss;

        //        return gradeBeat.State;
        //    }

        //    return HitState.Unknown;
        //}
    }

    public class HoldGrade
    {
        public double GradeBeat;
        public HitState State;
    }
}
