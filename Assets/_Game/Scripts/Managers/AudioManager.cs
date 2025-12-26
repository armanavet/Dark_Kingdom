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
    //[SerializeField] ClipsRepository TotalClips;
    [SerializeField] SoundDataRepository TotalSoundData;
    //[SerializeField] AudioClipCollector Collector;
    [SerializeField] SoundBank soundBank;
    [SerializeField] AudioSource BackgroundMusicSource;

    private SoundTypeResolver resolver = new SoundTypeResolver();
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

        //DontDestroyOnLoad(gameObject); // Optional: keep across scenes
    }
    #endregion
    private void Start()
    {
        //gameEnums = gameObject.GetComponent<SoundTypeResolver>();
        //TotalClips.Organize();
        TotalSoundData.Organize();
        func(GamePlaySFX_Type.TowerPuffVFX);
    }
    void func(Enum type)
    {
        SoundType a = resolver.Resolve(type);
        Debug.Log(a);
    }

    public SoundData SetData<TEnum>(
        SoundData data,
        SoundDataType soundDataType,
        //MixerType mixer,
        TEnum type) where TEnum : Enum
    {
        data = TotalSoundData.GetSoundDataByType(soundDataType);
        //data.mixerGroup = TotalClips.GetMixer(mixer, type);
        return data;
    }
    //public SoundData SetData(
    //    SoundData data,
    //    SoundDataType soundDataType,
    //    MixerType mixer)
    //{
    //    data = TotalSoundData.GetSoundDataByType(soundDataType);
    //    data.mixerGroup = TotalClips.GetMixer(mixer);
    //    if (data.mixerGroup == null)
    //    {
    //        Debug.LogError($"Mixer is null. Sound Data Type - {soundDataType}");
    //    }
    //    return data;
    //}
    public void Play<TSound, TSource>(SoundData data, Transform transform, TSound type, TSource stype)
    where TSound : Enum
        where TSource : Enum
    {
        //data.clip = soundBank.GetClip(resolver.Resolve(type), SoundRequest.SoundWithSource(type,stype));
        data.clip = soundBank.GetClip(stype, type);
        SoundManager.Instance.CreateSoundBuilder().WithPosition(transform.position).WithRandomPitch().Play(data);
    }
    //public void Play(SoundData data, Transform transform, ClipType clipType)
    //{
    //    data.clip = TotalClips.GetClip(clipType);
    //    SoundManager.Instance.CreateSoundBuilder().WithPosition(transform.position).WithRandomPitch().Play(data);
    //}
    //public void Play(SoundData data, ClipType clipType)
    //{
    //    data.clip = TotalClips.GetClip(clipType);
    //    SoundManager.Instance.CreateSoundBuilder().Play(data);
    //}
    //public void Play(SoundData data, GameState gameState)
    //{
    //    SoundManager soundManager = SoundManager.Instance;
    //    if (data.clip != null)
    //    {
    //        StopCoroutine(PlayWithDelay(data, gameState, soundManager));
    //    }
    //    StartCoroutine(PlayWithDelay(data, gameState, soundManager));
    //}
    //IEnumerator PlayWithDelay(SoundData data, GameState gameState, SoundManager soundManager)
    //{
    //    if (gameState == GameState.Active)
    //    {
    //        data.clip = TotalClips.GetClip(ClipType.OnWaveStart_State);
    //        soundManager.CreateSoundBuilder().Play(data);
    //        yield return new WaitForSeconds(data.clip.length);
    //        data.clip = null;
    //        data.clip = TotalClips.GetClip(ClipType.OnActive_State);
    //        soundManager.CreateSoundBuilder().Play(data);
    //    }
    //    else if (gameState == GameState.Passive)
    //    {
    //        data.clip = null;
    //        data.clip = TotalClips.GetClip(ClipType.OnPassive_State);
    //        soundManager.CreateSoundBuilder().Play(data);
    //    }
    //    yield break;
    //}
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
    TowerProjectileHit,
    EnemyAttack,
    EnemyMove,
    WaveStart,
    PortalGate
}
public enum AmbientType
{
    Null,
    //--Portal--
    OrbParticalL,
    OrbParticalR,
    FireOnTop,
    //----------
}
