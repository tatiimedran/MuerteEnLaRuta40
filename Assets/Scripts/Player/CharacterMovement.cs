using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    public float speed = 3f;
    public float sprintSpeed = 4f;
    private Rigidbody2D rb;
    private Vector2 direction;
    private Animator animator;

    [Header("Player weapons")]
    public Weapon[] availableWeapons; // List of available weapons
    private Weapon equippedWeapon;    // Currently equipped weapon

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // By default, equip the first weapon
        EquipWeapon(0); // Index 0 (Knife)
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            // Fire or melee attack depending on the equipped weapon
            animator.SetTrigger(equippedWeapon.attackAnimation);
            ApplyDamageToEnemy();
        }

        // Capture movement direction
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        direction = new Vector2(moveHorizontal, moveVertical).normalized;

        // Update animations and directions
        if (direction != Vector2.zero)
        {
            animator.SetFloat("MoveHorizontal", direction.x);
            animator.SetFloat("MoveVertical", direction.y);
            animator.SetBool("IsMoving", true);

            animator.SetFloat("LastMoveHorizontal", direction.x);
            animator.SetFloat("LastMoveVertical", direction.y);
        }
        else
        {
            animator.SetBool("IsMoving", false);
            animator.SetFloat("MoveHorizontal", animator.GetFloat("LastMoveHorizontal"));
            animator.SetFloat("MoveVertical", animator.GetFloat("LastMoveVertical"));
        }

        // Switch equipped weapon
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
    }

    void FixedUpdate()
    {
        // Handle movement depending on attack state
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsTag(equippedWeapon.attackAnimation))
        {
            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : speed;
            rb.linearVelocity = direction * currentSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero; // Stop movement during attack
        }
    }

    private void ApplyDamageToEnemy()
    {
        if (equippedWeapon.isRanged)
        {
            ShootProjectile();
        }
        else
        {
            float attackRadius = equippedWeapon.attackRange;

            // 1. Ataque a enemigos (sin cambios)
            Collider2D[] hitTargets = Physics2D.OverlapCircleAll(transform.position, attackRadius);
            foreach (Collider2D targetCollider in hitTargets)
            {
                if (targetCollider.CompareTag("Enemy"))
                {
                    EnemyBehavior enemy = targetCollider.GetComponent<EnemyBehavior>();
                    if (enemy != null)
                    {
                        float distance = Vector2.Distance(transform.position, enemy.transform.position);
                        if (distance <= equippedWeapon.attackRange)
                        {
                            enemy.TakeDamage(equippedWeapon.attackDamage);
                        }
                    }
                }
            }

            // 2. Ataque a destructibles (con rango y origen personalizados)
            Vector2 attackDirection = new Vector2(
                animator.GetFloat("LastMoveHorizontal"),
                animator.GetFloat("LastMoveVertical")
            ).normalized;

            Vector2 destructibleOrigin = (Vector2)transform.position + attackDirection * (attackRadius * 0.7f);
            float destructibleRange = attackRadius * 0.5f;

            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(destructibleOrigin, destructibleRange);
            foreach (Collider2D targetCollider in hitObjects)
            {
                if (targetCollider.CompareTag("Destructible"))
                {
                    DestructibleObject destructible = targetCollider.GetComponent<DestructibleObject>();
                    if (destructible != null)
                    {
                        float distance = Vector2.Distance(destructibleOrigin, destructible.transform.position);
                        if (distance <= destructibleRange)
                        {
                            destructible.TakeDamage(equippedWeapon.attackDamage);
                        }
                    }
                }
            }
        }
    }

    private void ShootProjectile()
    {
        if (equippedWeapon.projectilePrefab != null)
        {
            // Instantiate the projectile
            GameObject projectile = Instantiate(
                equippedWeapon.projectilePrefab,   // Projectile prefab
                transform.position,               // Initial position (player)
                Quaternion.identity               // No initial rotation
            );

            // Set projectile direction and speed
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Get direction from the Animator
                Vector2 shootDirection = new Vector2(
                    animator.GetFloat("MoveHorizontal"),
                    animator.GetFloat("MoveVertical")
                ).normalized;

                rb.linearVelocity = shootDirection * equippedWeapon.projectileSpeed; // Projectile speed
            }

            // Transfer weapon damage to projectile
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.SetDamage(equippedWeapon.attackDamage);
            }

            // Play attack sound if available
            if (equippedWeapon.attackSound != null)
            {
                AudioSource.PlayClipAtPoint(equippedWeapon.attackSound, transform.position);
            }

            Debug.Log($"Projectile fired with: {equippedWeapon.weaponName}, damage = {equippedWeapon.attackDamage}");
        }
        else
        {
            Debug.LogWarning("No projectile prefab assigned to this weapon.");
        }
    }

    private void EquipWeapon(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex < availableWeapons.Length)
        {
            equippedWeapon = availableWeapons[weaponIndex];
            Debug.Log($"Weapon equipped: {equippedWeapon.weaponName}");
        }
        else
        {
            Debug.LogWarning("Invalid weapon index");
        }
    }
}
