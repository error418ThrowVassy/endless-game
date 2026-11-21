using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Tracks whether the game is playing or over, increases score over time,
// shows score + highscore, saves the highscore and shows a Game Over screen.
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

    GameState gameState;
    float score;
    int highscore;

    public bool IsGameOver => gameState == GameState.GameOver;

    void Start()
    {
        score = 0f;
        gameState = GameState.Playing;
        gameOverPanel.SetActive(false);

        // Load the saved highscore (0 if none exists yet)
        highscore = PlayerPrefs.GetInt(HighscoreKey, 0);
        UpdateUI();
    }

    void Update()
    {
        if (gameState != GameState.Playing) return;

        score += scorePerSecond * Time.deltaTime;
        UpdateUI();
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}