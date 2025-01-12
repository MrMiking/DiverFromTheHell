using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject bulletPrefab;

    private class PlayerBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new PlayerComponent
            {
                moveSpeed = authoring.moveSpeed,
                bulletPrefab = GetEntity(authoring.bulletPrefab, TransformUsageFlags.None),
            });

            AddComponent(entity, new Health { 
                health = 1000, 
                currentHealth = 1000 
            });

            AddComponent(entity, new PlayerTag { });
        }
    }
}