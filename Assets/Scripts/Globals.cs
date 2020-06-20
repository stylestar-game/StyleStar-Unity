using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace StyleStar
{
    public static class Globals
    {
        //public static ContentManager ContentManager { get; set; }
        //public static GraphicsDeviceManager GraphicsManager { get; set; }

        //public static Vector2 WindowSize { get; set; }
        //public static ConfigFile ConfigFile { get; set; }

        public static double NoteSpeed { get; set; } = 5;    // World units per second
        public static double BeatToWorldXUnits { get; set; } = 0.375;
        public static double StepNoteHeightOffset { get; set; } = 0.5;
        public static double ShuffleNoteHeightOffset { get; set; } = 7.5;
        public static double ShuffleNoteMultiplier { get; set; } = 0.666;
        public static double ShuffleXOffset { get; set; } = 0.7;
        public static double YOffset { get; set; } = 1;
        public static float OverlapMultplier { get; set; } = -0.02f;
        public static int NumLanes = 16;
        public static float GradeZoneWidth { get; set; } = 6f;
        public static float NoteLaneAccentWidth { get; set; } = 3f;
        public static float BaseNoteZScale { get; set; } = 0.52f;
        public static float BaseNoteScale { get; set; } = 0.555f;
        public static float BaseLanePosition { get; set; } = 2.0f;
        public static float BaseShuffleScale { get; set; } = 0.78f;
        public static float BeatMarkerHeightOffset { get; set; } = 0.47f;

        public static float CurrentScalingFactor { get; set; } = 1.5f;

        public static float FootWidth { get; set; } = 4f;

        public static float LoadingScreenTransitionTime = 0.5f;
        public static float LoadingScreenWidth = 880f;

        //public static Vector2 Origin = new Vector2(0, 0);
        //public static Vector2 ItemOrigin = new Vector2(144, 212);
        public static Vector2 CardOrigin = new Vector2(-288, 88);
        public static Vector2 CardOffset = new Vector2(-75, -130);

        // public static double CurrentBpm { get; set; }
        public static List<BpmChangeEvent> BpmEvents { get; set; }

        public static NoteCollection CurrentNoteCollection { get; set; }
        public static SongMetadata CurrentSongMetadata { get; set; }
        public static MusicManager MusicManager { get; set; } = new MusicManager();

        //public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        //public static BasicEffect Effect;

        //public static Dictionary<string, SpriteFont> Font { get; set; }

        //public static GameSettingsScreen.AutoMode AutoMode { get; set; } = GameSettingsScreen.AutoMode.Off;

        // DEBUG STUFF
        //public static bool TouchProfiling = true;
        //public static Stopwatch TouchStopwatch = new Stopwatch();
        //public static List<DrawCycleEventLog> TouchCycleLog = new List<DrawCycleEventLog>();

        //public static bool DrawProfiling = true;
        //public static Stopwatch DrawStopwatch = new Stopwatch();
        //public static List<DrawCycleEventLog> DrawCycleLog = new List<DrawCycleEventLog>();
        //public static DrawCycleEventLog DrawTempLog;

        public static double CalcTransX(Note note, Side side = Side.NotSet)
        {
            float del = note.Width / 2.0f;
            switch (side)
            {
                case Side.Left:
                    del = 0;
                    break;
                case Side.Right:
                    del = note.Width;
                    break;
                case Side.NotSet:
                default:
                    break;
            }

            return (note.LaneIndex - NumLanes / 2.0f + del) * BeatToWorldXUnits;
        }

        public static double CalcTransX(float index, float width, Side side)
        {
            double del = width / 2.0f;
            switch (side)
            {
                case Side.Left:
                    del = 0;
                    break;
                case Side.Right:
                    del = width;
                    break;
                case Side.NotSet:
                default:
                    break;
            }

            return (index - NumLanes / 2.0f + del) * BeatToWorldXUnits;
        }

        public static double GetDistAtBeat(double beat)
        {
            // Determine which BPM change we're on (if any)
            var evt = BpmEvents.Where(x => beat >= x.StartBeat).LastOrDefault();
            if (evt == null)
                evt = BpmEvents[0];

            double curDist = 0;
            if (evt.StartBeat > 0)
            {
                // Add up all the distance prior
                for (int i = 0; i < BpmEvents.Count; i++)
                {
                    if (BpmEvents[i].StartBeat == evt.StartBeat)
                        i = BpmEvents.Count;
                    else
                    {
                        curDist += (NoteSpeed * GameState.ScrollSpeed * 60 / BpmEvents[i].BPM) * (BpmEvents[i + 1].StartBeat - BpmEvents[i].StartBeat);
                    }
                }
            }

            return curDist + (NoteSpeed * GameState.ScrollSpeed * 60 / evt.BPM) * (beat - evt.StartBeat);
        }

        public static double GetSecAtBeat(double beat)
        {
            // Determine which BPM change we're on (if any)
            var evt = BpmEvents.Where(x => beat >= x.StartBeat).LastOrDefault();
            if (evt == null)
                evt = BpmEvents[0];

            //double curTime = 0;
            //if (evt.StartBeat > 0)
            //{
            //    // Add up all the time prior
            //    for (int i = 0; i < BpmEvents.Count; i++)
            //    {
            //        if (BpmEvents[i].StartBeat == evt.StartBeat)
            //            i = BpmEvents.Count;
            //        else
            //            curTime = BpmEvents[i + 1].StartSeconds;
            //    }
            //}

            return evt.StartSeconds + ((beat - evt.StartBeat) / evt.BPM * 60);
        }

        //public static void LoadTextures()
        //{
        //    Textures["StepLeft"] = ContentManager.Load<Texture2D>("StepLeft");
        //    Textures["StepRight"] = ContentManager.Load<Texture2D>("StepRight");
        //    Textures["HoldLeft"] = ContentManager.Load<Texture2D>("HoldLeft");
        //    Textures["HoldRight"] = ContentManager.Load<Texture2D>("HoldRight");
        //    Textures["SlideLeft"] = ContentManager.Load<Texture2D>("SlideLeft");
        //    Textures["SlideRight"] = ContentManager.Load<Texture2D>("SlideRight");
        //    Textures["MotionUp"] = ContentManager.Load<Texture2D>("MotionUp");
        //    Textures["MotionDown"] = ContentManager.Load<Texture2D>("MotionDown");
        //    Textures["ShuffleLeft_R"] = ContentManager.Load<Texture2D>("ShuffleLeft_R");
        //    Textures["ShuffleLeft_L"] = ContentManager.Load<Texture2D>("ShuffleLeft_L");
        //    Textures["ShuffleRight_R"] = ContentManager.Load<Texture2D>("ShuffleRight_R");
        //    Textures["ShuffleRight_L"] = ContentManager.Load<Texture2D>("ShuffleRight_L");
        //    Textures["FootLeft"] = ContentManager.Load<Texture2D>("FootLeft");
        //    Textures["FootRight"] = ContentManager.Load<Texture2D>("FootRight");
        //    Textures["FootHold"] = ContentManager.Load<Texture2D>("FootHold");
        //    Textures["BeatMark"] = ContentManager.Load<Texture2D>("BeatMarker");

        //    // New Step Textures
        //    Textures["StepLeftPink"] = ContentManager.Load<Texture2D>("NoteTextures/StepLeftPink");
        //    Textures["StepRightPink"] = ContentManager.Load<Texture2D>("NoteTextures/StepRightPink");
        //    Textures["StepLeftBlue"] = ContentManager.Load<Texture2D>("NoteTextures/StepLeftBlue");
        //    Textures["StepRightBlue"] = ContentManager.Load<Texture2D>("NoteTextures/StepRightBlue");
        //    Textures["HoldPink"] = ContentManager.Load<Texture2D>("NoteTextures/HoldPink");
        //    Textures["HoldBlue"] = ContentManager.Load<Texture2D>("NoteTextures/HoldBlue");
        //    Textures["SlidePink"] = ContentManager.Load<Texture2D>("NoteTextures/SlidePink");
        //    Textures["SlideBlue"] = ContentManager.Load<Texture2D>("NoteTextures/SlideBlue");
        //    Textures["ShuffleLeftPink"] = ContentManager.Load<Texture2D>("NoteTextures/ShuffleLeftPink");
        //    Textures["ShuffleRightPink"] = ContentManager.Load<Texture2D>("NoteTextures/ShuffleRightPink");
        //    Textures["ShuffleLeftBlue"] = ContentManager.Load<Texture2D>("NoteTextures/ShuffleLeftBlue");
        //    Textures["ShuffleRightBlue"] = ContentManager.Load<Texture2D>("NoteTextures/ShuffleRightBlue");

        //    Textures["PerfectGrade"] = ContentManager.Load<Texture2D>("PerfectGrade");
        //    Textures["GreatGrade"] = ContentManager.Load<Texture2D>("GreatGrade");
        //    Textures["GoodGrade"] = ContentManager.Load<Texture2D>("GoodGrade");
        //    Textures["BadGrade"] = ContentManager.Load<Texture2D>("BadGrade");
        //    Textures["MissGrade"] = ContentManager.Load<Texture2D>("MissGrade");

        //    Textures["HitTexture"] = ContentManager.Load<Texture2D>("HitTexture");

        //    Textures["SsActive"] = ContentManager.Load<Texture2D>("SongSelection_Active");
        //    Textures["SsBgLine"] = ContentManager.Load<Texture2D>("SongSelection_BgLine");
        //    Textures["SsFrame"] = ContentManager.Load<Texture2D>("SongSelection_Frame");
        //    Textures["SsAlbumFrame"] = ContentManager.Load<Texture2D>("SongSelection_AlbumFrame");
        //    Textures["SsItemBg"] = ContentManager.Load<Texture2D>("SongSelection_BG");
        //    Textures["SsAccentStar"] = ContentManager.Load<Texture2D>("SongSelection_AccentStar");
        //    Textures["SsAccentAlbum"] = ContentManager.Load<Texture2D>("SongSelection_AccentAlbum");
        //    Textures["SsDifficultyBg"] = ContentManager.Load<Texture2D>("SongSelection_DifficultyBG");
        //    Textures["SsActiveDifficulty0"] = ContentManager.Load<Texture2D>("SongSelection_ActiveDifficulty0");
        //    Textures["SsActiveDifficulty1"] = ContentManager.Load<Texture2D>("SongSelection_ActiveDifficulty1");
        //    Textures["SsActiveDifficulty2"] = ContentManager.Load<Texture2D>("SongSelection_ActiveDifficulty2");

        //    Textures["ResultBg1"] = ContentManager.Load<Texture2D>("Result_BgLine1");
        //    Textures["ResultBg2"] = ContentManager.Load<Texture2D>("Result_BgLine2");
        //    Textures["ResultBg3"] = ContentManager.Load<Texture2D>("Result_BgLine3");
        //    Textures["ResultBgRight"] = ContentManager.Load<Texture2D>("Result_BgRight");
        //    for (int i = 0; i <= 7; i++)
        //        Textures["Star" + i] = ContentManager.Load<Texture2D>("Star_" + i);

        //    Textures["SsFolderSelect"] = ContentManager.Load<Texture2D>("SongSelection_FolderSelect");
        //    Textures["SsSongSelect"] = ContentManager.Load<Texture2D>("SongSelection_SongSelect");
        //    Textures["SsGoBack"] = ContentManager.Load<Texture2D>("SongSelection_GoBack");

        //    Textures["FallbackJacket"] = ContentManager.Load<Texture2D>("Fallback_Jacket");

        //    Textures["GpLowerBG"] = ContentManager.Load<Texture2D>("Gameplay_LowerBG");

        //    Textures["SettingsBg1"] = ContentManager.Load<Texture2D>("Settings_BG1");
        //    Textures["SettingsBg2"] = ContentManager.Load<Texture2D>("Settings_BG2");
        //    Textures["SettingsBg3"] = ContentManager.Load<Texture2D>("Settings_BG3");
        //    Textures["SettingsSelection"] = ContentManager.Load<Texture2D>("Settings_Selection");

        //    Effect = new BasicEffect(GraphicsManager.GraphicsDevice);
        //}
    }

    public static class GameState
    {
        public static Mode GameMode { get; set; } = Mode.SongSelect;
        public static Mode Destination { get; set; } = Mode.SongSelect;
        public static double ScrollSpeed { get; set; } = 3.0;
        public static TransitionState TransitionState { get; set; } = TransitionState.ScreenActive;
        public static Settings CurrentSettings { get; set; } = new Settings() { AutoSetting = Settings.AutoMode.Auto };
    }

    public enum Motion
    {
        NotSet,
        Up,
        Down
    }

    public enum Side
    {
        NotSet,
        Left,
        Right
    }

    public enum NoteType
    {
        Step,
        Hold,
        Slide,
        Shuffle,
        Motion,
        All
    }

    public enum Mode
    {
        MainMenu,
        GameSettings,
        SongSelect,
        Loading,
        GamePlay,
        Results,
        NoSongs
    }

    public enum TransitionState
    {
        LeavingLoadScreen,
        ScreenActive,
        EnteringLoadingScreen,
        SwitchingScreens
    }

    public enum Difficulty
    {
        Easy = 0,
        Normal,
        Hard
    }

    public enum SongEndReason
    {
        Undefined,
        Forfeit,
        Failed,
        Cleared
    }

    public enum DialogResult
    {
        NoAction,
        Confirm,
        Cancel
    }

    public enum Language
    {
        English,
        Spanish,
        Japanese,
        French
    }
}
