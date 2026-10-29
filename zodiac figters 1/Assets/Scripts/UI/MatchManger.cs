using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MatchManager : MonoBehaviour
{
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text resultText;

    private bool matchEnded = false;

    public bool MatchEnded
    { get { return matchEnded; } }

    public void EndMatch(bool playerWon, GameObject loser)
    {
        // Prevent the match from ending more than once
        if (matchEnded)
            return;

        matchEnded = true;

        // playtest log: result, duration and the winner's health (before the loser is destroyed)
        CollectedDataAnalysis.MatchEnd(playerWon);

        // remove the defeated fighter
        if (loser != null)
        {
            Destroy(loser);
        }

        // Stop the fight
        Time.timeScale = 0f;

        // Show result panel
        resultPanel.SetActive(true);

        if (playerWon)
        {
            resultText.text = "YOU WIN!";
        }
        else
        {
            resultText.text = "YOU LOSE!";
        }
    }

    public void Rematch()
    {
        // Restore normal game speed before reloading
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        // Restore normal game speed
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }
}