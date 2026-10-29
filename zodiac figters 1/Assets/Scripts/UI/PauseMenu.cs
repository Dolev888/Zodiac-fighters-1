using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject commandList;                 // keyboard command list
    [SerializeField] private GameObject controllerCommandList;       // controller command list (optional, can stay empty for now)

    [SerializeField] private MatchManager matchManager;

    [Header("Scenes (must match the scene names exactly and be in the Build Profiles list)")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string characterSelectSceneName = "Character Selection Screen";

    private bool isPaused = false;

    private void Start()
    {
        pausePanel.SetActive(false);
        commandList.SetActive(false);
        if (controllerCommandList != null) controllerCommandList.SetActive(false);
        Time.timeScale = 1.0f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !matchManager.MatchEnded)
        {
            // making sure a command list isn't immdiatly open when pausing;
            if (commandList.activeSelf) CloseCommandList();
            if (controllerCommandList != null && controllerCommandList.activeSelf) CloseControllerCommandList();

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
        if (controllerCommandList != null) controllerCommandList.SetActive(false);
        Time.timeScale = 0f;
    }
    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        commandList.SetActive(false);
        if (controllerCommandList != null) controllerCommandList.SetActive(false);
        CollectedDataAnalysis.ControlsClosed();   // playtest log: does nothing if no list was open
        Time.timeScale = 1.0f;
    }

    // ---- keyboard command list ----
    public void OpenCommandList()
    {
        pausePanel.SetActive(false);
        commandList.SetActive(true);
        CollectedDataAnalysis.ControlsOpened();
    }

    public void CloseCommandList()
    {
        commandList.SetActive(false);
        pausePanel.SetActive(true);
        CollectedDataAnalysis.ControlsClosed();
    }

    // ---- controller command list (for the button you will add later) ----
    public void OpenControllerCommandList()
    {
        if (controllerCommandList == null) return;
        pausePanel.SetActive(false);
        controllerCommandList.SetActive(true);
        CollectedDataAnalysis.ControlsOpened();
    }

    public void CloseControllerCommandList()
    {
        if (controllerCommandList == null) return;
        controllerCommandList.SetActive(false);
        pausePanel.SetActive(true);
        CollectedDataAnalysis.ControlsClosed();
    }

    // ---- character selection (for the button you will add later) ----
    public void GoToCharacterSelection()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(characterSelectSceneName);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

}