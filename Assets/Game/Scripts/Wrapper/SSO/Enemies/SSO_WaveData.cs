using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SSO_EnemySpawnSettings", menuName = "ScriptableObject/SSO_WaveData")]
public class SSO_WaveData : ScriptableObject
{
    public List<EnemySpawnData> enemiesToSpawn;
    public float spawnInterval = 5f;
    public int maxEnemiesPerSpawn = 100;
    public float spawnRadius = 5f;

    [System.Serializable]
    public class EnemySpawnData
    {
        public SSO_EnemyData enemyData;
        public int quantity;
    }
}