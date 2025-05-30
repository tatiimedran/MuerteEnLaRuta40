using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private int damage;
    private Vector2 direction;
    public float speed = 6f; // Projectile speed
    public float lifetime = 3f; // Time before destruction

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Check if the projectile has a Rigidbody2D assigned
        if (rb == null)
        {
            Debug.LogError("The projectile prefab has no Rigidbody2D assigned.");
            return;
        }

        //Improves collision detection
        rb.linearVelocity = direction * speed;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Destroy the projectile after its lifetime ends
        Destroy(gameObject, lifetime);
    }

    // Assign damage value to the projectile
    public void SetDamage(int damage)
    {
        this.damage = damage;
        Debug.Log($"Projectile damage set: {damage}");
    }

    // Assign direction to the projectile
    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;
    }

    // Handle projectile collisions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage); // HealthBar updates automatically
            }

            Destroy(gameObject);
        }
        else // Any other collision destroys the projectile
        {
            Debug.Log($"Projectile hit: {collision.gameObject.name}");
            Destroy(gameObject);
        }
    }
}
