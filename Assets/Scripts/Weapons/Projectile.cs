using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int damage; // Da�o infligido por el proyectil

    // Configurar el da�o del proyectil desde el arma equipada
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
            // Destruir el proyectil si impacta con un obst�culo
            Debug.Log("Proyectil impact� con un obst�culo.");
            Destroy(gameObject);
        }
        else
        {
            // Opcional: detectar otros tipos de colisi�n si es necesario
            Debug.Log($"Proyectil impact� con: {collision.gameObject.name}");
        }
    }
}



