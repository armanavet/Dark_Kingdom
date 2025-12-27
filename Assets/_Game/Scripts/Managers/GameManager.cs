using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static bool IsPaused;
    #region Singleton
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    #endregion

    void Start()
    {
        UIManager.Instance.Initialize();
        TowerManager.Instance.Initialize();
        WaveManager.Instance.Initialize();
        StateManager.Instance.Initialize();
    }
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
