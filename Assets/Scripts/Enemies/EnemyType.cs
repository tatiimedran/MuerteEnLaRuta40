using UnityEngine;

[CreateAssetMenu(fileName = "EnemyType", menuName = "Scriptable Objects/EnemyType")]
public class EnemyType : ScriptableObject
{
    public string enemyName; // Enemy name
    public float speed; // Movement speed
    public int health; // Maximum health
    public bool canMeleeAttack; // Can perform melee attacks?
    public bool canRangedAttack; // Can perform ranged attacks?
    public float meleeDamage; // Melee damage
    public float rangedDamage; // Ranged damage
    public GameObject projectilePrefab; // Prefab for ranged attacks

    public float attackRange; // Attack range
    public float detectionRange; // Range to detect the player

    public float attackCooldown; // Minimum time between attacks
}
