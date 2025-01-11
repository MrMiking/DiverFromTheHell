using System.Collections.Generic;
using System.Numerics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class EnemySpawnerSystem : SystemBase
{
    private EnemySpawnerComponent enemySpawnerComponent;
    private EnemyDataContainer enemyDataContainerComponent;
    private Entity enemySpawnerEntity;

    private float nextSpawnTime;

    private Random random;

    protected override void OnCreate()
    {
        random = Random.CreateFromIndex((uint)enemySpawnerComponent.GetHashCode());
    }

    protected override void OnUpdate()
    {
        if(!SystemAPI.TryGetSingletonEntity<EnemySpawnerComponent>(out enemySpawnerEntity))
        {
            return;
        }

        enemySpawnerComponent = EntityManager.GetComponentData<EnemySpawnerComponent>(enemySpawnerEntity);
        enemyDataContainerComponent = EntityManager.GetComponentObject<EnemyDataContainer>(enemySpawnerEntity);

        if(SystemAPI.Time.ElapsedTime > nextSpawnTime)
        {
            SpawnEnemies();
        }
    }

    private void SpawnEnemies()
    {
        float enemyScale = 1; //Temp

        int level = 2;
        List<EnemyData> availableEnemies = new List<EnemyData>();

        foreach (EnemyData enemyData in enemyDataContainerComponent.enemies)
        {
            if(enemyData.level <= level)
            {
                availableEnemies.Add(enemyData);
            }
        }

        int index = random.NextInt(availableEnemies.Count);

        LocalTransform spawnerTransform = EntityManager.GetComponentData<LocalTransform>(enemySpawnerEntity);

        Entity newEnemy = EntityManager.Instantiate(availableEnemies[index].prefab);
        EntityManager.SetComponentData(newEnemy, new LocalTransform
        {
            Position = GetRandomSpawnPosition() + spawnerTransform.Position,
            Rotation = quaternion.identity,
            Scale = enemyScale
        });

        EntityManager.AddComponentData(newEnemy, new EnemyComponent
        {
            currentHealth = availableEnemies[index].health,
        });

        nextSpawnTime = (float)SystemAPI.Time.ElapsedTime + enemySpawnerComponent.spawnCooldown;
    }

    private float3 GetRandomSpawnPosition()
    {
        float2 spawnPosition = random.NextFloat2Direction() * random.NextFloat(-enemySpawnerComponent.spawnRadius, enemySpawnerComponent.spawnRadius);

        return new float3(spawnPosition.x, 0, spawnPosition.y);
    }
}