using AudioSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioMixerGroup masterMixerGroup;
    [SerializeField] GameSFX gameSFX;

    [SerializeField] SoundData enemySFXSourceSettings;
    [SerializeField] SoundData towerSFXSourceSettings;

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

        //DontDestroyOnLoad(gameObject); // Optional: keep across scenes
    }
    #endregion
    private void Start()
    {
        gameSFX.OrganizeByType();
    }



    /*
     * ------------ TODO ----------------------------------------------------------------------------
     * get data and set it for each keeper
     * get data and set it for each clip if it's an object with the clip arrays
     * have a functions that set the certain clip to the sound data depend on the key typs
     * have functions to play the clips 
     * 
     */
    void SetData(SoundData fromData, SoundData toData)
    {
        toData = fromData;
    }
    public SoundData GetData(SoundData data, SFXGroupType sfxType, UnitType unitType = 0, TowerType towerType = 0)
    {
        if (data == null)
        {
            Debug.LogError("resieved data is null!");
            return null;
        }

        switch (sfxType)
        {
            case SFXGroupType.Enemy:
                SetData(enemySFXSourceSettings, data);
                data.mixerGroup = gameSFX.EnemySFXData(unitType).mixerGroup;
                break;
            case SFXGroupType.Tower:
                SetData(enemySFXSourceSettings, data);
                data.mixerGroup = gameSFX.TowerSFXData(towerType).mixerGroup;
                break;
            default: break;
        }

        return data;
    }
    public SoundData SetClip(SoundData data, ClipType clipType, UnitType unitType = 0, TowerType towerType = 0)
    {
        //var enemySFX = gameSFX.EnemySFXData(type);
        //switch (parametor)
        //{
        //    case SoundDataParametor.ClipAttack:
        //        data.clip = enemySFX.attack;
        //        break;
        //    case SoundDataParametor.ClipMove:
        //        int randomClip = Random.Range(0, enemySFX.move.Length);
        //        data.clip = enemySFX.move[randomClip];
        //        break;
        //    case SoundDataParametor.MixerGroup:
        //        data.mixerGroup = enemySFX.enemyMixerGroup;
        //        break;
        //    default: break;
        //}

        return data;
    }
    //public void PlayEnemySFX(SoundDataParametor parametor, UnitType type, SoundData data, Transform transform)
    //{
    //    //data = SetParametor(parametor, type, data);
    //    //switch (parametor)
    //    //{
    //    //    case SoundDataParametor.ClipAttack:
    //    //        SoundManager.Instance.CreateSoundBuilder().WithPosition(transform.position).Play(data);
    //    //        break;
    //    //    case SoundDataParametor.ClipMove:
    //    //        SoundManager.Instance.CreateSoundBuilder().WithRandomPitch().WithPosition(transform.position).Play(data);
    //    //        break;
    //    //    default: break;
    //    //}
    //}




}
[Serializable]
public class EnemySFX
{
    public UnitType type;
    public AudioClip attack;
    public AudioClip[] move;
    public AudioMixerGroup mixerGroup;
}
[Serializable]
public class TowerSFX
{
    public TowerType type;
    public AudioClip place;
    public AudioClip denied;
    public AudioClip shoot;
    public AudioClip hit;
    public AudioMixerGroup mixerGroup;
}
[Serializable]
public class GameSFX
{
    public EnemySFX[] enemySFX;
    public TowerSFX[] towerSFX;
    Dictionary<UnitType, EnemySFX> enemySFXLookUp = new Dictionary<UnitType, EnemySFX>();
    Dictionary<TowerType, TowerSFX> towerSFXLookUp = new Dictionary<TowerType, TowerSFX>();

    public void OrganizeByType()
    {
        SortDictionary(enemySFXLookUp, enemySFX, enemyType => enemyType.type);
        SortDictionary(towerSFXLookUp, towerSFX, towerType => towerType.type);
    }
    private void SortDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict, TValue[] items, Func<TValue, TKey> keySelector)
    {
        dict.Clear();

        foreach (var item in items)
        {
            if (item == null) continue;
            TKey key = keySelector(item);
            dict[key] = item;
        }
    }

    private TValue Lookup<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key)
    {
        if (!dict.TryGetValue(key, out var value))
        {
            Debug.LogError($"Dictionary does NOT contain key: {key}");
            return default;
        }
        return value;
    }
    public EnemySFX EnemySFXData(UnitType type) => Lookup(enemySFXLookUp, type);
    public TowerSFX TowerSFXData(TowerType type) => Lookup(towerSFXLookUp, type);
    
}

public enum ClipType
{
    ClipAttack,
    ClipMove,
    ClipShoot,
    ClipHit,
    ClipPlace,
    ClipDenied
}
public enum SFXGroupType
{
    Enemy,
    Tower
}


/*private void Start()
    {
        
    }
    #region BackgroundMusic
    public void PlayBackgroundMusic(GameState state)
    {
        //if (handlerForMusic == null || waveStartClip == null || BackgroundMusic == null)
        //    return;
        //StartCoroutine(PlayBackgroundMusicWithDelay(state, handlerForMusic, waveStartClip, BackgroundMusic));
    }

    #endregion
    #region UI
    public void PlayClickSoundForUI()
    {
        //UIPOS(menuItemsClickClip);
    }
    public void PlayClickSoundForPanelUI()
    {
        //UIPOS(towerPanelBuyClip);
    }
    //audioSource.PlayOneShot() => [name]POS();
    void UIPOS(AudioClip audioClip)
    {
        //handlerForUI.PlayOneShot(audioClip);
    }
    #endregion

    #region Tower
    public void PlayTowerActionSound(AudioClip audioClip, AudioSource audioSource)
    {
        //if (audioClip == null || audioSource == null)
        //    return;

        //audioSource.clip = audioClip;
        //audioSource.PlayOneShot(audioClip);
    }
    public void PlayTowerUpgradeSound()
    {
        //TowerPOS(towerUpgradeClip);
    }
    public void PlayTowerPlaceSound()
    {
        //TowerPOS(towerPlaceClip);
    }
    public void PlayTowerPuffEffectSoundDelayed(float delay = 0.08f)
    {
        //StartCoroutine(PlayWithDelay(handlerForTowerPuffEffect, towerPuffEffectClip, delay));
    }
    public void PlayTowerPlacementDeniedSound()
    {
        //TowerPOS(towerPlacementDeniedClip);
    }

    //audioSource.PlayOneShot() => [name]POS();
    void TowerPOS(AudioClip audioClip)
    {
        //handlerForTower.PlayOneShot(audioClip);
    }
    #endregion

    #region Enemy
    public void PlayExplosionSound(AudioClip audioClip, Transform spawnTransform)
    {
        //if (audioClip == null)
        //    return;
        //AudioSource source = Instantiate(handlerForEnemy, spawnTransform.position, Quaternion.identity);
        //source.clip = audioClip;
        //source.Play();
        //float clipLength = source.clip.length;
        //Destroy(source.gameObject, clipLength);
    }
    public void EnemyAttackSound(AudioClip audioClip, AudioSource audioSource)
    {
        //if (audioClip == null || audioSource == null)
        //    return;

        //audioSource.clip = audioClip;
        //audioSource.PlayOneShot(audioSource.clip);
    }
    public void EnemyMovingSound(AudioClip[] audioClips, AudioSource audioSource)
    {
        //if (audioClips == null || audioSource == null)
        //    return;
        //int randomSound = Random.Range(0, audioClips.Length);
        //audioSource.clip = audioClips[randomSound];
        //audioSource.PlayOneShot(audioSource.clip);
    }
    #endregion

    private IEnumerator PlayWithDelay(AudioSource source, AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (clip != null && source != null)
            source.PlayOneShot(clip);
    }
    private IEnumerator PlayBackgroundMusicWithDelay(GameState state, AudioSource source, AudioClip gongClip)
    {
        //float t = 0;
        //int rand = 0;
        //AudioClip[] audio;
        //if (state == music[0].state)
        //{
        //    audio = music[0].audio;
        //    rand = Random.Range(0, audio.Length);
        //    source.clip = gongClip;
        //    source.PlayOneShot(gongClip);
        //    source.clip = audio[rand];
        //    source.loop = true;
        //    source.PlayDelayed(gongClip.length);
        //}
        //else if (state == music[1].state)
        //{
        //    audio = music[1].audio;
        //    rand = Random.Range(0, audio.Length);
        //    source.clip = audio[rand];
        //    source.loop = true;
        //    source.Play();
        //    while (t < 0.15f)
        //    {
        //        //source.PlayScheduled(source.volume);
        //        source.volume = t += Time.deltaTime * 0.01f;
        //        yield return null;
        //    }
        //}

        yield break;
    }*/
