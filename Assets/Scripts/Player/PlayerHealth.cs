using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public HealthBar healthBar; // Health bar in the UI
    public int maxHealth = 100;
    private int currentHealth;

    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer to control the color
    private bool isBlinking = false; // Controls if the player is already blinking

    // Public property to access current health (read-only)
    public int CurrentHealth
    {
        get { return currentHealth; }
    }

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth); // Set the bar to max at start

        // Get the player's SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on the player.");
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;

        healthBar.SetHealth(currentHealth);
        Debug.Log($"Player took damage: {damage}. Remaining health: {currentHealth}");

        if (!isBlinking)
        {
            StartCoroutine(BlinkEffect(0.2f, 3));
        }

        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    void HandleDeath()
    {
        Debug.Log("Player has died!");
        GetComponent<Animator>().SetTrigger("Die"); // Trigger death animation
        this.enabled = false; // Disable this script
        GetComponent<CharacterMovement>().enabled = false; // Disable movement
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
        Color originalColor = spriteRenderer.color; // Store the original color
        Color blinkColor = Color.red; // The color the player will blink to

        for (int i = 0; i < blinkTimes; i++)
        {
            spriteRenderer.color = blinkColor; // Change to damage color
            yield return new WaitForSeconds(blinkDuration / 2); // Wait half the blink time
            spriteRenderer.color = originalColor; // Revert to original color
            yield return new WaitForSeconds(blinkDuration / 2); // Wait the other half
        }

        isBlinking = false;
    }
}
