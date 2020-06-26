using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StyleStar;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameSettingsScreenDisplay : MonoBehaviour
{
    public GameObject Bg1;
    public GameObject Bg2;
    public GameObject Bg3;
    public GameObject OptionBase;
    public GameObject Selection;
    public GameObject ConfirmReject;

    private RectTransform loadingLeft;
    private RectTransform loadingRight;

    private float transitionStartTime = -1;

    private bool init = false;
    private bool loadStarted = false;
    private bool transitionFinished;

    private static Vector3 selectionPoint = new Vector3(-127, 309);
    private static Vector3 selectionOffset = new Vector3(-47, -81);

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

        backgroundImage = GameObject.Find("Bg").GetComponent<UnityEngine.UI.RawImage>();
        if (backgroundImage != null && backgroundImage.texture != null)
            backgroundImage.texture.wrapMode = TextureWrapMode.Repeat;

        Bg1.SetColor(ThemeColors.Pink);
        Bg2.SetColor(ThemeColors.Blue);
        Bg3.SetColor(ThemeColors.Yellow);

        Pools.GameSettingOptions = ScriptableObject.CreateInstance<ObjectPooler>();
        Pools.GameSettingOptions.SetPool(OptionBase, 10, true, this.gameObject);

        GameSettingsScreen.Initialize(Selection, ConfirmReject);
        GameSettingsScreen.SetConfig(ConfigFile.GetTable(Defines.GameConfig));

        ConfirmReject.SetActive(false);

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

        GameSettingsScreen.Draw();
        Util.TranslateRawImage(ref backgroundImage, Globals.BackgroundOffset);

        float ratio;
        switch (GameState.TransitionState)
        {
            case TransitionState.LeavingLoadScreen:
                ratio = DrawLoadingScreenTransition(false);
                if (ratio >= 1.0f)
                {
                    GameState.TransitionState = TransitionState.ScreenActive;
                    GameState.GameMode = Mode.GameSettings;
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
}
