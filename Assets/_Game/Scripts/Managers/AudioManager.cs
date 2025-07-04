using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
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
        //DontDestroyOnLoad(gameObject); // Optional: keep across scenes
    }
    #endregion


    public AudioSource audioSource;
    [Header("Volume Setting")]
    [SerializeField] AudioClip menuItemsClickSound;
    [SerializeField] AudioClip towerPanelBuyClickSound;
    //[SerializeField] AudioClip menuItemsClickSound;
    //[SerializeField] AudioClip menuItemsClickSound;
    //[SerializeField] AudioClip menuItemsClickSound;
    //[SerializeField] AudioClip menuItemsClickSound;
    //[SerializeField] AudioClip menuItemsClickSound;
    //[SerializeField] AudioClip menuItemsClickSound;
    //[SerializeField] AudioClip menuItemsClickSound;

    public void PlayClickSoundForUI()
    {
        audioSource.PlayOneShot(menuItemsClickSound);
    }
    public void PlayClickSoundForPanelUI()
    {
        audioSource.PlayOneShot(towerPanelBuyClickSound);
    }
}
