using UnityEngine;
using System.Collections; // Necesario para IEnumerator

public class PlayerHealth : MonoBehaviour
{
    public HealthBar healthBar; // Barra de vida en la UI
    public int maxHealth = 100;
    private int currentHealth;

    private SpriteRenderer spriteRenderer; // Referencia al SpriteRenderer para controlar el color
    private bool isBlinking = false; // Controla si el jugador ya está parpadeando

    // Propiedad pública para acceder a la vida actual (de solo lectura)
    public int CurrentHealth
    {
        get { return currentHealth; }
    }

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth); // Configura la barra al máximo al inicio

        // Obtener el SpriteRenderer del jugador
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("No se encontró un SpriteRenderer en el jugador.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Espacio presionado");
            TakeDamage(10);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
            currentHealth = 0;

        healthBar.SetHealth(currentHealth);

        // Activar el parpadeo cuando el jugador pierde vida
        if (!isBlinking)
        {
            StartCoroutine(BlinkEffect(0.2f, 3)); // Configura la duración y número de parpadeos
        }

        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    void HandleDeath()
    {
        Debug.Log("Player has died!");
        GetComponent<Animator>().SetTrigger("Die"); // Activar animación de muerte
        this.enabled = false; // Desactiva este script
        GetComponent<CharacterMovement>().enabled = false; // Desactiva el movimiento
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        healthBar.SetHealth(currentHealth);
    }

    private IEnumerator BlinkEffect(float blinkDuration, int blinkTimes)
    {
        isBlinking = true;
        Color originalColor = spriteRenderer.color; // Almacenar el color original
        Color blinkColor = Color.red; // El color al que el jugador parpadeará

        for (int i = 0; i < blinkTimes; i++)
        {
            spriteRenderer.color = blinkColor; // Cambiar al color de daño
            yield return new WaitForSeconds(blinkDuration / 2); // Esperar la mitad del tiempo de parpadeo
            spriteRenderer.color = originalColor; // Volver al color original
            yield return new WaitForSeconds(blinkDuration / 2); // Esperar la otra mitad
        }

        isBlinking = false;
    }
}