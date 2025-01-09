using Unity.Entities;
public struct EnemySpawnerSettingsComponent : IComponentData
{
    public Entity enemyPrefab;
    public float spawnInterval;
    public int maxEnemiesPerSpawn;
    public float spawnRadius;
}

public struct EnemiesReferenceBuffer : IBufferElementData
{
    public Entity enemyEntity;
}