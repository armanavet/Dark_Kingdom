
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;
using AudioSystem;

public abstract class AudioBase
{
    [NonSerialized] public Dictionary<ClipFor, AudioClip> clipBase = new Dictionary<ClipFor, AudioClip>();
    [NonSerialized] public Dictionary<MixerFor, AudioMixerGroup> mixerBase = new Dictionary<MixerFor, AudioMixerGroup>();
    public abstract void BuildBase();

    public virtual AudioClip GetClip(ClipFor type)
    {
        clipBase.TryGetValue(type, out var clip);
        return clip;
    }
    public virtual AudioMixerGroup GetMixer(MixerFor type)
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
        clipBase = new Dictionary<ClipFor, AudioClip>();
        mixerBase = new Dictionary<MixerFor, AudioMixerGroup>();

        if (action != null) clipBase[ClipFor.Attack] = action;
        // Move uses multiple clips → store null, but mark it
        if (move != null && move.Length > 0)
            clipBase[ClipFor.Move] = null;

        if (mixerGroupEnemy != null) mixerBase[MixerFor.Enemy] = mixerGroupEnemy;
    }

    public override AudioClip GetClip(ClipFor type)
    {
        if (!clipBase.TryGetValue(type, out var clip))
            return null;

        if (type == ClipFor.Move)
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
        clipBase = new Dictionary<ClipFor, AudioClip>();
        mixerBase = new Dictionary<MixerFor, AudioMixerGroup>();

        if (place != null) clipBase[ClipFor.Place] = place;
        if (launch != null) clipBase[ClipFor.Launch] = launch;
        if (hit != null) clipBase[ClipFor.Hit] = hit;
        if (placeVfx_Sfx != null) clipBase[ClipFor.PlaceVfx] = placeVfx_Sfx;
        if (upgradeVfx_Sfx != null) clipBase[ClipFor.UpgardeVfx] = upgradeVfx_Sfx;

        if (mixerGroupTower != null) mixerBase[MixerFor.Tower] = mixerGroupTower;
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
        clipBase = new Dictionary<ClipFor, AudioClip>();
        mixerBase = new Dictionary<MixerFor, AudioMixerGroup>();

        if (click != null) clipBase[ClipFor.ClickUi] = click;
        if (sell != null) clipBase[ClipFor.ClickSellUi] = sell;
        if (upgrade != null) clipBase[ClipFor.ClickUpgradeUi] = upgrade;
        if (denied != null) clipBase[ClipFor.Denied] = denied;

        if (mixerGroupUI != null) mixerBase[MixerFor.Ui] = mixerGroupUI;
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
        clipBase = new Dictionary<ClipFor, AudioClip>();
        mixerBase = new Dictionary<MixerFor, AudioMixerGroup>();

        if (active != null) clipBase[ClipFor.Active] = active;
        if (passive != null) clipBase[ClipFor.Passive] = passive;
        if (gong != null) clipBase[ClipFor.Gong] = gong;

        if (mixerGroupState != null) mixerBase[MixerFor.State] = mixerGroupState;
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
    public AudioClip GetClip(ClipFor clipFor, object type = null)
    {
        if (type is UnitType enemyType && enemyDict.TryGetValue(enemyType, out var enemy))
            return enemy.GetClip(clipFor);

        if (type is TowerType towerType && towerDict.TryGetValue(towerType, out var tower))
            return tower.GetClip(clipFor);

        //foreach (var ui in uiClips)
        if (uiClips.clipBase.ContainsKey(clipFor))
            return uiClips.GetClip(clipFor);

        //foreach (var st in stateClips)
        if (stateClips.clipBase.ContainsKey(clipFor))
            return stateClips.GetClip(clipFor);

        Debug.LogError($"ClipFor {clipFor} not found!");
        return null;
    }
    public AudioMixerGroup GetMixer(MixerFor mixerFor, object type = null)
    {
        if (type is UnitType enemyType && enemyDict.TryGetValue(enemyType, out var enemy))
            return enemy.GetMixer(mixerFor);

        if (type is TowerType towerType && towerDict.TryGetValue(towerType, out var tower))
            return tower.GetMixer(mixerFor);

        //foreach (var ui in uiClips)
        if (uiClips.mixerBase.ContainsKey(mixerFor))
            return uiClips.GetMixer(mixerFor);

        //foreach (var st in stateClips)
        if (uiClips.mixerBase.ContainsKey(mixerFor))
            return uiClips.GetMixer(mixerFor);

        Debug.LogError($"ClipFor {mixerFor} not found!");
        return null;
    }

}
public enum ClipFor
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
public enum MixerFor
{
    Enemy,
    Tower,
    Ui,
    State
}
