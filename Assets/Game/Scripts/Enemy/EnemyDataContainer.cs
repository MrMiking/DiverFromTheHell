using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EnemyDataContainer : IComponentData
{
    public EnemyData enemy;
    public int quantity;
}

public struct EnemyData
{
    public Entity visual;
    public float health;
    public float damage;
    public float moveSpeed;
    public float cooldown;
}