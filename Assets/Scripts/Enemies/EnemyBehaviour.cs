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
    private bool isBlinking = false;
    private bool isPlayerInRange = false;
    private float lastAttackTime = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        vision = GetComponent<EnemyVision>();
        enemyAnimator = GetComponent<Animator>();
        enemyCollider = GetComponent<PolygonCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentHealth = enemyType.health;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("No se encontró un objeto con la etiqueta 'Player'.");
        }

        agent.speed = enemyType.speed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        if (currentHealth > 0 && player != null && player.GetComponent<PlayerHealth>().CurrentHealth > 0)
        {
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

            Vector2 moveDirection = new Vector2(agent.velocity.x, agent.velocity.y);
            if (moveDirection != Vector2.zero)
            {
                enemyAnimator.SetFloat("MoveHorizontal", moveDirection.x);
                enemyAnimator.SetFloat("MoveVertical", moveDirection.y);
            }

            HandleAttack();
        }
        else
        {
            agent.ResetPath();
            enemyAnimator.SetBool("EnemyIsMoving", false);
        }
    }

    void HandleAttack()
    {
        if (player == null || player.GetComponent<PlayerHealth>().CurrentHealth <= 0) return;

        float distanceToPlayer = Vector2.Distance(agent.nextPosition, player.position);

        //Bloquear ataques cuerpo a cuerpo si el jugador está fuera del rango exacto
        if (enemyType.canMeleeAttack && distanceToPlayer > enemyType.attackRange)
        {
            Debug.Log("Bloqueando ataque cuerpo a cuerpo: el jugador está demasiado lejos.");
            return;
        }

        if (Time.time >= lastAttackTime + enemyType.attackCooldown && distanceToPlayer <= enemyType.attackRange)
        {
            lastAttackTime = Time.time;
            Vector2 attackDirection = (player.position - agent.nextPosition).normalized;

            enemyAnimator.SetFloat("MoveHorizontal", attackDirection.x);
            enemyAnimator.SetFloat("MoveVertical", attackDirection.y);

            if (enemyType.canMeleeAttack)
            {
                enemyAnimator.SetTrigger("MeleeAttack");
                player.GetComponent<PlayerHealth>().TakeDamage((int)enemyType.meleeDamage);
                Debug.Log($"Ataque cuerpo a cuerpo ejecutado correctamente. Daño: {enemyType.meleeDamage}");
            }
            else if (enemyType.canRangedAttack && distanceToPlayer <= enemyType.detectionRange)
            {
                enemyAnimator.SetTrigger("RangedAttack");

                if (enemyType.projectilePrefab != null)
                {
                    Instantiate(enemyType.projectilePrefab, transform.position, Quaternion.identity);
                }

                player.GetComponent<PlayerHealth>().TakeDamage((int)enemyType.rangedDamage);
                Debug.Log($"Ataque a distancia ejecutado correctamente. Daño: {enemyType.rangedDamage}");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            if (player == null || player.GetComponent<PlayerHealth>().CurrentHealth <= 0)
            {
                Debug.Log("Ignorando daño: el jugador ha muerto.");
                return;
            }

            int damage = (int)enemyType.rangedDamage;
            TakeDamage(damage);
            Destroy(other.gameObject);
            Debug.Log($"El enemigo recibió daño por impacto de proyectil. Daño aplicado: {damage}");
        }
    }

    public void TakeDamage(int damage, bool isMeleeAttack = false)
    {
        if (player == null || player.GetComponent<PlayerHealth>().CurrentHealth <= 0)
        {
            Debug.Log("Ignorando daño: el jugador ha muerto.");
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        //Solo aplicar daño si el ataque es cuerpo a cuerpo y el jugador está cerca
        if (isMeleeAttack && distanceToPlayer > enemyType.attackRange)
        {
            Debug.Log("Ignorando daño por ataque cuerpo a cuerpo: el jugador está demasiado lejos.");
            return;
        }

        Debug.Log($"Enemigo impactado. Vida antes del daño: {currentHealth}, daño recibido: {damage}");
        currentHealth -= damage;
        Debug.Log($"Nueva vida del enemigo después del daño: {currentHealth}");

        if (!isBlinking)
        {
            StartCoroutine(BlinkEffect(0.2f, 3));
        }

        if (currentHealth <= 0)
        {
            Debug.Log("El enemigo ha muerto.");
            Die();
        }
    }

    private void Die()
    {
        if (!this.enabled) return; //Evita múltiples ejecuciones
        enemyAnimator.SetTrigger("Death");
        enemyCollider.enabled = false;
        agent.isStopped = true;
        this.enabled = false;
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

        spriteRenderer.color = originalColor;
        isBlinking = false;
    }
}