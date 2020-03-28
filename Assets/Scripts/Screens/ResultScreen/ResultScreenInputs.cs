using StyleStar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultScreenInputs : MonoBehaviour
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

        if (Input.GetButtonDown("Select") || Input.GetButtonDown("Left"))
        {
            GameState.TransitionState = TransitionState.EnteringLoadingScreen;
            GameState.Destination = Mode.SongSelect;
        }
    }
}
