using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(PolygonCollider2D))]
public class EnemyBehavior : MonoBehaviour
{
    public EnemyType enemyType;
    private int currentHealth;

    private NavMeshAgent agent;
    private Animator enemyAnimator;
    private PolygonCollider2D enemyCollider;
    private SpriteRenderer spriteRenderer;
    private EnemyVision vision;

    private Transform player;
    private PlayerHealth playerHealth; 
    private bool isBlinking = false;
    private bool isPlayerInRange = false;
    private float lastAttackTime = 0f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        vision = GetComponent<EnemyVision>();
        enemyAnimator = GetComponent<Animator>();
        enemyCollider = GetComponent<PolygonCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentHealth = enemyType.health;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>(); 
        }
        else
        {
            Debug.LogError("No object with the tag 'Player' was found.");
        }

        agent.speed = enemyType.speed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }
    private void ShootProjectile(Vector2 direction)
    {
        if (enemyType.projectilePrefab != null)
        {
            GameObject projectile = Instantiate(enemyType.projectilePrefab, transform.position, Quaternion.identity);
            EnemyProjectile projectileScript = projectile.GetComponent<EnemyProjectile>();

            if (projectileScript != null)
            {
                projectileScript.SetDirection(direction);
                projectileScript.SetDamage((int)enemyType.rangedDamage);
            }
            else
            {
                Debug.LogError("EnemyProjectile script not found on projectile prefab.");
            }
        }
        else
        {
            Debug.LogWarning("No projectile prefab assigned to this enemy type.");
        }
    }

    private void Update()
    {
        // Ensure enemy is alive and the player exists
        if (currentHealth > 0 && playerHealth != null && playerHealth.CurrentHealth > 0)
        {
        // Check if the player is within detection range
            isPlayerInRange = vision.CanSeePlayer();

            if (isPlayerInRange)
            {
                agent.SetDestination(player.position);
                enemyAnimator.SetBool("EnemyIsMoving", true);
            }
            else
            {
                agent.ResetPath();
                enemyAnimator.SetBool("EnemyIsMoving", false);
            }

            HandleAttack();
        }
        else
        {
          // Stop enemy movement if it is dead or player no longer exists
            agent.ResetPath();
            enemyAnimator.SetBool("EnemyIsMoving", false);
        }
    }

    private void HandleAttack()
    {
        if (playerHealth == null || playerHealth.CurrentHealth <= 0) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (Time.time < lastAttackTime + enemyType.attackCooldown) return;

        lastAttackTime = Time.time;
        Vector2 attackDirection = (player.position - transform.position).normalized;

        // Actualizar dirección para la animación
        enemyAnimator.SetFloat("MoveHorizontal", attackDirection.x);
        enemyAnimator.SetFloat("MoveVertical", attackDirection.y);

        if (enemyType.canMeleeAttack && enemyType.canRangedAttack) // Enemigo híbrido
        {
            if (distanceToPlayer <= enemyType.meleeAttackRange)
            {
                enemyAnimator.SetTrigger("MeleeAttack");
                playerHealth.TakeDamage((int)enemyType.meleeDamage);
            }
            else if (distanceToPlayer <= enemyType.rangedAttackRange)
            {
                enemyAnimator.SetTrigger("RangedAttack");
                ShootProjectile(attackDirection);
            }
        }
        else if (enemyType.canMeleeAttack) // Solo cuerpo a cuerpo
        {
            if (distanceToPlayer <= enemyType.meleeAttackRange)
            {
                enemyAnimator.SetTrigger("MeleeAttack");
                playerHealth.TakeDamage((int)enemyType.meleeDamage);
            }
        }
        else if (enemyType.canRangedAttack) // Solo a distancia
        {
            if (distanceToPlayer <= enemyType.rangedAttackRange)
            {
                enemyAnimator.SetTrigger("RangedAttack");
                ShootProjectile(attackDirection);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (playerHealth == null || playerHealth.CurrentHealth <= 0)
        {
            Debug.Log("Ignoring damage: the player has died.");
            return;
        }

        Debug.Log($"Enemy hit. Health before damage: {currentHealth}, damage taken: {damage}");
        currentHealth -= damage;
        Debug.Log($"New enemy health after damage: {currentHealth}");
        
        // Apply blinking effect when taking damage
        if (!isBlinking)
        {
            StartCoroutine(BlinkEffect(0.2f, 3));
        }

        // Check if enemy has died and handle its death sequence
        if (currentHealth <= 0)
        {
            Debug.Log("The enemy has died.");
            Die();
        }
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

        spriteRenderer.color = originalColor;
        isBlinking = false;
    }

    private void Die()
    {
        if (!this.enabled) return;

        enemyAnimator.SetTrigger("Death"); // Activar animación de muerte
        enemyCollider.enabled = false;
        agent.isStopped = true;
        this.enabled = false;

        // Si el enemigo tiene un efecto de muerte asignado, instanciarlo y eliminar el sprite
        if (enemyType.deathEffectPrefab != null)
        {
            spriteRenderer.enabled = false; // Ocultar el sprite para evitar interferencias con partículas
            Instantiate(enemyType.deathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject); // Eliminar enemigo inmediatamente
        }
        else
        {
            StartCoroutine(DestroyAfterAnimation()); // Enemigos sin efecto esperan su animación
        }
    }

    private IEnumerator DestroyAfterAnimation()
    {
        float deathAnimationDuration = enemyAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(deathAnimationDuration + 2f); // Espera la animación + 2 segundos
        Destroy(gameObject);
    }


}