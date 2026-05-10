using UnityEngine;

public interface ITargetable
{
    public Transform GetTransform();
    void ApplyDamage(float damage);
    float GetHealthPrecent();
    int GetTargetPriority();
    Faction GetFaction();
}

public enum Faction
{
    Player,
    Enemy
}
