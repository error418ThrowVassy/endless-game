using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Tracks whether the game is playing or over, increases score over time,
// shows score + highscore on screen and saves the highscore with PlayerPrefs.
public class GameManager : MonoBehaviour
{
    enum GameState { Playing, GameOver }

    const string HighscoreKey = "Highscore";

    [Header("Settings")]
    [SerializeField] float scorePerSecond = 1f;
    [SerializeField] float restartDelay = 2f;

    [Header("UI")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text highscoreText;

    GameState gameState;
    float score;
    int highscore;

    public bool IsGameOver => gameState == GameState.GameOver;

    void Start()
    {
        score = 0f;
        gameState = GameState.Playing;

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

        // Save a new highscore
        if (finalScore > highscore)
        {
            highscore = finalScore;
            PlayerPrefs.SetInt(HighscoreKey, highscore);
            PlayerPrefs.Save();
            highscoreText.text = $"NEW BEST: {highscore}!";
        }

        Debug.Log($"Game Over. Score: {finalScore}");
        Invoke(nameof(Restart), restartDelay);
    }

    void UpdateUI()
    {
        scoreText.text = $"Score: {Mathf.FloorToInt(score)}";
        highscoreText.text = $"Best: {highscore}";
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}