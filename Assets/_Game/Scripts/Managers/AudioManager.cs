using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    
    
    AudioSource audioSource;
    [Header("Audio Clips")]
    [SerializeField] AudioClip menuItemsClickSound;
    [SerializeField] AudioClip towerPanelBuyClickSound;
    [SerializeField] AudioClip towerUpgradeClip;
    [SerializeField] AudioClip towerPlaceClip;
    [SerializeField] AudioClip towerPuffEffectClip;
    [SerializeField] AudioClip towerPlacementDeniedClip;
    [Header("Audio Sources")]
    [SerializeField] AudioSource towerPuffEffectSource;
    
    #region Singleton
    private static AudioManager _instance;
    public static AudioManager instance
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
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }
    public void PlayClickSoundForUI()
    {
        PlayOneShot(menuItemsClickSound);
    }
    public void PlayClickSoundForPanelUI()
    {
        PlayOneShot(towerPanelBuyClickSound);
    }

    public void PlayExplosionSound(AudioClip audioClip)
    {
        if (audioClip == null)
            return;

        PlayOneShot(audioClip);
    }

    public void PlayTowerActionSound(AudioClip audioClip, AudioSource audioSource)
    {
        if (audioClip == null || audioSource == null)
            return;

        audioSource.clip = audioClip;
        PlayOneShot(audioClip);
    }
    public void EnemyAttackSound(AudioClip audioClip, AudioSource audioSource)
    {
        if (audioClip == null || audioSource == null)
            return;

        audioSource.clip = audioClip;
        PlayOneShot(audioClip);
    }
    public void EnemyMovingSound(AudioClip[] audioClips, AudioSource audioSource)
    {
        if (audioClips == null || audioSource == null)
            return;

        int randomSound = Random.Range(0, audioClips.Length);
        audioSource.clip = audioClips[randomSound];
        PlayOneShot(audioClips[randomSound]);
    }
    public void PlayTowerUpgradeSound()
    {
        PlayOneShot(towerUpgradeClip);
    }
    public void PlayTowerPlaceSound()
    {
        PlayOneShot(towerPlaceClip);
    }
    public void PlayTowerPuffEffectSoundDelayed(float delay = 0.08f)
    {
        StartCoroutine(PlayWithDelay(towerPuffEffectSource, towerPuffEffectClip, delay));
    }
    public void PlayTowerPlacementDeniedSound()
    {
        PlayOneShot(towerPlacementDeniedClip);
    }
    
    void PlayOneShot(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }
    private IEnumerator PlayWithDelay(AudioSource source, AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (clip != null && source != null)
            source.PlayOneShot(clip);
    }
}
