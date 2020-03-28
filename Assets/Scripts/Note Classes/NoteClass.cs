using UnityEngine;

namespace StyleStar
{
    public class Note
    {
        public bool IsLoaded { get; private set; }
        public double BeatLocation { get; private set; }
        public int LaneIndex { get; private set; }
        public int Width { get; set; }
        public NoteType Type { get; set; } = NoteType.Step;
        public Side Side { get; set; } = Side.NotSet;
        public Motion Motion { get; set; } = Motion.NotSet;
        public HitResult HitResult { get; set; } = new HitResult();

        // Only used for Shuffles
        public int EndLaneIndex { get; private set; } = int.MinValue;
        public int EndWidth { get; private set; } = int.MinValue;
        public bool IsComplexShuffle { get { return EndLaneIndex != int.MinValue && EndWidth != int.MinValue; } }

        private GameObject noteObject;
        private NoteQuad quadObject;
        private bool isScaleSet = false;
        private bool texturesLoaded = false;
        private double lastSpeed = 0;

        public Note(double beatLoc, int laneIndex, int width)
        {
            BeatLocation = beatLoc;
            LaneIndex = laneIndex;
            Width = width;
        }

        public Note(double beatLoc, int laneIndex, int width, Side side) 
            : this(beatLoc, laneIndex, width)
        {
            Side = side;
        }

        public Note(double beatLoc, int laneIndex, int width, Motion motion)
            : this(beatLoc, laneIndex, width)
        {
            Motion = motion;
            Type = NoteType.Motion;
        }

        public Note(double beatLoc, int laneIndex, int width, int endLaneIndex, int endWidth)
            : this(beatLoc, laneIndex, width)
        {
            EndLaneIndex = endLaneIndex;
            EndWidth = endWidth;
        }

        public void Draw(double currentBeat)
        {
            // Don't draw if this was hit and return to pool if it's active
            if (HitResult.WasHit)
            {
                if (noteObject != null)
                {
                    noteObject.SetActive(false);
                    noteObject = null;
                }
                return;
            }

            if (!texturesLoaded)
                GetNoteObject();

            var curDist = (float)Globals.GetDistAtBeat(currentBeat);
            var beatLoc = (float)Globals.GetDistAtBeat(BeatLocation);

            switch (Type)
            {
                case NoteType.Step:
                case NoteType.Motion:
                    noteObject.transform.position = noteObject.transform.position.ModZ(beatLoc - curDist);
                    break;
                case NoteType.Hold:
                    break;
                case NoteType.Slide:
                    break;
                case NoteType.Shuffle:
                    break;
                default:
                    break;
            }
        }

        public void Draw(double currentBeat, Note prevNote, NoteType type = NoteType.All)
        {
            if (!texturesLoaded)
                GetNoteObject(prevNote);

            var curDist = (float)Globals.GetDistAtBeat(currentBeat);
            var beatLoc = (float)Globals.GetDistAtBeat(BeatLocation);

            bool regen = !isScaleSet || (lastSpeed != GameState.ScrollSpeed);

            switch (Type)
            {
                case NoteType.Step:
                case NoteType.Motion:
                    Draw(currentBeat);
                    break;
                case NoteType.Hold:
                    if (regen)
                    {
                        quadObject.SetVerts(this, prevNote);
                        quadObject.transform.position = new Vector3(quadObject.XOffset, -0.0001f, beatLoc - curDist - quadObject.YOffset);
                        lastSpeed = GameState.ScrollSpeed;
                        isScaleSet = true;
                    }
                    quadObject.transform.position = quadObject.transform.position.ModZ(beatLoc - curDist - quadObject.YOffset);
                    break;
                case NoteType.Slide:
                    if(regen)
                    {
                        quadObject.SetVerts(this, prevNote);
                        quadObject.transform.position = new Vector3(quadObject.XOffset, -0.0001f, beatLoc - curDist - quadObject.YOffset);
                        lastSpeed = GameState.ScrollSpeed;
                        isScaleSet = true;
                    }
                    quadObject.transform.position = quadObject.transform.position.ModZ(beatLoc - curDist - quadObject.YOffset);
                    break;
                case NoteType.Shuffle:
                    if (regen)
                    {
                        int minLane, maxLane;
                        float flip, center;
                        if (this.IsComplexShuffle)
                        {
                            minLane = LaneIndex < EndLaneIndex ? LaneIndex : EndLaneIndex;
                            maxLane = (LaneIndex + Width) < (EndLaneIndex + EndWidth) ? (EndLaneIndex + EndWidth) : (LaneIndex + Width);
                            flip = LaneIndex > prevNote.LaneIndex ? -1.0f : 1.0f;
                        }
                        else
                        {
                            minLane = LaneIndex < prevNote.LaneIndex ? LaneIndex : prevNote.LaneIndex;
                            maxLane = (LaneIndex + Width) < (prevNote.LaneIndex + prevNote.Width) ? (prevNote.LaneIndex + prevNote.Width) : (LaneIndex + Width);
                            flip = LaneIndex > prevNote.LaneIndex ? 1.0f : -1.0f;
                        }
                        center = (float)((minLane - Globals.NumLanes / 2 + (maxLane - minLane) / 2.0f) * Globals.BeatToWorldXUnits);
                        noteObject.transform.position = new Vector3(center, 0, -5);
                        noteObject.transform.localScale = new Vector3(flip * Globals.BaseShuffleScale / 16.0f * (maxLane - minLane), (float)(Globals.BaseNoteZScale * GameState.ScrollSpeed * Globals.ShuffleNoteMultiplier), 1);
                        quadObject.SetVerts(this, prevNote);
                        quadObject.transform.position = new Vector3(quadObject.XOffset, -0.0001f, -10);
                        lastSpeed = GameState.ScrollSpeed;
                        isScaleSet = true;
                    }
                    float noteHeight = (float)(Globals.BaseNoteZScale * lastSpeed * Globals.ShuffleNoteMultiplier);
                    //noteObject.transform.position = noteObject.transform.position.ModZ(beatLoc - curDist + (noteHeight - 0.5f) / 2);
                    noteObject.transform.position = noteObject.transform.position.ModZ(beatLoc - curDist + noteHeight / 2);
                    quadObject.transform.position = quadObject.transform.position.ModZ(beatLoc - curDist - quadObject.YOffset);
                    break;
                default:
                    break;
            }
        }

        private void GetNoteObject(Note prevNote = null)
        {
            // Grab pooled object
            switch (Type)
            {
                case NoteType.Step:
                    noteObject = Side == Side.Left ? Pools.LeftStep.GetPooledObject() : Pools.RightStep.GetPooledObject();
                    noteObject.transform.position = new Vector3((float)Globals.CalcTransX(this), 0, -5);
                    noteObject.transform.localScale = new Vector3(Globals.BaseNoteScale / 16.0f * Width, Globals.BaseNoteZScale, 1);
                    isScaleSet = true;

                    noteObject.SetActive(true);
                    break;
                case NoteType.Hold:
                    quadObject = new NoteQuad(Side == Side.Left ? Pools.LeftHold.GetPooledObject() : Pools.RightHold.GetPooledObject());
                    quadObject.SetActive(true);
                    break;
                case NoteType.Slide:
                    quadObject = new NoteQuad(Side == Side.Left ? Pools.LeftSlide.GetPooledObject() : Pools.RightSlide.GetPooledObject());
                    quadObject.SetActive(true);
                    break;
                case NoteType.Shuffle:
                    noteObject = Side == Side.Left ? Pools.LeftShuffle.GetPooledObject() : Pools.RightShuffle.GetPooledObject();
                    noteObject.transform.position = new Vector3(0, 0, -5);
                    noteObject.SetActive(true);

                    //quadObject = Side == Side.Left ? Pools.LeftHold.GetPooledObject() : Pools.RightHold.GetPooledObject();   // FIXME use Slide when appropriate
                    //noteQuadObject = new NoteQuad(quadObject);
                    quadObject = new NoteQuad(Side == Side.Left ? Pools.LeftHold.GetPooledObject() : Pools.RightHold.GetPooledObject()); // FIXME use Slide when appropriate
                    //quadObject.SetVerts(this, prevNote);
                    //quadObject.transform.position = new Vector3(quadObject.XOffset, -0.0001f, -10);
                    quadObject.SetActive(true);
                    //quadObject.SetActive(true);
                    break;
                case NoteType.Motion:
                    noteObject = Motion == Motion.Down ? Pools.MotionDown.GetPooledObject() : Pools.MotionUp.GetPooledObject();
                    noteObject.transform.position = new Vector3(0, 0, -5);
                    isScaleSet = true;

                    noteObject.SetActive(true);
                    break;
                default:
                    break;
            }

            texturesLoaded = true;
        }
    }
}
