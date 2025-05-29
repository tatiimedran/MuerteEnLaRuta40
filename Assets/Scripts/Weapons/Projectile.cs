using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int damage; // Damage dealt by the projectile

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

            // Destroy the projectile after hitting
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Obstacle"))
        {
            // Destroy the projectile if it hits an obstacle
            Debug.Log("Projectile hit an obstacle.");
            Destroy(gameObject);
        }
    }
}
