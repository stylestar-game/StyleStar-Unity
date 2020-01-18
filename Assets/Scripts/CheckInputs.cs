using StyleStar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckInputs : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Global keypresses


        switch (GameState.GameMode)
        {
            case Mode.MainMenu:
                break;
            case Mode.GameSettings:
                break;
            case Mode.SongSelect:
                break;
            case Mode.Loading:
                break;
            case Mode.GamePlay:
                // This loop gathers inputs and HitResults to the corresponding notes

                if (Input.GetKeyDown(KeyCode.DownArrow))
                    GameState.ScrollSpeed = GameState.ScrollSpeed > 1.0 ? GameState.ScrollSpeed - 0.5 : 1.0;
                if (Input.GetKeyDown(KeyCode.UpArrow))
                    GameState.ScrollSpeed += 0.5;
                break;
            case Mode.Results:
                break;
            default:
                break;
        }

        

    }
}
