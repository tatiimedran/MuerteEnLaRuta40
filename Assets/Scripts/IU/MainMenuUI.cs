using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject startScreen;
    public GameObject menuButtons;
    public GameObject controlsPanel;

    private bool gameStarted = false;

    void Start()
    {
        startScreen.SetActive(true);
        menuButtons.SetActive(false);
        controlsPanel.SetActive(false);
    }

    void Update()
    {
        if (!gameStarted && Input.GetMouseButtonDown(0))
        {
            ShowMainMenu();
        }
    }

    void ShowMainMenu()
    {
        startScreen.SetActive(false);
        menuButtons.SetActive(true);
        gameStarted = true;
    }

    public void PlayButton()
    {
        SceneManager.LoadScene("Main Scene"); 
    }

    public void ExitButton()
    {
        Application.Quit();
        Debug.Log("The player left the game.");
    }

    public void ControlsButton()
    {
        menuButtons.SetActive(false);
        controlsPanel.SetActive(true);
    }

    public void CloseControls()
    {
        controlsPanel.SetActive(false);
        menuButtons.SetActive(true);
    }
}
