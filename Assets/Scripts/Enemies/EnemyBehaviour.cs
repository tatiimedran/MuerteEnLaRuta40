using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(PolygonCollider2D))]
public class EnemyBehavior : MonoBehaviour
{
    public EnemyType enemyType; // Referencia al Scriptable Object
    private int currentHealth; // Vida específica de esta instancia del enemigo

    private Rigidbody2D rb;
    private Vector2 direction;
    private Animator enemyAnimator;
    private PolygonCollider2D enemyCollider; // Cambiado a PolygonCollider2D
    private SpriteRenderer spriteRenderer;

    private Transform player;

    private bool isBlinking = false; // Controla si el enemigo ya está parpadeando
    private bool isPlayerInRange = false; // Controla si el jugador está en rango o colisiona físicamente
    private float lastAttackTime = 0f; // Tiempo del último ataque

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<Animator>();
        enemyCollider = GetComponent<PolygonCollider2D>(); // Usamos PolygonCollider2D
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Configura el Rigidbody2D para evitar rotaciones
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Reducir la masa del enemigo para evitar empujes significativos
        rb.mass = 0.5f;

        // Inicializar la vida actual con el valor del Scriptable Object
        currentHealth = enemyType.health;

        // Encuentra al jugador automáticamente (usando su tag "Player")
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player == null)
        {
            Debug.LogError("No se encontró un objeto con la etiqueta 'Player'. Asegúrate de asignar el tag al jugador.");
        }
    }

    void FixedUpdate()
    {
        // Solo ejecuta la lógica si el enemigo está vivo
        if (currentHealth > 0 && player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // Actualiza el estado de "rango" basado en la distancia
            isPlayerInRange = distanceToPlayer <= enemyType.attackRange;

            if (distanceToPlayer <= enemyType.detectionRange && distanceToPlayer > enemyType.attackRange)
            {
                MoveTowardsPlayer();
            }
            else if (distanceToPlayer <= enemyType.attackRange)
            {
                StopMoving();
                HandleAttack(distanceToPlayer); // Verificar ataque en rango específico
            }
            else
            {
                StopMoving();
            }
        }

        UpdateColliderToCurrentFrame(); // Actualizamos el colisionador basado en el frame del sprite
    }

    void MoveTowardsPlayer()
    {
        direction = (player.position - transform.position).normalized;

        if (direction != Vector2.zero)
        {
            rb.linearVelocity = direction * enemyType.speed;
            enemyAnimator.SetFloat("MoveHorizontal", direction.x);
            enemyAnimator.SetFloat("MoveVertical", direction.y);
            enemyAnimator.SetBool("EnemyIsMoving", true);
        }
    }

    void StopMoving()
    {
        rb.linearVelocity = Vector2.zero;
        enemyAnimator.SetBool("EnemyIsMoving", false);
    }

    void HandleAttack(float distanceToPlayer)
    {
        // Asegurarse de que el jugador esté dentro del rango de ataque y en contacto
        if (distanceToPlayer <= enemyType.attackRange && isPlayerInRange)
        {
            // Obtener el componente de salud del jugador
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

            // Verificar que el jugador tiene vida antes de proceder
            if (playerHealth != null && playerHealth.CurrentHealth > 0)
            {
                // Verifica si ha pasado el tiempo de cooldown desde el último ataque
                if (Time.time >= lastAttackTime + enemyType.attackCooldown)
                {
                    if (enemyType.canMeleeAttack)
                    {
                        enemyAnimator.SetTrigger("MeleeAttack");

                        // Aplicar daño cuerpo a cuerpo
                        playerHealth.TakeDamage((int)enemyType.meleeDamage); // Convertimos float a int
                        Debug.Log($"El enemigo atacó con un golpe cuerpo a cuerpo y le hizo {enemyType.meleeDamage} de daño al jugador.");
                    }
                    else if (enemyType.canRangedAttack && distanceToPlayer <= enemyType.detectionRange)
                    {
                        enemyAnimator.SetTrigger("RangedAttack");

                        // Aplicar daño a distancia y disparar proyectil
                        if (enemyType.projectilePrefab != null)
                        {
                            Instantiate(enemyType.projectilePrefab, transform.position, Quaternion.identity);
                        }

                        playerHealth.TakeDamage((int)enemyType.rangedDamage); // Convertimos float a int
                        Debug.Log($"El enemigo atacó con un ataque a distancia e hizo {enemyType.rangedDamage} de daño.");
                    }

                    // Actualiza el tiempo del último ataque
                    lastAttackTime = Time.time;
                }
            }
            else
            {
                Debug.Log("El jugador no tiene más vida. El enemigo detiene los ataques.");
            }
        }
    }

    private void UpdateColliderToCurrentFrame()
    {
        if (spriteRenderer.sprite != null)
        {
            enemyCollider.pathCount = spriteRenderer.sprite.GetPhysicsShapeCount();

            for (int i = 0; i < enemyCollider.pathCount; i++)
            {
                List<Vector2> pointsList = new List<Vector2>();
                spriteRenderer.sprite.GetPhysicsShape(i, pointsList);

                enemyCollider.SetPath(i, pointsList);
            }
        }
    }

    public void TakeDamage(int damage, bool ignorePlayerRange = false)
    {
        // Permitir daño incluso si el jugador no está cerca, dependiendo del tipo de ataque
        if (ignorePlayerRange || isPlayerInRange)
        {
            currentHealth -= damage; // Reducir la vida de esta instancia
            Debug.Log($"Enemigo: {enemyType.enemyName} recibió {damage} de daño. Salud restante: {currentHealth}");

            // Iniciar el efecto de parpadeo si no está ya activo
            if (!isBlinking)
            {
                StartCoroutine(BlinkEffect(0.2f, 3)); // Breve duración con cambios visibles
            }

            if (currentHealth <= 0)
            {
                Die();
            }
        }
        else
        {
            Debug.Log("El jugador no está en contacto físico con el enemigo. No se aplica daño.");
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} está muriendo.");

        // Activar la animación de muerte
        enemyAnimator.SetTrigger("Death");

        // Desactivar colisionadores para que el enemigo ya no interfiera
        enemyCollider.enabled = false;

        // Detener completamente el movimiento del Rigidbody2D
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // Cambiar a Kinematic (si no lo estaba antes)
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Desactivar este script para detener toda su lógica
        this.enabled = false;

        Debug.Log($"{gameObject.name} ahora reproduce la animación de muerte.");

        // Llamar a una corutina para destruir el enemigo tras unos segundos
        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        float deathAnimationDuration = enemyAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(deathAnimationDuration + 2f);
        Destroy(gameObject);
    }

    private IEnumerator BlinkEffect(float blinkDuration, int blinkTimes)
    {
        isBlinking = true;
        Color originalColor = spriteRenderer.color;

        for (int i = 0; i < blinkTimes; i++)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(blinkDuration / (blinkTimes * 2));
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(blinkDuration / (blinkTimes * 2));
        }

        isBlinking = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true; // El jugador está en contacto con el enemigo
            Debug.Log("Jugador en contacto con el enemigo.");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false; // El jugador ya no está en contacto
            Debug.Log("Jugador salió del contacto con el enemigo.");
        }
    }
}