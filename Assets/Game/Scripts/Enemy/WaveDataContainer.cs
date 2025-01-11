using System.Collections.Generic;
using Unity.Entities;

public class WaveDataContainer : IComponentData
{
    public List<WaveData> waves;
}

public struct WaveData
{
    public List<EnemyDataContainer> enemiesToSpawn;
    public float spawnInterval;
    public int maxEnemiesPerSpawn;
    public float spawnRadius;
}