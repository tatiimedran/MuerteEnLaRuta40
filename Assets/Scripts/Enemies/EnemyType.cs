using UnityEngine;

[CreateAssetMenu(fileName = "EnemyType", menuName = "Scriptable Objects/EnemyType")]
public class EnemyType : ScriptableObject
{
    public string enemyName; // Nombre del enemigo
    public float speed; // Velocidad de movimiento
    public int health; // Salud m�xima
    public bool canMeleeAttack; // �Puede atacar cuerpo a cuerpo?
    public bool canRangedAttack; // �Puede atacar a distancia?
    public float meleeDamage; // Da�o cuerpo a cuerpo
    public float rangedDamage; // Da�o a distancia
    public GameObject projectilePrefab; // Prefab para ataques a distancia

    public float attackRange; // Rango de ataque
    public float detectionRange; // Rango para detectar al jugador

    public float attackCooldown; // Tiempo m�nimo entre ataques
}



