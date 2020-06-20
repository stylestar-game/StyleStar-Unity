using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using StyleStar;
using UnityEngine.SceneManagement;
using System;

public class GameplayDisplay : MonoBehaviour
{
    public GameObject CanvasObj;


    List<GameObject> activeNotes;
    public float CullLocation;

    //MusicManager musicManager;

    //NoteCollection currentSongNotes;
    //SongMetadata currentSongMeta;

    private RectTransform loadingLeft;
    private RectTransform loadingRight;



    private float transitionStartTime = -1;
    private bool loadStarted = false;
    private bool transitionFinished;

    // Initialize things
    //private void Awake()
    //{
    //    musicManager = new MusicManager();
    //}

    private void Awake()
    {
        transitionFinished = false;
        SceneManager.activeSceneChanged += SceneChanged;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Load list of songs

        if(SongSelection.Songlist.Count == 0)
        {
            SongSelection.ImportSongs(Defines.SongFolder);
            var meta = SongSelection.Songlist[0];   // 8 is Summery
            Globals.CurrentNoteCollection = new NoteCollection(meta);
            Globals.CurrentSongMetadata = Globals.CurrentNoteCollection.ParseFile();

            Globals.MusicManager.LoadSong(Globals.CurrentSongMetadata.FilePath + Globals.CurrentSongMetadata.SongFilename, Globals.CurrentSongMetadata.BpmEvents);
            Globals.MusicManager.Offset = Globals.CurrentSongMetadata.PlaybackOffset * 1000;
        }

        GameObject.Find("DifficultyText").SetText(ConfigFile.GetLocalizedString("Sort_Title"));

        loadingLeft = GameObject.Find("LoadingLeft").GetComponent<RectTransform>();
        loadingRight = GameObject.Find("LoadingRight").GetComponent<RectTransform>();

        // Set sorting order for non-sprites
        GameObject.Find("LeftHold").GetComponent<MeshRenderer>().sortingLayerName = "LeftHold";
        GameObject.Find("RightHold").GetComponent<MeshRenderer>().sortingLayerName = "Rightold";
        GameObject.Find("LeftSlide").GetComponent<MeshRenderer>().sortingLayerName = "LeftHold";
        GameObject.Find("RightSlide").GetComponent<MeshRenderer>().sortingLayerName = "RightHold";
    }

    // Update is called once per frame
    void Update()
    {
        // Wait until the scene is fully loaded
        if (!transitionFinished)
            return;

        //if (Input.GetKeyDown(KeyCode.J))
        //    activeNotes.ForEach(x => x.transform.position -= new Vector3(0, 0, 0.5f));

        //var inactive = activeNotes.Where(x => x.transform.position.z < CullLocation);
        //for (int i = 0; i < inactive.Count(); i++)
        //{
        //    inactive.ElementAt(i).SetActive(false);
        //    activeNotes.Remove(inactive.ElementAt(i));
        //}

        

        // Draw notes (always)
        if(Globals.CurrentNoteCollection != null)
        {
            var currentBeat = Globals.MusicManager.GetCurrentBeat();
            var motions = Globals.CurrentNoteCollection.Motions.Where(p => p.BeatLocation > currentBeat - 6 && p.BeatLocation < currentBeat + 16);
            var holds = Globals.CurrentNoteCollection.Holds.Where(p => p.StartNote.BeatLocation > currentBeat - 16 && p.StartNote.BeatLocation < currentBeat + 16);
            var notes = Globals.CurrentNoteCollection.Steps.Where(p => p.BeatLocation > currentBeat - 6 && p.BeatLocation < currentBeat + 16);
            var marks = Globals.CurrentNoteCollection.Markers.Where(p => p.BeatLocation > currentBeat - 6 && p.BeatLocation < currentBeat + 16);

            foreach (var mark in marks)
                mark.Draw(currentBeat);

            foreach (var motion in motions)
                motion.Draw(currentBeat);

            foreach (var hold in holds)
                hold.Draw(currentBeat);

            foreach (var note in notes)
                note.Draw(currentBeat);
        }

        // Draw foot markers
        TouchCollection.Draw();

        // Update UI
        CanvasObj.transform.Find("ScrollNumMajor").gameObject.SetText(((int)Math.Floor(GameState.ScrollSpeed)).ToString("D1"));
        CanvasObj.transform.Find("ScrollNumMinor").gameObject.SetText("." + ((int)((GameState.ScrollSpeed - Math.Truncate(GameState.ScrollSpeed)) * 10)).ToString("D1"));
        var score = Globals.CurrentNoteCollection.CurrentScore / Globals.CurrentNoteCollection.TotalNotes * 100.00f;
        CanvasObj.transform.Find("ScoreNumMajor").gameObject.SetText(((int)Math.Floor(score)).ToString("D3"));
        CanvasObj.transform.Find("ScoreNumMinor").gameObject.SetText("." + ((int)((score - Math.Truncate(score)) * 1000)).ToString("D3") + "%");
        CanvasObj.transform.Find("AutoModeLabel").gameObject.SetActive(GameState.CurrentSettings.AutoSetting != Settings.AutoMode.Off);

        // Transitions
        float ratio;
        switch (GameState.TransitionState)
        {
            case TransitionState.LeavingLoadScreen:
                SetFields();
                ratio = DrawLoadingScreenTransition(false);
                if (ratio >= 1.0f)
                {
                    GameState.TransitionState = TransitionState.ScreenActive;
                    if (!Globals.MusicManager.IsPlaying)  // UNDO LATER
                        Globals.MusicManager.Play();
                }
                break;
            case TransitionState.ScreenActive:
                transitionStartTime = -1;
                if(Globals.MusicManager.IsFinished)
                {
                    Globals.CurrentNoteCollection.SongEnd = SongEndReason.Cleared;
                    GameState.TransitionState = TransitionState.EnteringLoadingScreen;
                }
                break;
            case TransitionState.EnteringLoadingScreen:
                ratio = DrawLoadingScreenTransition(true);
                if (ratio <= 0.0f)
                {
                    GameState.TransitionState = TransitionState.SwitchingScreens;
                    GameState.Destination = Mode.Results;
                    loadStarted = false;
                }
                break;
            case TransitionState.SwitchingScreens:
                loadingLeft.anchoredPosition = new Vector2(-200, 0);
                loadingRight.anchoredPosition = new Vector2(200, 0);
                if (!loadStarted)
                {
                    if (Globals.MusicManager.IsPlaying)
                        Globals.MusicManager.Pause();
                    StartCoroutine(Util.SwitchSceneAsync());
                    loadStarted = true;
                }
                break;
            default:
                break;
        }
    }

    private float DrawLoadingScreenTransition(bool entering)
    {
        if (transitionStartTime == -1)
            transitionStartTime = Time.fixedTime;

        var duration = Time.fixedTime - transitionStartTime;
        var ratio = entering ? 1 - duration / Globals.LoadingScreenTransitionTime : duration / Globals.LoadingScreenTransitionTime;

        var xLeft = Math.Min(-200, -200 - Globals.LoadingScreenWidth * ratio);
        var xRight = Math.Max(200, 200 + Globals.LoadingScreenWidth * ratio);

        loadingLeft.anchoredPosition = new Vector2(xLeft, 0);
        loadingRight.anchoredPosition = new Vector2(xRight, 0);

        return ratio;
    }

    private void SceneChanged(Scene current, Scene next)
    {
        GameState.TransitionState = TransitionState.LeavingLoadScreen;
        transitionFinished = true;
    }

    private void SetFields()
    {
        // Set text fields as necessary
        var meta = SongSelection.Songlist[SongSelection.CurrentSongIndex];
        SongMetadata childMeta = null;
        if (meta.IsMetadataFile)
        {
            childMeta = meta.ChildMetadata.FirstOrDefault(x => (int)x.Difficulty == SongSelection.CurrentSongLevelIndex);
        }
        CanvasObj.transform.Find("TitleText").gameObject.SetText(meta.Title);
        CanvasObj.transform.Find("ArtistText").gameObject.SetText(meta.Artist);
        CanvasObj.transform.Find("DifficultyText").gameObject.SetText(
            childMeta != null ?
            ConfigFile.GetLocalizedString(childMeta.Difficulty.ToString()) :
            ConfigFile.GetLocalizedString(meta.Difficulty.ToString()));
        CanvasObj.transform.Find("DifficultyNumber").gameObject.SetText(
            childMeta != null ?
            childMeta.Level.ToString("D2") :
            meta.Level.ToString("D2"));

        CanvasObj.transform.Find("ScrollLabel").gameObject.SetText(ConfigFile.GetLocalizedString("Scroll"));
        CanvasObj.transform.Find("AccuracyLabel").gameObject.SetText(ConfigFile.GetLocalizedString("Accuracy"));
        CanvasObj.transform.Find("AutoModeLabel").gameObject.SetText(ConfigFile.GetLocalizedString("Auto_Mode_Enabled"));

        // Set accent colors
        GameObject.Find("NoteLaneAccent1Left").gameObject.SetColor(meta.ColorAccent.IfNull(ThemeColors.Purple));
        GameObject.Find("NoteLaneAccent1Right").gameObject.SetColor(meta.ColorAccent.IfNull(ThemeColors.Purple));
        GameObject.Find("NoteLaneAccent2Left").gameObject.SetColor(meta.ColorAccent.IfNull(ThemeColors.Blue));
        GameObject.Find("NoteLaneAccent2Right").gameObject.SetColor(meta.ColorAccent.IfNull(ThemeColors.Blue));
    }
}
