using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Elements")]
    public Text scoreText; // Nếu bạn dùng TextMeshPro, thay đổi thành TMPro.TextMeshProUGUI
    public Image[] heartIcons;
    public GameObject gameOverPanel;
    public Text finalScoreText;

    private int score;
    private int lives = 3;
    private bool isGameOver;

    private void Awake()
    {
        // Khởi tạo Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        NewGame();
    }

    public void NewGame()
    {
        score = 0;
        lives = 3;
        isGameOver = false;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        UpdateUI();
        Time.timeScale = 1f; // Chạy game bình thường
        Debug.Log("Game bắt đầu!");
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;

        score += amount;
        UpdateUI();
    }

    public void LoseLife()
    {
        if (isGameOver) return;

        lives--;
        UpdateUI();

        if (lives <= 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f; // Dừng thời gian
        Debug.Log("Game Over!");
        
        if (gameOverPanel != null) 
        {
            gameOverPanel.SetActive(true);
        }
        
        if (finalScoreText != null)
        {
            finalScoreText.text = score.ToString();
        }
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }

        if (heartIcons != null)
        {
            for (int i = 0; i < heartIcons.Length; i++)
            {
                if (i < lives)
                    heartIcons[i].enabled = true;
                else
                    heartIcons[i].enabled = false;
            }
        }
    }

    public void RestartGame()
    {
        // Tải lại Scene hiện tại để bắt đầu lại toàn bộ game (xóa hoa quả cũ, reset điểm...)
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
