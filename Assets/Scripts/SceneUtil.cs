using StyleStar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static partial class Util
{
    public static IEnumerator SwitchSceneAsync()
    {
        string dest = "";
        switch (GameState.Destination)
        {
            case Mode.MainMenu:
                break;
            case Mode.GameSettings:
                dest = "Scenes/GameSettingsScreen";
                break;
            case Mode.SongSelect:
                dest = "Scenes/SongSelection";
                break;
            case Mode.Loading:
                break;
            case Mode.GamePlay:
                dest = "Scenes/GameplayScreen";
                break;
            case Mode.Results:
                dest = "Scenes/ResultScreen";
                break;
            case Mode.LanguageSelect:
                dest = "Scenes/LanguageSelect";
                break;
            default:
                break;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(dest);

        while (!asyncLoad.isDone)
            yield return null;
    }
}
