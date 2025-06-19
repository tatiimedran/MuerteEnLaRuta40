using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public Button retryButton;
    public Button returnToMenuButton; 

    private void Start()
    {
        retryButton.onClick.AddListener(OnRetryClicked);
        returnToMenuButton.onClick.AddListener(OnReturnToMenuClicked); 
    }

    void OnRetryClicked()
    {
        GameManager.Instance.Retry();
    }

    void OnReturnToMenuClicked()
    {
        GameManager.Instance.ReturnToMenu(); 
    }
}
