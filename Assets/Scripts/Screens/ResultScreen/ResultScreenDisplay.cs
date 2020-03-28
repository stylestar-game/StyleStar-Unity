using StyleStar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultScreenDisplay : MonoBehaviour
{
    private RectTransform loadingLeft;
    private RectTransform loadingRight;

    private float transitionStartTime = -1;
    private bool loadStarted = false;
    private bool transitionFinished;

    private void Awake()
    {
        transitionFinished = false;
        SceneManager.activeSceneChanged += SceneChanged;
    }

    // Start is called before the first frame update
    void Start()
    {
        loadingLeft = GameObject.Find("LoadingLeft").GetComponent<RectTransform>();
        loadingRight = GameObject.Find("LoadingRight").GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        // Wait until the scene is fully loaded
        if (!transitionFinished)
            return;

        float ratio;
        switch (GameState.TransitionState)
        {
            case TransitionState.LeavingLoadScreen:
                SetFields();
                ratio = DrawLoadingScreenTransition(false);
                if (ratio >= 1.0f)
                    GameState.TransitionState = TransitionState.ScreenActive;
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
                loadingLeft.anchoredPosition = new Vector2(-200, 0);
                loadingRight.anchoredPosition = new Vector2(200, 0);
                if (!loadStarted)
                {
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
        this.transform.Find("TitleText").gameObject.SetText(meta.Title);
        this.transform.Find("ArtistText").gameObject.SetText(meta.Artist);
        this.transform.Find("DifficultyText").gameObject.SetText(
            childMeta != null ?
            childMeta.Difficulty.ToString() + " Lv." + childMeta.Level.ToString("D2") :
            meta.Difficulty.ToString() + " Lv." + meta.Level.ToString("D2"));

        var noteCollection = Globals.CurrentNoteCollection;
        var score = noteCollection.CurrentScore / noteCollection.TotalNotes * 100.00f;

        // Set Stars / score
        var starID = GetStarID((float)score);
        for (int i = 0; i < 8; i++)
        {
            this.transform.Find("Star" + i).gameObject.SetActive(i == starID);
        }
        this.transform.Find("Score").gameObject.SetText(score.ToString("F3") + "%");

        // Set result string
        switch (noteCollection.SongEnd)
        {
            case SongEndReason.Cleared:
                this.transform.Find("Grading").gameObject.SetText("Cleared");
                break;
            case SongEndReason.Failed:
                this.transform.Find("Grading").gameObject.SetText("Failed");
                break;
            default:
                this.transform.Find("Grading").gameObject.SetText("Forfeit");
                break;
        }

        // Set notecounts
        var grading = this.transform.Find("Grading").gameObject;
        grading.transform.Find("StylishCount").gameObject.SetText(noteCollection.PerfectCount.ToString("D4"));
        grading.transform.Find("CoolCount").gameObject.SetText(noteCollection.GreatCount.ToString("D4"));
        grading.transform.Find("GoodCount").gameObject.SetText(noteCollection.GoodCount.ToString("D4"));
        grading.transform.Find("MissCount").gameObject.SetText(noteCollection.MissCount.ToString("D4"));

        // Set line colors
        this.transform.Find("BgLine3").gameObject.SetColor(meta.ColorAccent.IfNull(ThemeColors.BrightGreen));
        this.transform.Find("BgLine2").gameObject.SetColor(meta.ColorAccent.IfNull(ThemeColors.Purple));
        this.transform.Find("BgLine1").gameObject.SetColor(meta.ColorAccent.IfNull(ThemeColors.Pink));
    }

    private int GetStarID(float score)
    {
        if (score >= 98.0f)
            return 7;
        else if (score >= 95.0f)
            return 6;
        else if (score >= 90.0f)
            return 5;
        else if (score >= 80.0f)
            return 4;
        else if (score >= 60.0f)
            return 3;
        else if (score >= 30.0f)
            return 2;
        else if (score >= 10.0f)
            return 1;
        else
            return 0;
    }
}
