using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    public float speed = 5f;
    public float sprintSpeed = 10f;
    private Rigidbody2D rb;
    private Vector2 direction;
    private Animator animator;

    [Header("Armas del jugador")]
    public Weapon[] availableWeapons; // Lista de armas disponibles
    private Weapon equippedWeapon;    // Arma actualmente equipada

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Por defecto, equipa la primera arma
        EquipWeapon(0); // �ndice 0 (por ejemplo, Cuchillo)
    }

    void Update()
    {
        // Capturar la direcci�n del movimiento
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        direction = new Vector2(moveHorizontal, moveVertical).normalized;

        // Retener la �ltima direcci�n v�lida para los ataques si el personaje est� quieto
        if (direction != Vector2.zero)
        {
            animator.SetFloat("MoveHorizontal", direction.x);
            animator.SetFloat("MoveVertical", direction.y);
            animator.SetBool("IsMoving", true);

            // Guardar la �ltima direcci�n v�lida
            animator.SetFloat("LastMoveHorizontal", direction.x);
            animator.SetFloat("LastMoveVertical", direction.y);
        }
        else
        {
            animator.SetBool("IsMoving", false);

            // Usar la �ltima direcci�n v�lida para los ataques
            animator.SetFloat("MoveHorizontal", animator.GetFloat("LastMoveHorizontal"));
            animator.SetFloat("MoveVertical", animator.GetFloat("LastMoveVertical"));
        }

        // Cambiar arma equipada seg�n el n�mero presionado
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0); // Cuchillo
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1); // Pistola

        // Detectar ataque
        if (Input.GetKeyDown(KeyCode.F))
        {
            // El Blend Tree usar� la direcci�n almacenada (ya sea actual o �ltima)
            animator.SetTrigger(equippedWeapon.attackAnimation);
            ApplyDamageToEnemy();
        }
    }

    void FixedUpdate()
    {
        // Gestionar movimiento seg�n el estado de ataque
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsTag(equippedWeapon.attackAnimation))
        {
            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : speed;
            rb.linearVelocity = direction * currentSpeed; // Usando linearVelocity
        }
        else
        {
            rb.linearVelocity = Vector2.zero; // Detener movimiento durante el ataque
        }
    }

    private void ApplyDamageToEnemy()
    {
        // Verificar si el arma equipada es a distancia o cuerpo a cuerpo
        if (equippedWeapon.projectilePrefab != null)
        {
            ShootProjectile(); // Disparar proyectil
        }
        else
        {
            // Aplicar da�o cuerpo a cuerpo en un rango cercano
            Debug.Log($"Atacando con: {equippedWeapon.weaponName}");
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, equippedWeapon.attackRange);
            foreach (Collider2D enemyCollider in hitEnemies)
            {
                if (enemyCollider.CompareTag("Enemy"))
                {
                    EnemyBehavior enemy = enemyCollider.GetComponent<EnemyBehavior>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(equippedWeapon.attackDamage);
                        Debug.Log($"Enemigo golpeado: {enemy.enemyType.enemyName}, Salud restante: {enemy.enemyType.health}");
                    }
                }
            }
        }
    }

    private void ShootProjectile()
    {
        if (equippedWeapon.projectilePrefab != null)
        {
            // Instanciar el proyectil
            GameObject projectile = Instantiate(
                equippedWeapon.projectilePrefab, // Prefab del proyectil
                transform.position,             // Posici�n inicial del proyectil
                Quaternion.identity             // Sin rotaci�n inicial
            );

            // Configurar la direcci�n y velocidad del proyectil
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 shootDirection = new Vector2(
                    animator.GetFloat("MoveHorizontal"),
                    animator.GetFloat("MoveVertical")
                ).normalized;

                rb.linearVelocity = shootDirection * equippedWeapon.projectileSpeed; // Velocidad del proyectil
            }

            // Opcional: reproducir sonido de ataque si est� configurado
            if (equippedWeapon.attackSound != null)
            {
                AudioSource.PlayClipAtPoint(equippedWeapon.attackSound, transform.position);
            }

            Debug.Log($"Proyectil disparado con: {equippedWeapon.weaponName}");
        }
        else
        {
            Debug.LogWarning("No hay prefab de proyectil asignado a esta arma.");
        }
    }

    private void EquipWeapon(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex < availableWeapons.Length)
        {
            equippedWeapon = availableWeapons[weaponIndex];
            Debug.Log($"Arma equipada: {equippedWeapon.weaponName}");
        }
        else
        {
            Debug.LogWarning("�ndice de arma inv�lido");
        }
    }
}


