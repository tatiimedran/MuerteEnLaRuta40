using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int damage;

    // Set the projectile's damage from the equipped weapon
    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detect collision with enemies
        if (collision.CompareTag("Enemy"))
        {
            EnemyBehavior enemy = collision.GetComponent<EnemyBehavior>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        // Detect collision with destructible objects
        else if (collision.CompareTag("Destructible"))
        {
            DestructibleObject destructible = collision.GetComponent<DestructibleObject>();
            if (destructible != null)
            {
                destructible.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        // Detect collision with obstacles
        else if (collision.CompareTag("Obstacle"))
        {
            Debug.Log("Projectile hit an obstacle.");
            Destroy(gameObject);
        }
    }
}
