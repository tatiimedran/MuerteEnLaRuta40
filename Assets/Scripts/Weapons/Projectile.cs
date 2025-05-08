using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int damage; // Daño infligido por el proyectil

    // Configurar el daño del proyectil desde el arma equipada
    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detectar impacto con enemigos
        if (collision.CompareTag("Enemy"))
        {
            EnemyBehavior enemy = collision.GetComponent<EnemyBehavior>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); 
            }

            // Destruir el proyectil tras impactar
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Obstacle"))
        {
            // Destruir el proyectil si impacta con un obstáculo
            Debug.Log("Proyectil impactó con un obstáculo.");
            Destroy(gameObject);
        }
        else
        {
            // Opcional: detectar otros tipos de colisión si es necesario
            Debug.Log($"Proyectil impactó con: {collision.gameObject.name}");
        }
    }
}



