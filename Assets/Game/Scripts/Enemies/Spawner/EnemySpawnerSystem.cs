using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
public partial struct EnemySpawnerSystem : ISystem
{
    private float _timeSinceLastSpawn;

    private Unity.Mathematics.Random _random;

    private EntityManager _entityManager;
    private Entity _enemySpawnerSettingsEntity;
    private EnemySpawnerSettingsComponent _enemySpawnerSettingsComponent;

    public void OnCreate(ref SystemState state)
    {
        _random = Unity.Mathematics.Random.CreateFromIndex((uint)_enemySpawnerSettingsComponent.GetHashCode());
    }

    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;

        _enemySpawnerSettingsEntity = SystemAPI.GetSingletonEntity<EnemySpawnerSettingsComponent>();

        _enemySpawnerSettingsComponent = _entityManager.GetComponentData<EnemySpawnerSettingsComponent>(_enemySpawnerSettingsEntity);

        SpawnEnemies(ref state);
    }

    private void SpawnEnemies(ref SystemState state)
    {
        _timeSinceLastSpawn += SystemAPI.Time.DeltaTime;

        if (_timeSinceLastSpawn >= _enemySpawnerSettingsComponent.spawnInterval)
        {
            _timeSinceLastSpawn = 0f;
            for (int i = 0; i < _enemySpawnerSettingsComponent.maxEnemiesPerSpawn; i++)
            {
                EntityCommandBuffer ECB = new EntityCommandBuffer(Allocator.Temp);
                Entity enemyEntity = _entityManager.Instantiate(_enemySpawnerSettingsComponent.enemyPrefab);

                LocalTransform enemyTransform = _entityManager.GetComponentData<LocalTransform>(enemyEntity);

                float2 randomOffset = _random.NextFloat2Direction() * _random.NextFloat(
                    -_enemySpawnerSettingsComponent.spawnRadius,
                    _enemySpawnerSettingsComponent.spawnRadius);
                float3 spawnPosition = new float3(randomOffset.x, 1.5f, randomOffset.y);

                enemyTransform.Position = spawnPosition;

                ECB.SetComponent(enemyEntity, enemyTransform);

                ECB.Playback(_entityManager);
                ECB.Dispose();
            }
        }
    }
}