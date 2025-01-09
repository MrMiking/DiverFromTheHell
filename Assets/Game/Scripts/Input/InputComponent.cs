using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
public struct InputComponent : IComponentData
{
    public float2 Movement;
    public float2 MousePosition;
    public bool Shoot;
}