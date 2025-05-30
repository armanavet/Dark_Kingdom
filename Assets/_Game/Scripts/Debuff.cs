using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Debuff")]
public class Debuff : ScriptableObject
{
    public int Duration;
    public int DurationLeft;
    public int Damage;
    public float Slow;
    public DebuffType Type;
    public StackingType Stacking;
}
