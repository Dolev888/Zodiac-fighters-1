using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject commandList;

    [SerializeField] private MatchManager matchManager;

    private bool isPaused = false;

   private  void Start()
    {
       pausePanel.SetActive(false);
        commandList.SetActive(false);
        Time.timeScale = 1.0f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !matchManager.MatchEnded)
        {
            if (commandList.activeSelf) CloseCommandList(); // making sure the command list isn't immdiatly open when pausing;

            TogglePause();

        }

    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        commandList.SetActive(false);
        Time.timeScale = 0f;
    }
    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false );
        commandList.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void OpenCommandList()
    {
        pausePanel.SetActive(false) ;
        commandList.SetActive(true) ;
    }

    public void CloseCommandList()
    {
        commandList.SetActive(false);
        pausePanel.SetActive(true) ;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

}
