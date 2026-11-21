using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Shows the saved highscore and starts the game.
public class MainMenu : MonoBehaviour
{
    [SerializeField] string gameSceneName = "Main Scene";
    [SerializeField] TMP_Text highscoreText;

    void Start()
    {
        highscoreText.text = $"Best: {PlayerPrefs.GetInt("Highscore", 0)}";
    }

    // Called by the Start button
    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}