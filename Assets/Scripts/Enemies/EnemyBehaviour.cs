using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBehavior : MonoBehaviour
{
    public EnemyType enemyType; // Referencia al Scriptable Object
    private Rigidbody2D rb;
    private Vector2 direction;
    private Animator enemyAnimator; // Cambiado de "animator" a "enemyAnimator"

    private Transform player; // Referencia al jugador

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<Animator>(); // Asignación corregida

        // Encuentra al jugador automáticamente (usando su tag "Player")
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player == null)
        {
            Debug.LogError("No se encontró un objeto con la etiqueta 'Player'. Asegúrate de asignar el tag al jugador.");
        }
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // Detectar movimiento o ataque
            if (distanceToPlayer <= enemyType.detectionRange && distanceToPlayer > enemyType.attackRange)
            {
                MoveTowardsPlayer(); // Persigue al jugador
            }
            else if (distanceToPlayer <= enemyType.attackRange)
            {
                StopMoving(); // Detiene movimiento, pero aún puede atacar
                HandleAttack(distanceToPlayer); // Activar ataque
            }
            else
            {
                StopMoving(); // Fuera del rango de detección
            }
        }
    }

    void MoveTowardsPlayer()
    {
        direction = (player.position - transform.position).normalized;

        if (direction != Vector2.zero)
        {
            rb.linearVelocity = direction * enemyType.speed;

            // Actualizar los parámetros del Animator
            enemyAnimator.SetFloat("MoveHorizontal", direction.x);
            enemyAnimator.SetFloat("MoveVertical", direction.y);
            enemyAnimator.SetBool("EnemyIsMoving", true);

            // Depuración: confirma movimiento
            Debug.Log($"Moviéndose hacia el jugador. Dirección: {direction}, Velocidad: {rb.linearVelocity}");
        }
    }

    void StopMoving()
    {
        rb.linearVelocity = Vector2.zero; // Detener el movimiento
        enemyAnimator.SetBool("EnemyIsMoving", false);

        // Depuración: enemigo detenido
        Debug.Log("Enemigo detenido.");
    }

    void HandleAttack(float distanceToPlayer)
    {
        if (enemyType.canMeleeAttack && distanceToPlayer <= enemyType.attackRange)
        {
            enemyAnimator.SetTrigger("MeleeAttack");
            Debug.Log($"{enemyType.enemyName} está atacando cuerpo a cuerpo.");
        }
        else if (enemyType.canRangedAttack && distanceToPlayer > enemyType.attackRange && distanceToPlayer <= enemyType.detectionRange)
        {
            enemyAnimator.SetTrigger("RangedAttack");
            Debug.Log($"{enemyType.enemyName} está atacando a distancia.");
            if (enemyType.projectilePrefab != null)
            {
                Instantiate(enemyType.projectilePrefab, transform.position, Quaternion.identity);
                Debug.Log("Proyectil instanciado.");
            }
        }
    }
}