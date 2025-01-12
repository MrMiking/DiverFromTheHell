using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct BulletSystem : ISystem
{
    [BurstCompile]
    private void OnUpdate(ref SystemState state)
    {
        EntityManager entityManager = state.EntityManager;
        NativeArray<Entity> allEntities = entityManager.GetAllEntities();

        PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        foreach (Entity entity in allEntities)
        {
            if(entityManager.HasComponent<BulletComponent>(entity) && entityManager.HasComponent<BulletLifeTimeComponent>(entity))
            {
                LocalTransform bulletTransform = entityManager.GetComponentData<LocalTransform>(entity);
                BulletComponent bulletComponent = entityManager.GetComponentData<BulletComponent>(entity);

                bulletTransform.Position += bulletComponent.speed * SystemAPI.Time.DeltaTime * bulletTransform.Forward();
                entityManager.SetComponentData(entity, bulletTransform);

                BulletLifeTimeComponent bulletLifeTimeComponent = entityManager.GetComponentData<BulletLifeTimeComponent>(entity);
                bulletLifeTimeComponent.remainingLifeTime -= SystemAPI.Time.DeltaTime;

                if(bulletLifeTimeComponent.remainingLifeTime <= 0f)
                {
                    entityManager.DestroyEntity(entity);
                    continue;
                }

                entityManager.SetComponentData(entity, bulletLifeTimeComponent);

                NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);

                float3 point1 = new float3(bulletTransform.Position - bulletTransform.Forward() * 0.15f);
                float3 point2 = new float3(bulletTransform.Position + bulletTransform.Forward() * 0.15f);

                uint layerMask = LayerMaskHelper.GetLayerMaskFromTwoLayer(CollisionLayer.Wall, CollisionLayer.Enemy);

                physicsWorld.CapsuleCastAll(point1, point2, bulletComponent.size / 2, float3.zero, 1f, ref hits, new CollisionFilter
                {
                    BelongsTo = (uint)CollisionLayer.Default,
                    CollidesWith = layerMask,
                });

                if (hits.Length > 0)
                {
                    for(int i = 0; i < hits.Length; i++)
                    {
                        Entity hitsEntity = hits[i].Entity;
                        if (entityManager.HasComponent<Health>(hitsEntity))
                        {
                            Health enemyComponent = entityManager.GetComponentData<Health>(hitsEntity);
                            enemyComponent.currentHealth -= bulletComponent.damage;
                            entityManager.SetComponentData(hitsEntity, enemyComponent);
                        }
                    }

                    entityManager.DestroyEntity(entity);
                }

                hits.Dispose();
            }
        }
    }
}