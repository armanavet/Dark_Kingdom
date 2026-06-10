using UnityEngine;

public interface ITargetable
{
    void ApplyDamage(float damage);
    public Transform Transform { get; }
    public float HealthPercent { get; }
    public bool IsDead { get; }
}

public enum Faction
{
    Player,
    Enemy
}
