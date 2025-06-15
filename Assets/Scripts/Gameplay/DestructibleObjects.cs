using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class DestructibleObject : MonoBehaviour
{
    public int health = 1;
    public float fadeDuration = 1f; // Tiempo total de desvanecimiento

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            StartCoroutine(FadeAndDestroy());
        }
    }

    private IEnumerator FadeAndDestroy()
    {
        float elapsed = 0f;
        Color originalColor = spriteRenderer.color;

        while (elapsed < fadeDuration)
        {
            float blink = Mathf.PingPong(elapsed * 10f, 1f); // parpadeo entre visible e invisible
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration); // desvanecimiento progresivo

            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, blink * alpha);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}

