using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StyleStar;

public class LanguageSelectInputs : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Down"))
            LanguageSelect.ScrollDown();
        else if (Input.GetButtonDown("Up"))
            LanguageSelect.ScrollUp();
        else if (Input.GetButtonDown("Left"))
            LanguageSelect.Cancel();
        else if (Input.GetButtonDown("Select"))
        {
            DialogResult outRes = LanguageSelect.Select();
            if (outRes == DialogResult.Confirm)
            {
                ConfigFile.LocalizedLanguage = LanguageSelect.GetLanguage();
                // if it's the first time opening the game, recreate ConfigFile to capture Language state
                if (LanguageSelect.NextGameMode == Mode.SongSelect)
                    ConfigFile.Recreate();
                GameState.TransitionState = TransitionState.EnteringLoadingScreen;
                GameState.Destination = LanguageSelect.NextGameMode;
            }
        }
    }
}
