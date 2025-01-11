using Unity.Entities;

public struct WaveSpawnerComponent : IComponentData
{
    public float wavesDelay;

    public float currentTimeBeforeNextSpawn;
    public int currentWaveIndex;
}