using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;
using AudioSystem;

public abstract class AudioBase
{
    [NonSerialized] public Dictionary<ClipType, AudioClip> clipBase = new Dictionary<ClipType, AudioClip>();
    [NonSerialized] public Dictionary<MixerType, AudioMixerGroup> mixerBase = new Dictionary<MixerType, AudioMixerGroup>();
    public abstract void BuildBase();

    public virtual AudioClip GetClip(ClipType type)
    {
        clipBase.TryGetValue(type, out var clip);
        return clip;
    }
    public virtual AudioMixerGroup GetMixer(MixerType type)
    {
        mixerBase.TryGetValue(type, out var mixer);
        return mixer;
    }

}
[Serializable]
public class EnemyClips : AudioBase
{
    [SerializeReference] public UnitType unitType;
    public AudioMixerGroup mixerGroupEnemy;
    public AudioClip action;
    public AudioClip[] move;

    public override void BuildBase()
    {
        clipBase = new Dictionary<ClipType, AudioClip>();
        mixerBase = new Dictionary<MixerType, AudioMixerGroup>();

        if (action != null) clipBase[ClipType.Attack] = action;
        // Move uses multiple clips → store null, but mark it
        if (move != null && move.Length > 0)
            clipBase[ClipType.Move] = null;

        if (mixerGroupEnemy != null) mixerBase[MixerType.Enemy] = mixerGroupEnemy;
    }

    public override AudioClip GetClip(ClipType type)
    {
        if (!clipBase.TryGetValue(type, out var clip))
            return null;

        if (type == ClipType.Move)
            return move[Random.Range(0, move.Length)];

        return clip;
    }
}
[Serializable]
public class TowerClips : AudioBase
{
    public TowerType towerType;
    public AudioMixerGroup mixerGroupTower;
    public AudioClip place;
    public AudioClip launch;
    public AudioClip hit;
    public AudioClip placeVfx_Sfx;
    public AudioClip upgradeVfx_Sfx;

    public override void BuildBase()
    {
        clipBase = new Dictionary<ClipType, AudioClip>();
        mixerBase = new Dictionary<MixerType, AudioMixerGroup>();

        if (place != null) clipBase[ClipType.Place] = place;
        if (launch != null) clipBase[ClipType.Launch] = launch;
        if (hit != null) clipBase[ClipType.Hit] = hit;
        if (placeVfx_Sfx != null) clipBase[ClipType.PlaceVfx] = placeVfx_Sfx;
        if (upgradeVfx_Sfx != null) clipBase[ClipType.UpgardeVfx] = upgradeVfx_Sfx;

        if (mixerGroupTower != null) mixerBase[MixerType.Tower] = mixerGroupTower;
    }
}
[Serializable]
public class UIClips : AudioBase
{
    public AudioMixerGroup mixerGroupUI;
    public AudioClip click;
    public AudioClip sell;
    public AudioClip upgrade;
    public AudioClip denied;

    public override void BuildBase()
    {
        clipBase = new Dictionary<ClipType, AudioClip>();
        mixerBase = new Dictionary<MixerType, AudioMixerGroup>();

        if (click != null) clipBase[ClipType.ClickUi] = click;
        if (sell != null) clipBase[ClipType.ClickSellUi] = sell;
        if (upgrade != null) clipBase[ClipType.ClickUpgradeUi] = upgrade;
        if (denied != null) clipBase[ClipType.Denied] = denied;

        if (mixerGroupUI != null) mixerBase[MixerType.Ui] = mixerGroupUI;
    }
}
[Serializable]
public class AmbyenceClips
{
    public AudioMixerGroup mixerGroupUI;
    public AudioClip river;
    public AudioClip forest;
    //environment clips => river, forest
}
[Serializable]
public class StateClips : AudioBase
{
    public AudioMixerGroup mixerGroupState;
    public AudioClip active;
    public AudioClip passive;
    public AudioClip gong;

    public override void BuildBase()
    {
        clipBase = new Dictionary<ClipType, AudioClip>();
        mixerBase = new Dictionary<MixerType, AudioMixerGroup>();

        if (active != null) clipBase[ClipType.Active] = active;
        if (passive != null) clipBase[ClipType.Passive] = passive;
        if (gong != null) clipBase[ClipType.Gong] = gong;

        if (mixerGroupState != null) mixerBase[MixerType.State] = mixerGroupState;
    }
}
[Serializable]
public class Clips
{
    public AudioMixerGroup mixerGroup;
    public EnemyClips[] enemyClips;
    public TowerClips[] towerClips;
    public UIClips uiClips;
    public StateClips stateClips;

    Dictionary<UnitType, EnemyClips> enemyDict = new Dictionary<UnitType, EnemyClips>();
    Dictionary<TowerType, TowerClips> towerDict = new Dictionary<TowerType, TowerClips>();
    List<AudioBase[]> allClips;
    //private TData Get<TEnum, TData>(TEnum key, Dictionary<TEnum, TData> dict)
    //{
    //    if (!dict.TryGetValue(key, out var value))
    //    {
    //        Debug.LogError($"Missing key {key} in dictionary.");
    //        return default;
    //    }
    //    return value;
    //}
    private void OrganizeClips()
    {
        allClips = new List<AudioBase[]>
        {
            enemyClips,
            towerClips,
            //uiClips,
            //stateClips
        };
        foreach (var clipsArray in allClips)
        {
            if (clipsArray == null) continue;
            foreach (var item in clipsArray) item?.BuildBase();
        }
    }
    public void OrganizeAll()
    {
        OrganizeClips();
        FillSelectedDictionary.Fill(enemyDict, enemyClips, type => type.unitType);
        FillSelectedDictionary.Fill(towerDict, towerClips, type => type.towerType);
    }
    public AudioClip GetClip(ClipType clipFor)
    {
        if (uiClips.clipBase.ContainsKey(clipFor))
            return uiClips.GetClip(clipFor);
        if (stateClips.clipBase.ContainsKey(clipFor))
            return stateClips.GetClip(clipFor);
        Debug.LogError($"ClipFor {clipFor} not found!");
        return null;
    }
    public AudioClip GetClip<TEnum>(ClipType clipFor, TEnum type)
        where TEnum : Enum
    {
        if (type is UnitType enemyType && enemyDict.TryGetValue(enemyType, out var enemy))
            return enemy.GetClip(clipFor);
        if (type is TowerType towerType && towerDict.TryGetValue(towerType, out var tower))
            return tower.GetClip(clipFor);
        Debug.LogError($"ClipFor {clipFor} not found!");
        return null;
    }
    public AudioMixerGroup GetMixer<TEnum>(MixerType mixerFor, TEnum type) 
        where TEnum : Enum
    {
        if (type is UnitType enemyType && enemyDict.TryGetValue(enemyType, out var enemy))
            return enemy.GetMixer(mixerFor);
        if (type is TowerType towerType && towerDict.TryGetValue(towerType, out var tower))
            return tower.GetMixer(mixerFor);
        Debug.LogError($"ClipFor {mixerFor} not found!");
        return null;
    }
    public AudioMixerGroup GetMixer(MixerType mixerFor)
    {
        if (uiClips.mixerBase.ContainsKey(mixerFor))
            return uiClips.GetMixer(mixerFor);
        if (uiClips.mixerBase.ContainsKey(mixerFor))
            return uiClips.GetMixer(mixerFor);
        Debug.LogError($"ClipFor {mixerFor} not found!");
        return null;
    }
}
public enum ClipType
{
    Attack,
    Move,
    Launch,
    Hit,
    Place,
    Denied,
    PlaceVfx,
    UpgardeVfx,
    ClickUi,
    ClickSellUi,
    ClickUpgradeUi,
    Active,
    Passive,
    Gong
}
public enum MixerType
{
    Enemy,
    Tower,
    Ui,
    State
}
