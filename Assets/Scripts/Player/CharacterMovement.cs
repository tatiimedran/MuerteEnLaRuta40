using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    public float speed = 5f;
    public float sprintSpeed = 10f; // Velocidad al correr (con Shift)
    private Rigidbody2D rb;
    private Vector2 direction;
    private Animator animator;

    private int weaponType = 0; // 0: cuchillo, 1: pistola
    public int attackDamage = 10; // Daño por ataque
    public float attackRange = 1f; // Rango de detección del ataque

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Captura la dirección del movimiento
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        direction = new Vector2(moveHorizontal, moveVertical).normalized;

        // Actualizar los parámetros del Animator
        if (direction != Vector2.zero)
        {
            animator.SetFloat("MoveHorizontal", moveHorizontal);
            animator.SetFloat("MoveVertical", moveVertical);
            animator.SetBool("IsMoving", true);
        }
        else
        {
            float lastMoveHorizontal = animator.GetFloat("MoveHorizontal");
            float lastMoveVertical = animator.GetFloat("MoveVertical");
            animator.SetFloat("MoveHorizontal", lastMoveHorizontal);
            animator.SetFloat("MoveVertical", lastMoveVertical);
            animator.SetBool("IsMoving", false);
        }

        // Cambiar arma equipada
        if (Input.GetKeyDown(KeyCode.Alpha1)) // Cambiar a cuchillo
        {
            weaponType = 0;
            Debug.Log("Cuchillo seleccionado");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // Cambiar a pistola
        {
            weaponType = 1;
            Debug.Log("Pistola seleccionada");
        }

        // Detectar ataque dependiendo del arma equipada
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (weaponType == 0)
            {
                animator.SetTrigger("AttackKnife"); // Disparar animación de cuchillo
            }
            else if (weaponType == 1)
            {
                animator.SetTrigger("AttackGun"); // Disparar animación de pistola
            }

            ApplyDamageToEnemy(); // Aplicar daño a los enemigos cercanos
        }
    }

    void FixedUpdate()
    {
        // Solo aplica movimiento si no está en estado de ataque
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsTag("AttackKnife") && !stateInfo.IsTag("AttackGun"))
        {
            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : speed;
            rb.linearVelocity = direction * currentSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero; // Detener movimiento durante el ataque
        }
    }

    private void ApplyDamageToEnemy()
    {
        Debug.Log("Aplicando daño a los enemigos cercanos...");

        // Detectar enemigos cercanos en el rango de ataque
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (enemyCollider.CompareTag("Enemy"))
            {
                EnemyBehavior enemy = enemyCollider.GetComponent<EnemyBehavior>();
                if (enemy != null)
                {
                    enemy.TakeDamage(attackDamage); // Quitar vida al enemigo
                    Debug.Log($"Enemigo golpeado: {enemy.enemyType.enemyName}, salud restante: {enemy.enemyType.health}");
                }
            }
        }
    }

    // Dibuja el área de detección en la ventana de escena (opcional)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}