using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Slider healthSlider; // Esta es la barra de vida
    public PlayerHealth playerHealth; // Este es el script que maneja la salud del jugador

    private void Start()
    {
        // Cuando cambia la salud, se actualiza la barra
        if (playerHealth != null)
        {
            PlayerHealth.OnHealthChanged += UpdateHealthBar;
            healthSlider.value = 1f; // Comienza con vida llena
        }
    }

    private void UpdateHealthBar(int currentHealth)
    {
        // Calcula el porcentaje de vida (por ejemplo 70 / 100 = 0.7)
        float normalizedHealth = (float)currentHealth / playerHealth.maxHealth;
        healthSlider.value = normalizedHealth;
    }

    private void OnDestroy()
    {
        // Deja de escuchar cuando se destruye
        PlayerHealth.OnHealthChanged -= UpdateHealthBar;
    }
}
