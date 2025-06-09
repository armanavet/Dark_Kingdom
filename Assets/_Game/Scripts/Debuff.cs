using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Debuff")]
public class Debuff : ScriptableObject
{
    public float Duration;
    public float Damage;
    public float Slow;
    public DebuffType Type;
    public StackingType Stacking;
}
