using Unity.Entities;
public struct PlayerComponent : IComponentData
{
    public float moveSpeed;
    public Entity bulletPrefab;
}