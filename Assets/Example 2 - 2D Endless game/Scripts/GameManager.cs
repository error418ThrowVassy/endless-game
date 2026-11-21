using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Tracks the game state, score and highscore, and handles
// Game Over, Pause, Restart and going back to the menu.
public class GameManager : MonoBehaviour
{
    enum GameState { Playing, GameOver }

    const string HighscoreKey = "Highscore";

    [Header("Settings")]
    [SerializeField] float scorePerSecond = 1f;
    [Tooltip("Delay before the Game Over screen appears, so the death animation is visible.")]
    [SerializeField] float gameOverDelay = 1f;

    [Header("UI")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text highscoreText;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] TMP_Text finalScoreText;
    [SerializeField] GameObject pausePanel;

    GameState gameState;
    float score;
    int highscore;
    bool isPaused;

    public bool IsGameOver => gameState == GameState.GameOver;
    public bool IsPaused => isPaused;

    void Start()
    {
        score = 0f;
        gameState = GameState.Playing;
        isPaused = false;
        Time.timeScale = 1f;

        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);

        // Load the saved highscore (0 if none exists yet)
        highscore = PlayerPrefs.GetInt(HighscoreKey, 0);
        UpdateUI();
    }

    void Update()
    {
        // Pause / unpause with Escape or P (only while playing)
        if (gameState == GameState.Playing &&
            (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)))
        {
            TogglePause();
        }

        if (gameState != GameState.Playing || isPaused) return;

        score += scorePerSecond * Time.deltaTime;
        UpdateUI();
    }

    // Called by the Escape key and the "Resume" button
    public void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);

        // timeScale 0 freezes physics and everything that uses Time.deltaTime
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void OnPlayerCollidedWithObstacle()
    {
        if (gameState == GameState.GameOver) return;

        gameState = GameState.GameOver;
        int finalScore = Mathf.FloorToInt(score);

        if (finalScore > highscore)
        {
            highscore = finalScore;
            PlayerPrefs.SetInt(HighscoreKey, highscore);
            PlayerPrefs.Save();
            finalScoreText.text = $"NEW HIGHSCORE: {finalScore}!";
        }
        else
        {
            finalScoreText.text = $"Score: {finalScore}\nBest: {highscore}";
        }

        UpdateUI();
        Invoke(nameof(ShowGameOver), gameOverDelay);
    }

    void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
    }

    void UpdateUI()
    {
        scoreText.text = $"Score: {Mathf.FloorToInt(score)}";
        highscoreText.text = $"Best: {highscore}";
    }

    // Called by the "Play Again" button
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Called by the "Menu" buttons
    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}