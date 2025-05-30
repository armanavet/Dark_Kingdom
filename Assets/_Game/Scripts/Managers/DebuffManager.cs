using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DebuffManager : MonoBehaviour
{
    Dictionary<IDebuffable, List<Debuff>> ActiveDebuffs = new Dictionary<IDebuffable, List<Debuff>>();
    float timer;

    void Update()
    {
        Tick();
    }

    void ApplyDebuff(IDebuffable target, Debuff debuff)
    {
        if (ActiveDebuffs.ContainsKey(target))
        {
            var activeDebuffs = ActiveDebuffs[target];
            foreach (var activeDebuff in activeDebuffs)
            {
                if (activeDebuff.Type == debuff.Type)
                {
                    activeDebuff.DurationLeft = activeDebuff.Duration;
                    return;
                }
            }
            activeDebuffs.Add(debuff);
            return;
        }

        ActiveDebuffs.Add(target, new List<Debuff>() { debuff });
    }

    void RemoveDebuff(IDebuffable target, Debuff debuff)
    {
        var activeDebuffs = ActiveDebuffs[target];
        activeDebuffs.Remove(debuff);
        if (activeDebuffs.Count == 0)
        {
            ActiveDebuffs.Remove(target);
        }
    }

    void HandleDebuff(IDebuffable target, Debuff debuff)
    {
        if (debuff.Damage > 0) target.TakeDamage(debuff.Damage);
        if (debuff.Slow > 0) target.ApplySlow(debuff.Slow);
    }

    void Tick()
    {
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            foreach (var target in ActiveDebuffs.Keys)
            {
                if (target == null)
                {
                    ActiveDebuffs.Remove(target);
                    continue;
                }

                var debuffs = ActiveDebuffs[target];
                foreach (var debuff in debuffs)
                {
                    HandleDebuff(target, debuff);
                    debuff.DurationLeft--;
                    if (debuff.DurationLeft <= 0)
                        RemoveDebuff(target, debuff);
                }
            }
            timer = 0;
        }
    }
}

public enum DebuffType
{
    Slow,
    Fire,
    Ice
}

public enum StackingType
{
    ResetDuration
}
