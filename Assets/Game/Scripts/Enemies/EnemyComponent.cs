using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct EnemyDataComponent : IComponentData
{
    public Entity visual;

    public float health;
    public float speed;
    public int damage;

    public float detectionRange;
}