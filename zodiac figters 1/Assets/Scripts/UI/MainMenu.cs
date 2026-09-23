using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string GamePlayScene = "GamePlayScene";
    [SerializeField] private GameObject commandListPanel;
    [SerializeField] private GameObject mainMenuBcakground;
    [SerializeField] private Animator[] _animator;
    [SerializeField] private int[] _twinkleOrder;
    [SerializeField] private float _whaitTime;
    private float timer;
    private int twinklecount=0;
   public void Playgame()
    {
        SceneManager.LoadScene(GamePlayScene);
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
        timer += Time.deltaTime;
        if (timer >= _whaitTime) 
        {
            Debug.Log("helo");
            timer = 0;
            _animator[_twinkleOrder[twinklecount]].Play("twinkel");
            twinklecount++;
            if (twinklecount>11)
            {
                twinklecount=0;
            }
        }
    }
}
