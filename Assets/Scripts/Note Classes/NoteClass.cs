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

        private GameObject noteObject;
        private NoteQuad quadObject;
        private bool isScaleSet = false;
        private bool texturesLoaded = false;
        private double lastSpeed = 0;

        //private Model model;
        //private Matrix world;
        //private MidNoteTexture bgTexture;
        //private NoteTextureBase noteTexture;

        public Note(double beatLoc, int laneIndex, int width, Side side)
        {
            BeatLocation = beatLoc;
            LaneIndex = laneIndex;
            Width = width;
            Side = side;
        }

        public Note(double beatLoc, int laneIndex, int width, Motion motion)
        {
            BeatLocation = beatLoc;
            LaneIndex = laneIndex;
            Width = width;
            Motion = motion;
            Type = NoteType.Motion;
        }

        //public void PreloadTexture(UserSettings settings)
        //{
        //    PreloadTexture(settings, null);
        //}

        //public void PreloadTexture(UserSettings settings, Note prevNote)
        //{
        //    switch (Type)
        //    {
        //        case NoteType.Step:
        //            if (noteTexture == null)
        //                noteTexture = new StepNoteTexture(settings, this);
        //            return;
        //        case NoteType.Motion:
        //            if (noteTexture == null)
        //                noteTexture = new MotionTexture(settings, this);
        //            return;
        //        case NoteType.Hold:
        //            if (bgTexture == null)
        //                bgTexture = new MidNoteTexture(settings, this, prevNote);
        //            break;
        //        case NoteType.Slide:
        //            if (bgTexture == null)
        //                bgTexture = new MidNoteTexture(settings, this, prevNote);
        //            break;
        //        case NoteType.Shuffle:
        //            if (bgTexture == null)
        //                bgTexture = new MidNoteTexture(settings, this, prevNote);
        //            if (noteTexture == null)
        //                noteTexture = new ShuffleNoteTexture(settings, this, prevNote);
        //            break;
        //        default:
        //            break;
        //    }
        //}

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
                        int minLane = LaneIndex < prevNote.LaneIndex ? LaneIndex : prevNote.LaneIndex;
                        int maxLane = (LaneIndex + Width) < (prevNote.LaneIndex + prevNote.Width) ? (prevNote.LaneIndex + prevNote.Width) : (LaneIndex + Width);
                        float flip = LaneIndex > prevNote.LaneIndex ? 1.0f : -1.0f;
                        float center = (float)((minLane - Globals.NumLanes / 2 + (maxLane - minLane) / 2.0f) * Globals.BeatToWorldXUnits);
                        noteObject.transform.position = new Vector3(center, 0, -5);
                        noteObject.transform.localScale = new Vector3(flip * Globals.BaseShuffleScale / 16.0f * (maxLane - minLane), (float)(0.5 * GameState.ScrollSpeed * Globals.ShuffleNoteMultiplier), 1);
                        quadObject.SetVerts(this, prevNote);
                        quadObject.transform.position = new Vector3(quadObject.XOffset, -0.0001f, -10);
                        lastSpeed = GameState.ScrollSpeed;
                        isScaleSet = true;
                    }
                    float noteHeight = (float)(0.5 * lastSpeed * Globals.ShuffleNoteMultiplier);
                    noteObject.transform.position = noteObject.transform.position.ModZ(beatLoc - curDist + (noteHeight - 0.5f) / 2);
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
                    noteObject.transform.localScale = new Vector3(Globals.BaseNoteScale / 16.0f * Width, 0.5f, 1);
                    isScaleSet = true;

                    noteObject.SetActive(true);
                    break;
                case NoteType.Hold:
                    //quadObject = Side == Side.Left ? Pools.LeftHold.GetPooledObject() : Pools.RightHold.GetPooledObject();
                    //noteQuadObject = new NoteQuad(quadObject);
                    quadObject = new NoteQuad(Side == Side.Left ? Pools.LeftHold.GetPooledObject() : Pools.RightHold.GetPooledObject());
                    //quadObject.SetVerts(this, prevNote);
                    quadObject.SetActive(true);
                    //quadObject.SetActive(true);
                    break;
                case NoteType.Slide:
                    quadObject = new NoteQuad(Side == Side.Left ? Pools.LeftSlide.GetPooledObject() : Pools.RightSlide.GetPooledObject()); // FIXME use Slide when appropriate
                    //quadObject.SetVerts(this, prevNote);
                    //quadObject.transform.position = new Vector3(quadObject.XOffset, -0.0001f, -10);
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

        private void GenerateTextureVerts(double currentBeat, Note prevNote)
        {
            //var curDist = (float)Globals.GetDistAtBeat(currentBeat);
            //var beatLoc = (float)Globals.GetDistAtBeat(BeatLocation);

            //var prevBeatLoc = (float)Globals.GetDistAtBeat(prevNote.BeatLocation);
            //float upperLeftX = (float)Globals.CalcTransX(this, Side.Left);
            //float upperRightX = (float)Globals.CalcTransX(this, Side.Right);
            //float lowerLeftX = (float)Globals.CalcTransX(prevNote, Side.Left);
            //float lowerRightX = (float)Globals.CalcTransX(prevNote, Side.Right);
            //Vector3[] verts =
            //{
            //    new Vector3(lowerLeftX, 0, prevBeatLoc - curDist),
            //    new Vector3(lowerRightX, 0, prevBeatLoc - curDist),
            //    new Vector3(upperLeftX, 0, beatLoc - curDist),
            //    new Vector3(upperRightX, 0, beatLoc - curDist),
            //};
            //var mesh = textureObject.GetComponent<MeshFilter>().sharedMesh;
            //mesh.vertices = verts;
        }

        //public void Draw(double currentBeat, Matrix view, Matrix projection)
        //{
        //    // Don't draw if this was hit
        //    if (HitResult.WasHit)
        //        return;

        //    if (Type == NoteType.Step)
        //    {
        //        //if (noteTexture == null)
        //        //    noteTexture = new StepNoteTexture(this);
        //        noteTexture.Draw(currentBeat, view, projection);
        //    }
        //    else if (Type == NoteType.Motion)
        //    {
        //        //if (noteTexture == null)
        //        //    noteTexture = new MotionTexture(this);
        //        noteTexture.Draw(currentBeat, view, projection);
        //    }
        //}

        //public void Draw(double currentBeat, Matrix view, Matrix projection, Note prevNote, int overlapIndex = 0, NoteType type = NoteType.All)
        //{
        //    switch (Type)
        //    {
        //        case NoteType.Step:
        //        case NoteType.Motion:
        //            Draw(currentBeat, view, projection);
        //            return;
        //        case NoteType.Hold:
        //            //if (bgTexture == null)
        //            //    bgTexture = new MidNoteTexture(this, prevNote);
        //            bgTexture.Draw(currentBeat, view, projection, overlapIndex);
        //            break;
        //        case NoteType.Slide:
        //            //if (bgTexture == null)
        //            //    bgTexture = new MidNoteTexture(this, prevNote);
        //            bgTexture.Draw(currentBeat, view, projection, overlapIndex);
        //            break;
        //        case NoteType.Shuffle:
        //            //if (bgTexture == null)
        //            //    bgTexture = new MidNoteTexture(this, prevNote);
        //            if (type == NoteType.All || type == NoteType.Hold || type == NoteType.Slide)
        //                bgTexture.Draw(currentBeat, view, projection, overlapIndex);
        //            //if (noteTexture == null)
        //            //    noteTexture = new ShuffleNoteTexture(this, prevNote);
        //            if (type == NoteType.All || type == NoteType.Shuffle)
        //                ((ShuffleNoteTexture)noteTexture).Draw(currentBeat, view, projection, overlapIndex);
        //            break;
        //        default:
        //            break;
        //    }
        //}
    }
}
