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
            Debug.LogError("No object with the tag 'Player' was found.");
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

            HandleAttack(); // The enemy can attack while moving.
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

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Debug.Log($"Distance to Player: {distanceToPlayer}, Detection Range: {enemyType.detectionRange}");

        if (Time.time >= lastAttackTime + enemyType.attackCooldown)
        {
            lastAttackTime = Time.time;
            Vector2 attackDirection = (player.position - transform.position).normalized;

            enemyAnimator.SetFloat("MoveHorizontal", attackDirection.x);
            enemyAnimator.SetFloat("MoveVertical", attackDirection.y);

            if (enemyType.canMeleeAttack && distanceToPlayer <= enemyType.attackRange)
            {
                enemyAnimator.SetTrigger("MeleeAttack");
                player.GetComponent<PlayerHealth>().TakeDamage((int)enemyType.meleeDamage);
                Debug.Log($"Melee attack executed correctly. Damage: {enemyType.meleeDamage}");
            }
            else if (enemyType.canRangedAttack && isPlayerInRange) //  shoot even if you are moving
            {
                Debug.Log("Attempting a long-distance attack...");

                if (enemyType.projectilePrefab != null)
                {
                    enemyAnimator.SetTrigger("RangedAttack");

                    GameObject projectile = Instantiate(enemyType.projectilePrefab, transform.position, Quaternion.identity);
                    EnemyProjectile projectileScript = projectile.GetComponent<EnemyProjectile>();

                    if (projectileScript != null)
                    {
                        projectileScript.SetDirection(attackDirection);
                        projectileScript.SetDamage((int)enemyType.rangedDamage);
                        Debug.Log($"Projectile instantiated with damage: {enemyType.rangedDamage}");
                    }
                    else
                    {
                        Debug.LogError("The projectile prefab does not have the EnemyProjectile script.");
                    }
                }
                else
                {
                    Debug.LogWarning("Attempting a ranged attack, but there is no assigned projectile prefab..");
                }
            }
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

        //Only apply damage if the attack is melee and the player is nearby.
        if (isMeleeAttack && distanceToPlayer > enemyType.attackRange)
        {
            Debug.Log("Ignoring damage from melee attack: the player is too far away.");
            return;
        }

        Debug.Log($"Enemy hit. Health before damage: {currentHealth}, damage taken: {damage}");
        currentHealth -= damage;
        Debug.Log($"New life of the enemy after the damage: {currentHealth}");

        if (!isBlinking)
        {
            StartCoroutine(BlinkEffect(0.2f, 3));
        }

        if (currentHealth <= 0)
        {
            Debug.Log("The enemy has died.");
            Die();
        }
    }

    private void Die()
    {
        if (!this.enabled) return; //Prevents multiple executions
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