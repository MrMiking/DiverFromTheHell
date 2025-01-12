using UnityEngine;

[CreateAssetMenu(fileName = "SSO_EnemyData", menuName = "ScriptableObject/SSO_EnemyData")]
public class SSO_EnemyData : ScriptableObject
{
    public string enemyName;
    public GameObject visual;
    public float health;
    public float damage;
    public float moveSpeed;
    public float attackSpeed;
}