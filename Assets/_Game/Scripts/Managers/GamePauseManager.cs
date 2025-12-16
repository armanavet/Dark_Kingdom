using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePauseManager : MonoBehaviour
{
    [HideInInspector] public static bool IsPaused;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsPaused = !IsPaused;
            PauseGame();
        }
    }
    public void PauseGame()
    {
        if (IsPaused)
        {
            Time.timeScale = 0f;
            StateManager.Instance.ChangeGameStateTo(GameState.Paused);
            AudioListener.pause = true;
        }
        else
        {
            Time.timeScale = 1f;
            StateManager.Instance.ChangeGameStateTo(GameState.Resume);
            AudioListener.pause = false;
        }
    }
}
