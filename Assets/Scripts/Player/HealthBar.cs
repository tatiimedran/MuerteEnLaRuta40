using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public static HealthBar Instance { get; private set; } 

    [SerializeField] private Slider slider;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        PlayerHealth.OnHealthChanged += UpdateHealthBar; // Subscribe to event
    }

    private void OnDisable()
    {
        PlayerHealth.OnHealthChanged -= UpdateHealthBar; // Unsubscribe when disabled
    }

    public void UpdateHealthBar(int health)
    {
        if (slider != null)
        {
            slider.value = health;
        }
    }

    public void SetMaxHealth(int maxHealth)
    {
        if (slider != null)
        {
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
        }
    }
}