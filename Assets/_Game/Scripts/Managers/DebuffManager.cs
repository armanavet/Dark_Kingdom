using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DebuffManager : MonoBehaviour
{
    Dictionary<IDebuffable, Dictionary<Debuff, float>> ActiveDebuffs = new ();
    float timer;
    
    #region Singleton 
    private static DebuffManager _instance;
    public static DebuffManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<DebuffManager>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    #endregion
    void Update()
    {
        Tick();
    }

    public void RemoveTarget(IDebuffable target)
    {
        if (ActiveDebuffs.ContainsKey(target))
        {
            ActiveDebuffs.Remove(target);
        }
    }
    public void ApplyDebuff(IDebuffable target, Debuff debuff)
    {
        if (ActiveDebuffs.ContainsKey(target))
        {
            var activeDebuffs = ActiveDebuffs[target];
            List<Debuff> debuffs = activeDebuffs.Keys.ToList();
            for (int i = 0; i < debuffs.Count; i++)
            {
                if (debuffs[i].Type == debuff.Type)
                {
                    activeDebuffs.Remove(debuffs[i]);
                    activeDebuffs.Add(debuff, debuff.Duration);
                    return;
                }
            }
            
            activeDebuffs.Add(debuff,debuff.Duration);
            return;
        }

        Dictionary<Debuff, float> newDebuff = new ();
        newDebuff.Add(debuff, debuff.Duration);
        ActiveDebuffs.Add(target, newDebuff);
        HandleDebuff(target,debuff);
    }

    void RemoveDebuff(IDebuffable target, Debuff debuff)
    {
        var activeDebuffs = ActiveDebuffs[target];
        activeDebuffs.Remove(debuff);
        if (debuff.Slow > 0) target.ApplySlow(0);
        if (activeDebuffs.Count == 0)
        {
            ActiveDebuffs.Remove(target);
        }
    }

    void HandleDebuff(IDebuffable target, Debuff debuff)
    {
        if (target == null)
        {
            ActiveDebuffs.Remove(target);
            return;
        }

        if (debuff.Damage > 0) target.ApplyDamage(debuff.Damage);
        if (debuff.Slow > 0) target.ApplySlow(debuff.Slow);
    }

    void Tick()
    {
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            List<IDebuffable> TargetsToRemove = ActiveDebuffs.Keys.ToList();

            for (int i = TargetsToRemove.Count - 1; i >= 0; i--)
            {
                IDebuffable target = TargetsToRemove[i];
                if (target == null)
                {
                    ActiveDebuffs.Remove(target);
                    continue;
                }

                IncrementDuration(target);
            }

            timer = 0;
        }
    }
    void IncrementDuration(IDebuffable target)
    {
        Dictionary<Debuff, float> debuffDurations = ActiveDebuffs[target];
        List<Debuff> debuffs = debuffDurations.Keys.ToList();

        for (int i = debuffs.Count - 1; i >= 0; i--)
        {
            Debuff debuff = debuffs[i];
            HandleDebuff(target, debuff);

            debuffDurations[debuff]--;

            if (debuffDurations[debuff] <= 0)
            {
                RemoveDebuff(target, debuff);
            }
        }
    }
}

public enum DebuffType
{
    Burn,
    Poison,
    Slow
}

public enum StackingType
{
    ResetDuration
}


