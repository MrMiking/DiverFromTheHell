using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class WavesSpawnerSystem : SystemBase
{
    private Random _random;

    protected override void OnCreate()
    {
        _random = Random.CreateFromIndex((uint)UnityEngine.Time.frameCount);
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.WithStructuralChanges().ForEach((Entity entity, ref WaveSpawnerComponent spawner, in WaveDataContainer waveDataContainer) =>
        {
            if (spawner.currentWaveIndex >= waveDataContainer.waves.Count) return;

            spawner.currentTimeBeforeNextSpawn -= SystemAPI.Time.DeltaTime;

            WaveData currentWave = waveDataContainer.waves[spawner.currentWaveIndex];

            if (spawner.currentTimeBeforeNextSpawn <= 0)
            {
                spawner.currentWaveIndex += 1;
                spawner.currentTimeBeforeNextSpawn = spawner.wavesDelay;

                SpawnEnemies(ecb, currentWave);
                return;
            }
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }

    private void SpawnEnemies(EntityCommandBuffer ecb, WaveData wave)
    {
        foreach(EnemyDataContainer enemyData in wave.enemiesToSpawn)
        {
            int spawnCount = math.min(wave.maxEnemiesPerSpawn, enemyData.quantity);

            for(int i = 0; i < spawnCount; i++)
            {
                Entity enemyEntity = ecb.Instantiate(enemyData.enemy.visual);
                Entity spawnerEntity = SystemAPI.GetSingletonEntity<WaveSpawnerComponent>();

                float3 spawnerPosition = EntityManager.GetComponentData<LocalTransform>(spawnerEntity).Position;
                float3 spawnPosition = spawnerPosition + new float3(
                    _random.NextFloat(-wave.spawnRadius, wave.spawnRadius), 
                    0,
                    _random.NextFloat(-wave.spawnRadius, wave.spawnRadius));

                ecb.SetComponent(enemyEntity, LocalTransform.FromPosition(spawnPosition));

                ecb.AddComponent(enemyEntity, new Health
                {
                    health = enemyData.enemy.health,
                    currentHealth = enemyData.enemy.health
                });
            }
        }
    }
}