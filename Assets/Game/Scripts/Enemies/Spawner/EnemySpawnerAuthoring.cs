using Unity.Entities;
using UnityEngine;
public class EnemySpawnerAuthoring : MonoBehaviour
{
    public SSO_EnemySpawnerSettings enemiesSpawnerSettings;
    public GameObject enemyPrefab;

    public class EnemySpawnerBaker : Baker<EnemySpawnerAuthoring>
    {
        public override void Bake(EnemySpawnerAuthoring authoring)
        {
            Entity spawnerEntity = GetEntity(TransformUsageFlags.None);

            AddComponent(spawnerEntity, new EnemySpawnerSettingsComponent
            {
                enemyPrefab = GetEntity(authoring.enemyPrefab, TransformUsageFlags.None),
                spawnInterval = authoring.enemiesSpawnerSettings.spawnInterval,
                maxEnemiesPerSpawn = authoring.enemiesSpawnerSettings.maxEnemiesPerSpawn,
                spawnRadius = authoring.enemiesSpawnerSettings.spawnRadius
            });

            DynamicBuffer<EnemiesReferenceBuffer> buffer = AddBuffer<EnemiesReferenceBuffer>(spawnerEntity);

            foreach(var enemyData in authoring.enemiesSpawnerSettings.enemiesToSpawn)
            {
                Entity enemyEntity = CreateAdditionalEntity(TransformUsageFlags.None);

                AddComponent(enemyEntity, new EnemyDataComponent
                {
                    visual = GetEntity(enemyData.visual, TransformUsageFlags.None),
                    health = enemyData.health,
                    speed = enemyData.speed,
                    detectionRange = enemyData.detectionRange
                });

                buffer.Add(new EnemiesReferenceBuffer { enemyEntity = enemyEntity });
            }
        }
    }
}