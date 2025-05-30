using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDebuffable
{
    public void TakeDamage(int damage);

    public void ApplySlow(float slow);
}

