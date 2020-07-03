using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StyleStar;
using System.IO;

public class SongSelectInputs : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        // Need to load language selection if the game is being loaded for the first time
        if (!File.Exists(Defines.ConfigFile))
        {
            GameState.TransitionState = TransitionState.EnteringLoadingScreen;
            GameState.Destination = Mode.LanguageSelect;
            StartCoroutine(Util.SwitchSceneAsync());
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Prevent inputs while the game is transitioning between screens or cards
        if (GameState.TransitionState == TransitionState.EnteringLoadingScreen ||
            GameState.TransitionState == TransitionState.LeavingLoadScreen)
            return;

        // Prevent inputs while the menus are animating
        if (SongSelectionMovement.IsAnimating())
        {
            SongSelectionMovement.IncrementFrame(); // increment by one frame
            return;
        }

        if (Input.GetButtonDown("Down"))
            SongSelection.ScrollDown();
        else if (Input.GetButtonDown("Up"))
            SongSelection.ScrollUp();
        else if (Input.GetButtonDown("Left"))
            SongSelection.GoBack();
        else if (Input.GetButtonDown("Right"))
            SongSelection.CycleDifficulty();
        else if (Input.GetButtonDown("Auto"))
        {
            switch (GameState.CurrentSettings.AutoSetting)
            {
                case Settings.AutoMode.Off:
                    GameState.CurrentSettings.AutoSetting = Settings.AutoMode.Auto;
                    break;
                case Settings.AutoMode.Auto:
                    GameState.CurrentSettings.AutoSetting = Settings.AutoMode.Off;
                    break;
                case Settings.AutoMode.AutoDown:
                    break;
                default:
                    break;
            }
        }
        else if (Input.GetButtonDown("Select"))
        {
            if (SongSelection.Select())
            {
                GameState.TransitionState = TransitionState.EnteringLoadingScreen;
                GameState.Destination = Mode.GamePlay;
            }
        }
        else if (Input.GetButtonDown("GameSettings"))
        {
            GameState.TransitionState = TransitionState.EnteringLoadingScreen;
            GameState.Destination = Mode.GameSettings;
        }
    }
}
