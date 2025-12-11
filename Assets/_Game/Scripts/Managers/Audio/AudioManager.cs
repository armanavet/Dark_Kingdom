using AudioSystem;
using System;
using System.Threading.Tasks;
using UnityEngine;

//public interface ISoundKey { }
public class AudioManager : MonoBehaviour
{
    [SerializeField] Clips TotalClips;
    [SerializeField] SoundDataRepository TotalSoundData;

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
        TotalClips.OrganizeAll();
        TotalSoundData.Organize();
    }
    public SoundData SetData<TEnum>(
        SoundData data,
        SoundDataType soundDataType,
        MixerType mixer,
        TEnum type) where TEnum : Enum
    {
        data = TotalSoundData.GetSoundDataByType(soundDataType);
        data.mixerGroup = TotalClips.GetMixer<TEnum>(mixer, type);
        return data;
    }
    public SoundData SetData(
        SoundData data,
        SoundDataType soundDataType,
        MixerType mixer)
    {
        data = TotalSoundData.GetSoundDataByType(soundDataType);
        data.mixerGroup = TotalClips.GetMixer(mixer);
        return data;
    }
    public void PlaySFX<TEnum>(SoundData data, Transform transform, ClipType clipType, TEnum type)
    where TEnum : Enum
    {
        data.clip = TotalClips.GetClip<TEnum>(clipType, type);
        SoundManager.Instance.CreateSoundBuilder().WithPosition(transform.position).WithRandomPitch().Play(data);
    }
    public void PlaySFX(SoundData data, Transform transform, ClipType clipType)
    {
        data.clip = TotalClips.GetClip(clipType);
        SoundManager.Instance.CreateSoundBuilder().WithPosition(transform.position).WithRandomPitch().Play(data);
    }
    public void PlaySFX(SoundData data, ClipType clipType)
    {
        data.clip = TotalClips.GetClip(clipType);
        SoundManager.Instance.CreateSoundBuilder().WithRandomPitch().Play(data);
    }

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
