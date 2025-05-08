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
        if (Input.GetKeyDown(KeyCode.F))
        {
            // Disparar o atacar según el arma equipada
            animator.SetTrigger(equippedWeapon.attackAnimation);
            ApplyDamageToEnemy();
        }

        // Capturar la dirección del movimiento
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        direction = new Vector2(moveHorizontal, moveVertical).normalized;

        // Actualizar animaciones y direcciones
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

        // Cambiar arma equipada
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
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
        Debug.Log($"El arma equipada ({equippedWeapon.weaponName}) es a distancia: {equippedWeapon.isRanged}");

        if (equippedWeapon.isRanged)
        {
            ShootProjectile();
        }
        else
        {
            Debug.Log($"Atacando con: {equippedWeapon.weaponName}");

            //para verificar el alcance
            RaycastHit2D hitEnemy = Physics2D.Raycast(transform.position, direction, equippedWeapon.attackRange);

            if (hitEnemy.collider != null && hitEnemy.collider.CompareTag("Enemy"))
            {
                EnemyBehavior enemy = hitEnemy.collider.GetComponent<EnemyBehavior>();
                if (enemy != null)
                {
                    float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);

                    if (distanceToEnemy <= equippedWeapon.attackRange) //Solo aplicar daño si está dentro del rango real
                    {
                        enemy.TakeDamage(equippedWeapon.attackDamage, true);
                        Debug.Log($"Enemigo golpeado correctamente: {enemy.enemyType.enemyName}, Salud restante: {enemy.enemyType.health}");
                    }
                    else
                    {
                        Debug.Log("Ataque cuerpo a cuerpo ignorado: el enemigo está demasiado lejos.");
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
                equippedWeapon.projectilePrefab,   // Prefab del proyectil
                transform.position,               // Posición inicial (jugador)
                Quaternion.identity               // Sin rotación inicial
            );

            // Configurar la dirección y velocidad del proyectil
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Obtener dirección desde el Animator
                Vector2 shootDirection = new Vector2(
                    animator.GetFloat("MoveHorizontal"),
                    animator.GetFloat("MoveVertical")
                ).normalized;

                rb.linearVelocity = shootDirection * equippedWeapon.projectileSpeed; // Velocidad del proyectil
            }

            // Transferir el daño del arma al proyectil
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.SetDamage(equippedWeapon.attackDamage);
            }

            // Opcional: reproducir sonido de ataque si está configurado
            if (equippedWeapon.attackSound != null)
            {
                AudioSource.PlayClipAtPoint(equippedWeapon.attackSound, transform.position);
            }

            Debug.Log($"Proyectil disparado con: {equippedWeapon.weaponName}, daño = {equippedWeapon.attackDamage}");
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