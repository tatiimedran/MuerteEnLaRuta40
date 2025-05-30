using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps the GameManager active between scenes
        }
        else
        {
            Destroy(gameObject); // Prevents duplicate instances
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Detect ESC key press
        {
            QuitGame();
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit(); 
    }
}
