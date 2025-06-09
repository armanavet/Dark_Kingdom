using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDebuffable
{
    public void ApplyDamage(float damage);

    public void ApplySlow(float slow);
}

