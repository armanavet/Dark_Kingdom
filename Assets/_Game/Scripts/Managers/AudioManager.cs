using AudioSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

//public interface ISoundKey { }
public class AudioManager : MonoBehaviour
{
    [SerializeField] SoundBank soundBank;
    [SerializeField] SourceBank sourceBank;
    #region Singleton
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<AudioManager>();
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
    public void Initialize()
    {
        soundBank.Build();
        sourceBank.Build();
    }
    public SoundData SetData<T>(T sourceType, SoundDataType soundDataType)
        where T : Enum
    {
        return sourceBank.GetData(sourceType, soundDataType);
    }
    public SoundData SetData(SoundDataType soundDataType)
    {
        return sourceBank.GetData(soundDataType);
    }
    public void Play<T>(T soundType, SoundData data, Transform target = null)
        where T : Enum
    {
        if (data == null || soundBank == null)
        {
            Debug.LogError("Sound system is not configured correctly.");
            return;
        }

        data.clip = soundBank.GetClip(soundType);

        Vector3 position = target != null ? target.position : transform.position;

        SoundManager.Instance
            .CreateSoundBuilder()
            .WithPosition(position)
            .WithRandomPitch()
            .Play(data);
    }
    public void Play<T_Sound, T_Source>(T_Source sourcType, T_Sound soundType, SoundData data, Transform target)
        where T_Sound : Enum
            where T_Source : Enum
    {
        if (data == null || soundBank == null)
        {
            Debug.LogError("Sound system is not configured correctly.");
            return;
        }

        data.clip = soundBank.GetClip(sourcType, soundType);

        Vector3 position = target != null ? target.position : transform.position;

        SoundManager.Instance
            .CreateSoundBuilder()
            .WithPosition(transform.position)
            .WithRandomPitch()
            .Play(data);
    }
    public void Stop()
    {
        SoundManager.Instance.StopFrequent();
    }
}
public enum SoundDataType
{
    Null,
    Music,
    UI,
    Gameplay
}
public enum MixerType
{
    Null,
    UI,
    Music,
    Ambient
}
public enum SoundType
{
    Null,
    Music,
    UISFX,
    GameplaySFX,
    EnemySFX,
    TowerSFX,
    Ambient
}
public enum MusicType
{
    Null,
    InWave,
    InNormal
}
public enum UISFX_Type
{
    Null,
    MouseLeftButton,  //Click on the UI element in the Menu 
    TowerBuyButton,
    TowerSellButton,
    TowerUpgradeButton,
    PlacementDenied   // Plays when player try to build tower in the forbidden place.
}
public enum GamePlaySFX_Type
{
    Null,
    TowerUpgradeVFX,
    TowerPuffVFX,
    TowerPlace,
    TowerShoot,
    ProjectileHit,
    EnemyAttack,
    EnemyMove,
    WaveStart,
    PortalGate,
    PortalOrb,
    PortalTopFire,
    PortalScreaming
}
//public enum AmbientType
//{
//    Null,
//    //--Portal--
//    OrbParticalL,
//    OrbParticalR,
//    FireOnTop,
//    //----------
//}
