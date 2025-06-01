using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public TextMeshProUGUI scoreText;   // Texto del score
    public TextMeshProUGUI timerText;   // 👈 NUEVO: Texto del temporizador

    private int score = 0;
    private float timer = 0f;           // 👈 NUEVO: tiempo acumulado en segundos

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateScoreUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }

        // Test: barra espaciadora suma puntos
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddScore(10);
        }

        // 👇 NUEVO: actualizar temporizador
        timer += Time.deltaTime;
        UpdateTimerUI();
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timer / 60f);
            int seconds = Mathf.FloorToInt(timer % 60f);
            timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }
    }
}
