using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                // Solo recoger el botiquín si la vida **NO** está al máximo
                if (playerHealth.CurrentHealth < playerHealth.maxHealth)
                {
                    playerHealth.SetMaxHealth(playerHealth.maxHealth); // Restaurar salud al máximo
                    Debug.Log("¡El jugador ha recogido el botiquín!");
                    Destroy(gameObject); // Eliminar el objeto tras ser recogido
                }
                else
                {
                    Debug.Log("El jugador ya tiene la vida al máximo y no puede recoger el botiquín.");
                }
            }
        }
    }
}
