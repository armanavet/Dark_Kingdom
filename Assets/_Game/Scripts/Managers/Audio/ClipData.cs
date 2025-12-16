using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;
using AudioSystem;

public abstract class AudioStuffRepository
{
    [NonSerialized] public Dictionary<ClipType, AudioClip> ClipsDict = new Dictionary<ClipType, AudioClip>();
    [NonSerialized] public Dictionary<MixerType, AudioMixerGroup> MixerGroupDict = new Dictionary<MixerType, AudioMixerGroup>();
    public abstract void BuildBase();

    public virtual AudioClip GetClip(ClipType type)
    {
        ClipsDict.TryGetValue(type, out var clip);
        return clip;
    }
    public virtual AudioMixerGroup GetMixer(MixerType type)
    {
        MixerGroupDict.TryGetValue(type, out var mixer);
        return mixer;
    }

}
[Serializable]
public class EnemyClips : AudioStuffRepository
{
    [SerializeReference] public UnitType unitType;
    public AudioMixerGroup mixerGroupEnemy;
    public AudioClip OnAttack;
    public AudioClip[] OnMove;

    public override void BuildBase()
    {
        ClipsDict = new Dictionary<ClipType, AudioClip>();
        MixerGroupDict = new Dictionary<MixerType, AudioMixerGroup>();

        if (OnAttack != null) ClipsDict[ClipType.OnAttack_Enemy] = OnAttack;
        if (OnMove != null && OnMove.Length > 0) ClipsDict[ClipType.OnMove_Enemy] = null;

        if (mixerGroupEnemy != null) MixerGroupDict[MixerType.Enemy] = mixerGroupEnemy;
    }

    public override AudioClip GetClip(ClipType type)
    {
        if (!ClipsDict.TryGetValue(type, out var clip))
            return null;

        if (type == ClipType.OnMove_Enemy)
            return OnMove[Random.Range(0, OnMove.Length)];

        return clip;
    }
}
[Serializable]
public class TowerClips : AudioStuffRepository
{
    public TowerType towerType;
    public AudioMixerGroup mixerGroupTower;
    public AudioClip OnPlace;
    public AudioClip OnLaunch;
    public AudioClip OnTargetHit;
    public AudioClip OnPlaceVisualEffect;
    public AudioClip OnUpgradeVisualEffect;

    public override void BuildBase()
    {
        ClipsDict = new Dictionary<ClipType, AudioClip>();
        MixerGroupDict = new Dictionary<MixerType, AudioMixerGroup>();

        if (OnPlace != null) ClipsDict[ClipType.OnPlace_Tower] = OnPlace;
        if (OnLaunch != null) ClipsDict[ClipType.OnLaunch_Tower] = OnLaunch;
        if (OnTargetHit != null) ClipsDict[ClipType.OnHit_Tower] = OnTargetHit;
        if (OnPlaceVisualEffect != null) ClipsDict[ClipType.OnPlaceVisualEffect_Tower] = OnPlaceVisualEffect;
        if (OnUpgradeVisualEffect != null) ClipsDict[ClipType.OnUpgradeVisualEffect_Tower] = OnUpgradeVisualEffect;

        if (mixerGroupTower != null) MixerGroupDict[MixerType.Tower] = mixerGroupTower;
    }
}
[Serializable]
public class UIClips : AudioStuffRepository
{
    public AudioMixerGroup mixerGroupUI;
    public AudioClip OnMauseClick;
    public AudioClip OnTowerPanelClick_Purchase;
    public AudioClip OnTowerButtonClick_Sell;
    public AudioClip OnTowerButtonClick_Upgrade;
    public AudioClip OnTowerPlacmentDenied;

    public override void BuildBase()
    {
        ClipsDict = new Dictionary<ClipType, AudioClip>();
        MixerGroupDict = new Dictionary<MixerType, AudioMixerGroup>();

        if (OnMauseClick != null) ClipsDict[ClipType.OnClick_UI] = OnMauseClick;
        if (OnTowerPanelClick_Purchase != null) ClipsDict[ClipType.OnPurchaseButtonClick_UI] = OnTowerPanelClick_Purchase;
        if (OnTowerButtonClick_Sell != null) ClipsDict[ClipType.OnSellButtonClick_UI] = OnTowerButtonClick_Sell;
        if (OnTowerButtonClick_Upgrade != null) ClipsDict[ClipType.OnUpgradeButtonClick_UI] = OnTowerButtonClick_Upgrade;
        if (OnTowerPlacmentDenied != null) ClipsDict[ClipType.OnDenied_Tower] = OnTowerPlacmentDenied;

        if (mixerGroupUI != null) MixerGroupDict[MixerType.UI] = mixerGroupUI;
    }
}
[Serializable]
public class AmbyenceClips
{
    public AudioMixerGroup mixerGroupUI;
    public AudioClip OnRiver;
    public AudioClip OnForestAmbyence;
    //environment clips => river, forest
}
[Serializable]
public class StateClips : AudioStuffRepository
{
    public AudioMixerGroup mixerGroupState;
    public AudioClip OnState_Active;
    public AudioClip OnState_Passive;
    public AudioClip OnState_WaveStart
        ;

    public override void BuildBase()
    {
        ClipsDict = new Dictionary<ClipType, AudioClip>();
        MixerGroupDict = new Dictionary<MixerType, AudioMixerGroup>();

        if (OnState_Active != null) ClipsDict[ClipType.OnActive_State] = OnState_Active;
        if (OnState_Passive != null) ClipsDict[ClipType.OnPassive_State] = OnState_Passive;
        if (OnState_WaveStart != null) ClipsDict[ClipType.OnWaveStart_State] = OnState_WaveStart;

        if (mixerGroupState != null) MixerGroupDict[MixerType.State] = mixerGroupState;
    }
}
[Serializable]
public class ClipsRepository
{
    public AudioMixerGroup mixerGroup;
    public EnemyClips[] EnemyClips;
    public TowerClips[] TowerClips;
    public UIClips UIClips;
    public StateClips BackgroundClips;

    Dictionary<UnitType, EnemyClips> enemyDict = new Dictionary<UnitType, EnemyClips>();
    Dictionary<TowerType, TowerClips> towerDict = new Dictionary<TowerType, TowerClips>();
    List<AudioStuffRepository[]> allClips;
    private void OrganizeClips()
    {
        allClips = new List<AudioStuffRepository[]>
        {
            EnemyClips,
            TowerClips,
        };

        foreach (var clipsArray in allClips)
        {
            if (clipsArray == null) continue;
            foreach (var item in clipsArray) item?.BuildBase();
        }
        UIClips?.BuildBase();
        BackgroundClips?.BuildBase();
    }
    public void Organize()
    {
        OrganizeClips();
        FillSelectedDictionary.Fill(enemyDict, EnemyClips, type => type.unitType);
        FillSelectedDictionary.Fill(towerDict, TowerClips, type => type.towerType);
    }
    public AudioClip GetClip(ClipType clipFor)
    {
        if (UIClips.ClipsDict.ContainsKey(clipFor)) return UIClips.GetClip(clipFor);

        if (BackgroundClips.ClipsDict.ContainsKey(clipFor)) return BackgroundClips.GetClip(clipFor);

        Debug.LogError($"Mixer for - {clipFor} not found!");
        return null;
    }
    public AudioClip GetClip<TEnum>(ClipType clipFor, TEnum type)
        where TEnum : Enum
    {
        if (type is UnitType enemyType && enemyDict.TryGetValue(enemyType, out var enemy)) return enemy.GetClip(clipFor);

        if (type is TowerType towerType && towerDict.TryGetValue(towerType, out var tower)) return tower.GetClip(clipFor);

        Debug.LogError($"Clip for - {clipFor}, type - {type} not found!");
        return null;
    }
    public AudioMixerGroup GetMixer<TEnum>(MixerType mixerFor, TEnum type)
        where TEnum : Enum
    {
        if (type is UnitType enemyType && enemyDict.TryGetValue(enemyType, out var enemy)) return enemy.GetMixer(mixerFor);

        if (type is TowerType towerType && towerDict.TryGetValue(towerType, out var tower)) return tower.GetMixer(mixerFor);

        Debug.LogError($"Mixer for - {mixerFor}, type - {type} not found!");
        return null;
    }
    public AudioMixerGroup GetMixer(MixerType mixerFor)
    {
        if (UIClips.MixerGroupDict.ContainsKey(mixerFor)) return UIClips.GetMixer(mixerFor);

        if (BackgroundClips.MixerGroupDict.ContainsKey(mixerFor)) return UIClips.GetMixer(mixerFor);

        Debug.LogError($"Mixer for - {mixerFor} not found!");
        return null;
    }
}
public enum ClipType
{
    OnAttack_Enemy,
    OnMove_Enemy,
    OnLaunch_Tower,
    OnHit_Tower,
    OnPlace_Tower,
    OnDenied_Tower,
    OnPlaceVisualEffect_Tower,
    OnUpgradeVisualEffect_Tower,
    OnClick_UI,
    OnPurchaseButtonClick_UI,
    OnSellButtonClick_UI,
    OnUpgradeButtonClick_UI,
    OnActive_State,
    OnPassive_State,
    OnWaveStart_State
}
public enum MixerType
{
    Enemy,
    Tower,
    Projectile_Shell,
    UI,
    State
}
