using Unity.Entities;
using UnityEngine;
public struct Attack : IComponentData
{
    public float attackRange;
    public float cooldown;
    public float damage;

    public bool isAttacking;

    public TargetType targetType;

    public double lastAttackTime;
}

public enum TargetType
{
    Everyone,
    Player,
    Enemy,
}