using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private int damage;
    private Vector2 direction;
    public float speed = 6f; // Velocidad del proyectil
    public float lifetime = 5f; // Tiempo por el cual el proyectil permanece en escena

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("El prefab del proyectil no tiene un Rigidbody2D asignado.");
            return;
        }

        rb.linearVelocity = direction * speed; // Movimiento con física 
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Mejora la detección de colisión

        Destroy(gameObject, lifetime); // Destruir al completar su ciclo 
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
        Debug.Log($"Daño del proyectil configurado: {damage}");
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;
        Debug.Log($"Dirección establecida: {direction}");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Proyectil impactó con: {collision.gameObject.name}, Tag: {collision.tag}, Layer: {collision.gameObject.layer}");

        if (collision.CompareTag("Player"))
        {
            PlayerHealth player = collision.GetComponent<PlayerHealth>();

            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log($"Impacto en el jugador. Daño aplicado: {damage}");
            }
            else
            {
                Debug.LogError("El objeto con el tag 'Player' no tiene el componente PlayerHealth.");
            }

            Destroy(gameObject);
        }
        else if (collision.CompareTag("Obstacle"))
        {
            Debug.Log($"Proyectil impactó contra un obstáculo: {collision.gameObject.name}");
            Destroy(gameObject);
        }
    }
}