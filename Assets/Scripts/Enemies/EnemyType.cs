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
    public float meleeAttackRange; // Rango para ataques cuerpo a cuerpo
    public float rangedAttackRange; // Rango para ataques a distancia
    public GameObject projectilePrefab; // Prefab for ranged attacks
    public float detectionRange; // Range to detect the player
    public float attackCooldown; // Minimum time between attacks
    [Header("Death Effects")]
    public GameObject deathEffectPrefab; // Prefab de partículas de explosión (opcional)
}
