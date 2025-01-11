using UnityEngine;
using Unity.Entities;
using System.Collections.Generic;

public class EnemySpawnerAuthoring : MonoBehaviour
{
    public float spawnCooldown = 1;
    public float spawnRadius = 5;
    public List<SSO_EnemyData> enemies;

    public class EnemySpawnerBaker : Baker<EnemySpawnerAuthoring>
    {
        public override void Bake(EnemySpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new EnemySpawnerComponent
            {
                spawnCooldown = authoring.spawnCooldown,
                spawnRadius = authoring.spawnRadius,
            });

            List<EnemyData> enemyData = new List<EnemyData>();

            foreach (SSO_EnemyData ssoEnemyData in authoring.enemies)
            {
                enemyData.Add(new EnemyData
                {
                    level = ssoEnemyData.level,
                    prefab = GetEntity(ssoEnemyData.prefab, TransformUsageFlags.None),
                    health = ssoEnemyData.health,
                    damage = ssoEnemyData.damage,
                    moveSpeed = ssoEnemyData.moveSpeed,
                });
            }

            AddComponentObject(entity, new EnemyDataContainer { enemies = enemyData });
        }
    }
}