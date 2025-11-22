using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Background Audio Clips")]
    [SerializeField] AudioClip backgroundPassivePhaseClip;
    [SerializeField] AudioClip backgroundActivePhaseClip;
    [SerializeField] AudioClip waveStartClip;
    [Header("Menu Audio Clips")]
    [SerializeField] AudioClip menuItemsClickClip;
    [Header("Tower Audio Clips")]
    [SerializeField] AudioClip towerPanelBuyClip;
    [SerializeField] AudioClip towerUpgradeClip;
    [SerializeField] AudioClip towerPlaceClip;
    [SerializeField] AudioClip towerPuffEffectClip;
    [SerializeField] AudioClip towerPlacementDeniedClip;
    [Header("Audio Sources")]
    //[SerializeField] AudioSource towerPuffEffectSource;
    [Tooltip("Attach the Audio Source handler for...")]
    [SerializeField] AudioSource handlerForTower, 
                                 handlerForTowerPuffEffect, 
                                 handlerForUI, 
                                 handlerForMusic, 
                                 handlerForEnemy;

    //AudioSource audioSource;
    #region Singleton
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<AudioManager>();
                if (_instance == null)
                {
                    Debug.LogError("AudioManager not found in the scene!");
                }
            }
            return _instance;
        }
    }
    private void Awake()
    {
        // If another instance already exists, destroy this one
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject); // Optional: keep across scenes
    }
    #endregion


    private void Start()
    {
        //if (audioSource == null)
        //{
        //    audioSource = GetComponent<AudioSource>();
        //}
    }
    #region BackgroundMusic
    public void PlayBackgroundMusic(GameState state)
    {
        if (handlerForMusic == null || backgroundPassivePhaseClip == null || backgroundActivePhaseClip == null || waveStartClip == null)
            return;
        StartCoroutine(PlayBackgroundMusicWithDelay(state, handlerForMusic, waveStartClip, backgroundActivePhaseClip,backgroundPassivePhaseClip));
    }

    #endregion
    #region UI
    public void PlayClickSoundForUI()
    {
        UIPOS(menuItemsClickClip);
    }
    public void PlayClickSoundForPanelUI()
    {
        UIPOS(towerPanelBuyClip);
    }
    //audioSource.PlayOneShot() => [name]POS();
    void UIPOS(AudioClip audioClip)
    {
        handlerForUI.PlayOneShot(audioClip);
    }
    #endregion
    #region Tower
    public void PlayTowerActionSound(AudioClip audioClip, AudioSource audioSource)
    {
        if (audioClip == null || audioSource == null)
            return;

        audioSource.clip = audioClip;
        audioSource.PlayOneShot(audioClip);
    }
    public void PlayTowerUpgradeSound()
    {
        TowerPOS(towerUpgradeClip);
    }
    public void PlayTowerPlaceSound()
    {
        TowerPOS(towerPlaceClip);
    }
    public void PlayTowerPuffEffectSoundDelayed(float delay = 0.08f)
    {
        StartCoroutine(PlayWithDelay(handlerForTowerPuffEffect, towerPuffEffectClip, delay));
    }
    public void PlayTowerPlacementDeniedSound()
    {
        TowerPOS(towerPlacementDeniedClip);
    }

    //audioSource.PlayOneShot() => [name]POS();
    void TowerPOS(AudioClip audioClip)
    {
        handlerForTower.PlayOneShot(audioClip);
    }
    #endregion
    #region Enemy
    public void PlayExplosionSound(AudioClip audioClip)
    {
        if (audioClip == null)
            return;

        EnemyPOS(audioClip);
    }
    public void EnemyAttackSound(AudioClip audioClip, AudioSource audioSource)
    {
        if (audioClip == null || audioSource == null)
            return;

        audioSource.clip = audioClip;
        audioSource.PlayOneShot(audioClip);
    }
    public void EnemyMovingSound(AudioClip[] audioClips, AudioSource audioSource)
    {
        if (audioClips == null || audioSource == null)
            return;
        int randomSound = Random.Range(0, audioClips.Length);
        audioSource.clip = audioClips[randomSound];
        audioSource.PlayOneShot(audioClips[randomSound]);
    }
    //audioSource.PlayOneShot() => [name]POS();
    void EnemyPOS(AudioClip audioClip)
    {
        handlerForEnemy.PlayOneShot(audioClip);
    }
    #endregion

    private IEnumerator PlayWithDelay(AudioSource source, AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (clip != null && source != null)
            source.PlayOneShot(clip);
    }
    private IEnumerator PlayBackgroundMusicWithDelay(GameState state, AudioSource source, AudioClip gongClip, AudioClip musicClip1, AudioClip musicClip2)
    {
        if (state == GameState.Active)
        {
            Debug.Log("Start the gong");
            source.clip = gongClip;
            source.PlayOneShot(gongClip);
            yield return new WaitForSeconds(6);
            source.clip = musicClip1;
            source.loop = true;
            source.PlayOneShot(musicClip1);
        }
        else if (state == GameState.Passive)
        {
            //source.clip = null;
            //source.Stop();
            //source.loop = false;
            source.clip = musicClip2;
            source.loop = true;
            source.Play();
            Debug.Log("Start the passive phase music");


        }
        Debug.Log("End the Courutine");

        yield break;
    }
}
