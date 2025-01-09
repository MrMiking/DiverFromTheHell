using UnityEngine;

[CreateAssetMenu(fileName = "SSO_EnemyData", menuName = "ScriptableObject/EnemyData")]
public class SSO_EnemyData : ScriptableObject
{
    [Header("Infos")]
    public string enemyName;
    public GameObject visual;

    [Header("Stats")]
    public float health;
    public float speed;
    public int damage;

    [Header("Other")]
    public float detectionRange;
}