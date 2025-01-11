using UnityEngine;
using Unity.Entities;
using System.Collections.Generic;

public class EnemySpawnerAuthoring : MonoBehaviour
{
    [SerializeField] private SSO_SpawnerData spawnerData;

    public class EnemySpawnerBaker : Baker<EnemySpawnerAuthoring>
    {
        public override void Bake(EnemySpawnerAuthoring authoring)
        {
            Entity waveEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(waveEntity, new WaveSpawnerComponent
            {
                wavesDelay = authoring.spawnerData.wavesDelay
            });

            List<WaveData> wavesData = new List<WaveData>();

            foreach (SSO_WaveData ssoWaveData in authoring.spawnerData.waves)
            {
                var waveData = new WaveData
                {
                    spawnInterval = ssoWaveData.spawnInterval,
                    maxEnemiesPerSpawn = ssoWaveData.maxEnemiesPerSpawn,
                    spawnRadius = ssoWaveData.spawnRadius,
                    enemiesToSpawn = new List<EnemyDataContainer>()
                };

                foreach(var enemy in ssoWaveData.enemiesToSpawn)
                {
                    Entity enemyEntity = GetEntity(enemy.enemyData.visual, TransformUsageFlags.Dynamic);

                    waveData.enemiesToSpawn.Add(new EnemyDataContainer
                    {
                        enemy = new EnemyData
                        {
                            visual = enemyEntity,
                            health = enemy.enemyData.health,
                            damage = enemy.enemyData.damage,
                            moveSpeed = enemy.enemyData.moveSpeed,
                        },
                        quantity = enemy.quantity
                    });
                }

                wavesData.Add(waveData);
            }

            AddComponentObject(waveEntity, new WaveDataContainer { waves = wavesData });
        }
    }
}