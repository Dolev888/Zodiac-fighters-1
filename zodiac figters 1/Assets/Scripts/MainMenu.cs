using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string GamePlayScene = "GamePlayScene";
    [SerializeField] private GameObject commandListPanel;
    public void Playgame()
    {
        SceneManager.LoadScene(GamePlayScene);
    }

    public void OpenCommandList()
    {
       commandListPanel.SetActive(true);
    }

    public void CloseCommandList()
    {
        commandListPanel.SetActive(false);
    }

    public void Quitgame ()
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
        
    }
}
