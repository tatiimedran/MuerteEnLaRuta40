using UnityEngine;
using System;
using System.Collections; 

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    private SpriteRenderer spriteRenderer;  // Reference to the SpriteRenderer to control the color

    private bool isBlinking = false;  // Controls if the player is already blinking 

    public static event Action<int> OnHealthChanged;

    public int CurrentHealth => currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on the player.");
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); // Ensure health does not go negative

        Debug.Log($"Player took damage: {damage}. Current health: {currentHealth}"); 

        OnHealthChanged?.Invoke(currentHealth); // Notify HealthBar

        if (!isBlinking)
        {
            StartCoroutine(BlinkEffect(0.2f, 3)); // Blink effect when damaged
        }

        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    private IEnumerator BlinkEffect(float blinkDuration, int blinkTimes)
    {
        isBlinking = true;
        Color originalColor = spriteRenderer.color; // Store the original color 
        Color blinkColor = Color.red;  // The color the player will blink to 

        for (int i = 0; i < blinkTimes; i++)
        {
            spriteRenderer.color = blinkColor; // Change to damage color 
            yield return new WaitForSeconds(blinkDuration / 3); // Wait half the blink time
            spriteRenderer.color = originalColor; // Revert to original color 
            yield return new WaitForSeconds(blinkDuration / 3); // Wait the other half
        }

        isBlinking = false;
    }
    private void HandleDeath()
    {
        Debug.Log("Player has died!");
        GetComponent<Animator>().SetTrigger("Die"); // Trigger death animation
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero; // Detiene el movimiento completamente
        this.enabled = false; // Disable PlayerHealth script
        GetComponent<CharacterMovement>().enabled = false; // Disable movement
    }
}

