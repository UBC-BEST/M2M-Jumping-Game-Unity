using UnityEngine;
using UnityEngine.UI;
using TMPro; // If using TextMeshPro, otherwise use UnityEngine.UI.Text

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText; // Or Text if not using TMP
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    
    void Start()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitClicked);
        }
    }
    
    void OnEnable()
    {
        // Update score when UI becomes active
        if (scoreText != null)
        {
            // You can integrate your score system here
            // scoreText.text = "Score: " + ScoreManager.Instance.GetScore();
            scoreText.text = "Game Over!";
        }
    }
    
    void OnRestartClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }
    
    void OnQuitClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitGame();
        }
    }
}