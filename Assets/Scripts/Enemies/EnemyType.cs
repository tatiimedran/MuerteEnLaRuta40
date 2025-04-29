using UnityEngine;

[CreateAssetMenu(fileName = "EnemyType", menuName = "Scriptable Objects/EnemyType")]
public class EnemyType : ScriptableObject
{
    public string enemyName; // Nombre del enemigo
    public float speed; // Velocidad de movimiento
    public int health; // Salud máxima
    public bool canMeleeAttack; // ¿Puede atacar cuerpo a cuerpo?
    public bool canRangedAttack; // ¿Puede atacar a distancia?
    public float meleeDamage; // Daño cuerpo a cuerpo
    public float rangedDamage; // Daño a distancia
    public GameObject projectilePrefab; // Prefab para ataques a distancia

    public float attackRange; // Rango de ataque
    public float detectionRange; // Rango para detectar al jugador

    public float attackCooldown; // Tiempo mínimo entre ataques
}



