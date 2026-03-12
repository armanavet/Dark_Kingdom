using AudioSystem;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] SoundData music;
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
        music = AudioManager.Instance.SetData(SoundDataType.Music);
        UIManager.Instance.Initialize();
        PortalManager.Instance.Initialize();
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
    public void PlayMusic(GameState state)
    {
        if (music.clip != null)
        {
            music.clip = null;
            StopAllCoroutines();
        }
        StartCoroutine(ChooseTheMusic(state));
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
    IEnumerator ChooseTheMusic(GameState state)
    {
        if (state == GameState.Passive)
        {
            AudioManager.Instance.Play(MusicType.InNormal, music, transform);
        }
        else if (state == GameState.Active)
        {
            AudioManager.Instance.Play(GamePlaySFX_Type.WaveStart, music, transform);
            yield return new WaitForSeconds(music.clip.length);
            AudioManager.Instance.Play(MusicType.InWave, music, transform);
        }
    }
}
