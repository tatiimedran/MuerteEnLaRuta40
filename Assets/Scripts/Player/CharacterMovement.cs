using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    public float speed = 3f;
    public float sprintSpeed = 4f;
    private Rigidbody2D rb;
    private Vector2 direction;
    private Animator animator;
    private bool isSprinting = false; 

    [Header("Player weapons")]
    public Weapon[] availableWeapons;
    private Weapon equippedWeapon;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        EquipWeapon(0); // Default weapon
    }

    void Update()
    {
       

        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector2 rawInput = new Vector2(moveHorizontal, moveVertical);

        if (rawInput.magnitude < 0.2f)
            rawInput = Vector2.zero;

        direction = rawInput.normalized;

        
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

        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
    }

    void FixedUpdate()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsTag(equippedWeapon.attackAnimation))
        {
            float currentSpeed = isSprinting ? sprintSpeed : speed; 
            rb.linearVelocity = direction * currentSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context) 
    {
        isSprinting = context.ReadValueAsButton();
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            animator.SetTrigger(equippedWeapon.attackAnimation);
            ApplyDamageToEnemy();
            Debug.Log("Ataque ejecutado");
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

    private void ApplyDamageToEnemy()
    {
        if (equippedWeapon.isRanged)
        {
            ShootProjectile();
        }
        else
        {
            float attackRadius = equippedWeapon.attackRange;

            Collider2D[] hitTargets = Physics2D.OverlapCircleAll(transform.position, attackRadius);
            foreach (Collider2D targetCollider in hitTargets)
            {
                if (targetCollider.CompareTag("Enemy"))
                {
                    EnemyBehavior enemy = targetCollider.GetComponent<EnemyBehavior>();
                    if (enemy != null && Vector2.Distance(transform.position, enemy.transform.position) <= attackRadius)
                    {
                        enemy.TakeDamage(equippedWeapon.attackDamage);
                    }
                }
            }

            Vector2 attackDirection = new Vector2(
                animator.GetFloat("LastMoveHorizontal"),
                animator.GetFloat("LastMoveVertical")
            ).normalized;

            Vector2 destructibleOrigin = (Vector2)transform.position + attackDirection * (attackRadius * 0.9f);
            float destructibleRange = attackRadius * 0.9f;

            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(destructibleOrigin, destructibleRange);
            foreach (Collider2D targetCollider in hitObjects)
            {
                if (targetCollider.CompareTag("Destructible"))
                {
                    DestructibleObject destructible = targetCollider.GetComponent<DestructibleObject>();
                    if (destructible != null && Vector2.Distance(destructibleOrigin, destructible.transform.position) <= destructibleRange)
                    {
                        destructible.TakeDamage(equippedWeapon.attackDamage);
                    }
                }
            }
        }
    }

    private void ShootProjectile()
    {
        if (equippedWeapon.projectilePrefab != null)
        {
            GameObject projectile = Instantiate(
                equippedWeapon.projectilePrefab,
                transform.position,
                Quaternion.identity
            );

            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 shootDirection = new Vector2(
                    animator.GetFloat("MoveHorizontal"),
                    animator.GetFloat("MoveVertical")
                ).normalized;

                rb.linearVelocity = shootDirection * equippedWeapon.projectileSpeed; 
            }

            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.SetDamage(equippedWeapon.attackDamage);
            }

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
    public void OnQuickWeaponSwap(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            int currentIndex = System.Array.IndexOf(availableWeapons, equippedWeapon);
            int nextIndex = (currentIndex + 1) % availableWeapons.Length; // Alternar al siguiente arma
            EquipWeapon(nextIndex);
            Debug.Log("Quick weapon swap ejecutado");
        }
    }

}
