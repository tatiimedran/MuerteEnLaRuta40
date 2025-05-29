using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private int damage;
    private Vector2 direction;
    public float speed = 6f; // Projectile speed
    public float lifetime = 5f; // Time the projectile stays in the scene

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("The projectile prefab has no Rigidbody2D assigned.");
            return;
        }

        rb.linearVelocity = direction * speed; // Physics-based movement
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Improves collision detection

        Destroy(gameObject, lifetime); // Destroy after its lifetime ends
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
        Debug.Log($"Projectile damage set: {damage}");
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;
        Debug.Log($"Direction set: {direction}");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Projectile hit: {collision.gameObject.name}, Tag: {collision.tag}, Layer: {collision.gameObject.layer}");

        if (collision.CompareTag("Player"))
        {
            PlayerHealth player = collision.GetComponent<PlayerHealth>();

            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log($"Hit the player. Damage applied: {damage}");
            }
            else
            {
                Debug.LogError("The object with tag 'Player' does not have the PlayerHealth component.");
            }

            Destroy(gameObject);
        }
        else if (collision.CompareTag("Obstacle"))
        {
            Debug.Log($"Projectile hit an obstacle: {collision.gameObject.name}");
            Destroy(gameObject);
        }
    }
}
