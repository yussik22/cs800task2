using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject gameOverPanel;

    private int score = 0;
    private bool gameOver = false;


    private void Awake()
    {
        instance = this;

        Time.timeScale = 1f;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        UpdateScoreText();
    }


    // =============================
    // EVENT SUBSCRIPTION
    // =============================

    private void OnEnable()
    {
        Health.OnEnemyDied += EnemyDied;
    }


    private void OnDisable()
    {
        Health.OnEnemyDied -= EnemyDied;
    }


    // Викликається автоматично,
    // коли Enemy помирає
    private void EnemyDied()
    {
        AddScore(1);
    }


    // =============================
    // SCORE
    // =============================

    public void AddScore(int amount)
    {
        if (gameOver)
            return;

        score += amount;

        UpdateScoreText();
    }


    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text =
                "Kills: " + score;
        }
    }


    // =============================
    // GAME OVER
    // =============================

    public void GameOver()
    {
        if (gameOver)
            return;

        gameOver = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }


    // =============================
    // RESTART
    // =============================

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}