using Unity.Entities;
using UnityEngine;
public struct Health : IComponentData
{
    public float currentHealth;
    public float health;
}