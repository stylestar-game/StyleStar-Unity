using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StyleStar;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class SongSelectDisplay : MonoBehaviour
{
    public GameObject BgLine;

    private RectTransform loadingLeft;
    private RectTransform loadingRight;

    private float transitionStartTime = -1;

    private List<CardBase> songCards;
    private List<CardBase> folderCards;
    private List<CardBase> levelCards;

    private bool init = false;
    private bool loadStarted = false;
    private bool transitionFinished;
    private int currentSongIndex = -1;

    private UnityEngine.UI.RawImage backgroundImage = null;

    private void Awake()
    {
        transitionFinished = false;
        SceneManager.activeSceneChanged += SceneChanged;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!ConfigFile.IsLoaded)
            ConfigFile.Load();

        loadingLeft = GameObject.Find("LoadingLeft").GetComponent<RectTransform>();
        loadingRight = GameObject.Find("LoadingRight").GetComponent<RectTransform>();

        BgLine.SetColor(ThemeColors.Blue);

        songCards = new List<CardBase>();
        if(SongSelection.Songlist.Count == 0)
            SongSelection.ImportSongs(Defines.SongFolder);
        for (int i = 0; i < SongSelection.Songlist.Count; i++)
        {
            songCards.Add(new SongCard(Pools.SongCards.GetPooledObject(), SongSelection.Songlist[i]));
            songCards[i].Shift(i, SongSelection.CurrentSongIndex);
        }
        levelCards = new List<CardBase>();
        for (int i = 0; i < 10; i++)
            levelCards.Add(new FolderCard(Pools.FolderCards.GetPooledObject(), "LEVEL " + (i + 1)));
        folderCards = new List<CardBase>();
        foreach (var folder in SongSelection.FolderParams)
        {
            switch (folder.Type)
            {
                case SortType.Title: folder.Name = ConfigFile.GetLocalizedString("Sort_Title"); break;
                case SortType.Artist: folder.Name = ConfigFile.GetLocalizedString("Sort_Artist"); break;
                case SortType.Level: folder.Name = ConfigFile.GetLocalizedString("Sort_Level"); break;
                default: break;
            }
            folderCards.Add(new FolderCard(Pools.FolderCards.GetPooledObject(), folder.Name));
        }

        transform.Find("AutoModeText").gameObject.SetText(ConfigFile.GetLocalizedString("Auto_Mode_Enabled"));

        // No Songs
        if (SongSelection.Songlist.Count == 0)
        {
            GameState.GameMode = Mode.NoSongs;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            transform.Find("Bg").gameObject.SetActive(true);
            transform.Find("NoSongs").gameObject.SetActive(true);
        }
        else
        {
            backgroundImage = GameObject.Find("BgLine").GetComponent<UnityEngine.UI.RawImage>();
            if (backgroundImage != null && backgroundImage.texture != null)
                backgroundImage.texture.wrapMode = TextureWrapMode.Repeat;
        }

        init = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!init)
            return;

        // Wait until the scene is fully loaded
        if (!transitionFinished)
            return;

        // Always draw cards (this has to happen before the loading screen slides open)
        if (SongSelection.Songlist.Count > 0)
        {
            Util.TranslateRawImage(ref backgroundImage, Globals.BackgroundOffset);
            if (SongSelection.SelectedFolderIndex == -1)
            {
                folderCards.SetActive(true);
                levelCards.SetActive(false);
                songCards.SetActive(false);
                UpdateMetadata(false);
                UpdateLoopedSong(true);

                for (int i = 0; i < folderCards.Count; i++)
                    folderCards[i].Shift(i, SongSelection.CurrentFolderIndex, SongSelectionMovement.IsAnimating(), SongSelectionMovement.AnimationMovement.x, SongSelectionMovement.AnimationMovement.y);
            }
            else
            {
                if (SongSelection.FolderParams[SongSelection.SelectedFolderIndex].Type == SortType.Level && SongSelection.SelectedLevelIndex == -1)
                {
                    folderCards.SetActive(false);
                    levelCards.SetActive(true);
                    songCards.SetActive(false);
                    UpdateMetadata(false);
                    UpdateLoopedSong(true);

                    for (int i = 0; i < levelCards.Count; i++)
                        levelCards[i].Shift(i, SongSelection.CurrentLevelIndex, SongSelectionMovement.IsAnimating(), SongSelectionMovement.AnimationMovement.x, SongSelectionMovement.AnimationMovement.y);
                }
                else
                {
                    folderCards.SetActive(false);
                    levelCards.SetActive(false);
                    songCards.SetActive(true);
                    UpdateMetadata(true);
                    UpdateLoopedSong(false);

                    for (int i = 0; i < SongSelection.Songlist.Count; i++)
                    {
                        var card = songCards.FirstOrDefault(x => ((SongCard)x).SongID == SongSelection.Songlist[i].SongID);

                        card.Shift(i, SongSelection.CurrentSongIndex, SongSelectionMovement.IsAnimating(), SongSelectionMovement.AnimationMovement.x, SongSelectionMovement.AnimationMovement.y);
                        int diff = i == SongSelection.CurrentSongIndex ? SongSelection.CurrentSongLevelIndex : -1;
                        ((SongCard)card).SetDifficulty(diff);
                    }
                }
            }
        }

        this.transform.Find("AutoModeText").gameObject.SetActive(GameState.CurrentSettings.AutoSetting != Settings.AutoMode.Off);

        float ratio;
        switch (GameState.TransitionState)
        {
            case TransitionState.LeavingLoadScreen:
                ratio = DrawLoadingScreenTransition(false);
                if (ratio >= 1.0f)
                {
                    GameState.TransitionState = TransitionState.ScreenActive;
                    GameState.GameMode = Mode.SongSelect;
                }
                break;
            case TransitionState.ScreenActive:
                transitionStartTime = -1;
                break;
            case TransitionState.EnteringLoadingScreen:
                ratio = DrawLoadingScreenTransition(true);
                if (ratio <= 0.0f)
                {
                    GameState.TransitionState = TransitionState.SwitchingScreens;
                    loadStarted = false;
                }
                break;
            case TransitionState.SwitchingScreens:
                if (!loadStarted)
                {
                    Globals.CurrentNoteCollection = new NoteCollection(SongSelection.GetSelectedMetadata());
                    Globals.CurrentSongMetadata = Globals.CurrentNoteCollection.ParseFile();

                    // If music is currently looping, cease it now.
                    Globals.MusicManager.EndSongLoop(this);

                    Globals.MusicManager.LoadSong(Globals.CurrentSongMetadata.FilePath + Globals.CurrentSongMetadata.SongFilename, Globals.CurrentSongMetadata.BpmEvents);
                    Globals.MusicManager.Offset = Globals.CurrentSongMetadata.PlaybackOffset * 1000;

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
        {
            loadingRight.SetAsLastSibling();
            loadingLeft.SetAsLastSibling();
            transitionStartTime = Time.fixedTime;
        }

        var duration = Time.fixedTime - transitionStartTime;
        var ratio = entering ? 1 - duration / Globals.LoadingScreenTransitionTime : duration / Globals.LoadingScreenTransitionTime;

        var xLeft = Math.Min(-200, -200 - Globals.LoadingScreenWidth * ratio);
        var xRight = Math.Max(200, 200 + Globals.LoadingScreenWidth * ratio);

        loadingRight.anchoredPosition = new Vector2(xRight, 0);        
        loadingLeft.anchoredPosition = new Vector2(xLeft, 0);

        return ratio;
    }

    private void SceneChanged(Scene current, Scene next)
    {
        GameState.TransitionState = TransitionState.LeavingLoadScreen;
        transitionFinished = true;
    }

    private void UpdateMetadata(bool active)
    {
        this.transform.Find("TitleText").gameObject.SetActive(active);
        this.transform.Find("ArtistText").gameObject.SetActive(active);
        this.transform.Find("BPMText").gameObject.SetActive(active);
        this.transform.Find("ChoreoText").gameObject.SetActive(active);

        if (!active)
            return;

        var currentMeta = SongSelection.Songlist[SongSelection.CurrentSongIndex];

        string bpm = "???";
        if (currentMeta.BpmIndex.Count > 0)
            bpm = currentMeta.BpmIndex.First().Value.ToString("F0");
        else if (currentMeta.IsMetadataFile && currentMeta.BpmIndex.Count == 0 && currentMeta.ChildMetadata.Count > 0)
            bpm = currentMeta.ChildMetadata.First().BpmIndex.First().Value.ToString("F0");

        this.transform.Find("TitleText").gameObject.SetText(currentMeta.Title);
        this.transform.Find("ArtistText").gameObject.SetText(currentMeta.Artist);
        this.transform.Find("BPMText").gameObject.SetText(bpm + " BPM");
        this.transform.Find("ChoreoText").gameObject.SetText(ConfigFile.GetLocalizedString("Choreo") + ": " + currentMeta.GetPropertyFromChild("Designer", SongSelection.CurrentSongLevelIndex));
    }

    private void UpdateLoopedSong(bool endSong)
    {
        if (endSong)
        {
            Globals.MusicManager.EndSongLoop(this);
            currentSongIndex = -1;
        }
        else if (!endSong && currentSongIndex != SongSelection.CurrentSongIndex)
        {
            Globals.MusicManager.EndSongLoop(this);
            var filename = SongSelection.Songlist[SongSelection.CurrentSongIndex].FilePath + SongSelection.Songlist[SongSelection.CurrentSongIndex].SongFilename;
            var offset = SongSelection.Songlist[SongSelection.CurrentSongIndex].PlaybackOffset;
            Globals.MusicManager.BeginSongLoop(filename, (long)offset, this);
            currentSongIndex = SongSelection.CurrentSongIndex;
        }
    }
}
