using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SSO_EnemySpawnSettings", menuName = "ScriptableObject/SSO_EnemySpawnerSettings")]
public class SSO_EnemySpawnerSettings : ScriptableObject
{
    public List<SSO_EnemyData> enemiesToSpawn;
    public float spawnInterval = 5f;
    public int maxEnemiesPerSpawn = 3;
    public float spawnRadius = 5f;
}