using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;   // controller

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string GamePlayScene = "GamePlayScene";
    [SerializeField] private GameObject commandListPanel;              // keyboard command list
    [SerializeField] private GameObject controllerCommandListPanel;    // controller command list
    [SerializeField] private GameObject mainMenuBcakground;
    [SerializeField] private Animator[] _animator;
    [SerializeField] private int[] _twinkleOrder;
    [SerializeField] private float _whaitTime;
    private float timer;
    private int twinklecount = 0;
    [Header("Play panel (opens when PLAY is pressed)")]
    [SerializeField] private GameObject playModePanel;   // the panel with: Player vs Player, Player vs CPU, Back

    // PLAY button -> opens the panel with the two game modes
    public void OpenPlayModePanel()
    {
        mainMenuBcakground.SetActive(false);
        playModePanel.SetActive(true);
    }

    // Back button of the panel (also wire it to the Controller Back Button on the panel)
    public void ClosePlayModePanel()
    {
        playModePanel.SetActive(false);
        mainMenuBcakground.SetActive(true);
    }

    // "Player vs CPU" button
    public void StartVsCpu()
    {
        GameMode.Versus = false;
        SceneManager.LoadScene("Character Selection Screen");
    }

    // "Player vs Player" button
    public void StartVersus()
    {
        GameMode.Versus = true;
        SceneManager.LoadScene("Character Selection Screen");
    }

    // old PLAY button wiring: behaves like Player vs CPU, so nothing breaks if it is still connected
    public void Playgame()
    {
        StartVsCpu();
    }

    public void OpenCommandList()
    {
        mainMenuBcakground.SetActive(false);
        commandListPanel.SetActive(true);
    }

    public void CloseCommandList()
    {
        commandListPanel.SetActive(false);
        mainMenuBcakground.SetActive(true);
    }

    public void OpenControllerCommandList()
    {
        mainMenuBcakground.SetActive(false);
        controllerCommandListPanel.SetActive(true);
    }

    public void CloseControllerCommandList()
    {
        controllerCommandListPanel.SetActive(false);
        mainMenuBcakground.SetActive(true);
    }

    [Header("About the team panel")]
    [SerializeField] private GameObject devTeamPanel;   // the DevTeam panel (names, roles, university logo)

    // "Dev Team" button on the main menu
    public void OpenDevTeamPanel()
    {
        mainMenuBcakground.SetActive(false);
        devTeamPanel.SetActive(true);
    }

    // "Return" button of the panel (also wire it to the Controller Back Button on the panel)
    public void CloseDevTeamPanel()
    {
        devTeamPanel.SetActive(false);
        mainMenuBcakground.SetActive(true);
    }

    public void Quitgame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // East button (Circle on PlayStation, B on Xbox) closes an open command list
        if (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame)
        {
            if (commandListPanel != null && commandListPanel.activeSelf) CloseCommandList();
            else if (controllerCommandListPanel != null && controllerCommandListPanel.activeSelf) CloseControllerCommandList();
        }

        timer += Time.deltaTime;
        if (timer >= _whaitTime)
        {
            Debug.Log("helo");
            timer = 0;
            _animator[_twinkleOrder[twinklecount]].Play("twinkel");
            twinklecount++;
            if (twinklecount > 11)
            {
                twinklecount = 0;
            }
        }
    }
}