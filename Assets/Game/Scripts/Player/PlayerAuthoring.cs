using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject bulletPrefab;
    public int numOfBulletsToSpawn = 50;
    [Range(0f, 10f)] public float bulletSpread;

    private class PlayerBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new PlayerComponent
            {
                moveSpeed = authoring.moveSpeed,
                bulletPrefab = GetEntity(authoring.bulletPrefab, TransformUsageFlags.None),
                numOfBulletsToSpawn = authoring.numOfBulletsToSpawn,
                bulletSpread = authoring.bulletSpread
            });
        }
    }
}