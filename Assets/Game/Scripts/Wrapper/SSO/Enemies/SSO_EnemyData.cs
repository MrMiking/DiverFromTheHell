using UnityEngine;

[CreateAssetMenu(fileName = "SSO_EnemyData", menuName = "ScriptableObject/EnemyData")]
public class SSO_EnemyData : ScriptableObject
{
    public int level;
    public GameObject prefab;
    public float health;
    public float damage;
    public float moveSpeed;
}