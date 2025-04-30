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
        EquipWeapon(0); // Índice 0 (por ejemplo, Cuchillo)
    }

    void Update()
    {
        // Capturar la dirección del movimiento
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        direction = new Vector2(moveHorizontal, moveVertical).normalized;

        // Retener la última dirección válida para los ataques si el personaje está quieto
        if (direction != Vector2.zero)
        {
            animator.SetFloat("MoveHorizontal", direction.x);
            animator.SetFloat("MoveVertical", direction.y);
            animator.SetBool("IsMoving", true);

            // Guardar la última dirección válida
            animator.SetFloat("LastMoveHorizontal", direction.x);
            animator.SetFloat("LastMoveVertical", direction.y);
        }
        else
        {
            animator.SetBool("IsMoving", false);

            // Usar la última dirección válida para los ataques
            animator.SetFloat("MoveHorizontal", animator.GetFloat("LastMoveHorizontal"));
            animator.SetFloat("MoveVertical", animator.GetFloat("LastMoveVertical"));
        }

        // Cambiar arma equipada según el número presionado
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0); // Cuchillo
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1); // Pistola

        // Detectar ataque
        if (Input.GetKeyDown(KeyCode.F))
        {
            // El Blend Tree usará la dirección almacenada (ya sea actual o última)
            animator.SetTrigger(equippedWeapon.attackAnimation);
            ApplyDamageToEnemy();
        }
    }

    void FixedUpdate()
    {
        // Gestionar movimiento según el estado de ataque
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
            // Aplicar daño cuerpo a cuerpo en un rango cercano
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
                transform.position,             // Posición inicial del proyectil
                Quaternion.identity             // Sin rotación inicial
            );

            // Configurar la dirección y velocidad del proyectil
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 shootDirection = new Vector2(
                    animator.GetFloat("MoveHorizontal"),
                    animator.GetFloat("MoveVertical")
                ).normalized;

                rb.linearVelocity = shootDirection * equippedWeapon.projectileSpeed; // Velocidad del proyectil
            }

            // Opcional: reproducir sonido de ataque si está configurado
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
            Debug.LogWarning("Índice de arma inválido");
        }
    }
}


