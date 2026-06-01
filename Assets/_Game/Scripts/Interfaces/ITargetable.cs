using UnityEngine;

public interface ITargetable
{
    void ApplyDamage(float damage);
    public Transform Transform { get; }
    public Faction Faction { get; }
    public int TargetPriority { get; }
    public float HealthPercent { get; }
}

public enum Faction
{
    Player,
    Enemy
}
