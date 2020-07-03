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
                GameState.TransitionState = TransitionState.EnteringLoadingScreen;
                GameState.Destination = LanguageSelect.NextGameMode;
            }
        }
    }
}
