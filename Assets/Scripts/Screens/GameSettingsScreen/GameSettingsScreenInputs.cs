using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StyleStar;

public class GameSettingsScreenInputs : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Prevent inputs while the game is transitioning between screens
        if (GameState.TransitionState == TransitionState.EnteringLoadingScreen ||
            GameState.TransitionState == TransitionState.LeavingLoadScreen)
            return;

        if (Input.GetButtonDown("Down"))
            GameSettingsScreen.ScrollDown();
        else if (Input.GetButtonDown("Up"))
            GameSettingsScreen.ScrollUp();
        else if (Input.GetButtonDown("Left"))
            GameSettingsScreen.ScrollLeft();
        else if (Input.GetButtonDown("Right"))
            GameSettingsScreen.ScrollRight();
        else if (Input.GetButtonDown("Select"))
        {
            if (GameSettingsScreen.Select() != DialogResult.NoAction)
            {
                GameState.TransitionState = TransitionState.EnteringLoadingScreen;
                GameState.Destination = Mode.SongSelect;
            }
        }
    }
}
