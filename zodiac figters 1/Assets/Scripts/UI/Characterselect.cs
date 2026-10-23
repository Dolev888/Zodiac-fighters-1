using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// Goes on any object in the "Character Selection Screen" scene.
// The SAME character buttons are used twice: first you pick your fighter, then you pick your opponent.
// Each character button calls ChooseCharacter in its OnClick list:
//   Aries button   -> ChooseCharacter, argument 0
//   Scorpio button -> ChooseCharacter, argument 1
//   new character  -> ChooseCharacter, argument 2   (its prefab goes in MatchSetup's Character Prefabs list, in the same order)
//   optional "Random" button -> ChooseCharacter, argument -1
// The Back button calls GoBack: from the opponent step it returns to the fighter step, from the fighter step to the main menu.
public class CharacterSelect : MonoBehaviour
{
    // both names must match the scene names exactly, and both scenes must be in the Build Profiles scene list
    [SerializeField] private string matchSceneName = "GamePlayScene";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Optional: a text that tells the player which step he is on")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private string fighterTitle = "CHOOSE YOUR FIGHTER";
    [SerializeField] private string opponentTitle = "CHOOSE YOUR OPPONENT";

    // how many characters exist (only used by the Random button). Raise it when the new character is added
    [SerializeField] private int characterCount = 2;

    private bool choosingOpponent;

    private void Start()
    {
        Time.timeScale = 1f;
        choosingOpponent = false;
        MatchSettings.EnemyCharacter = -1;
        RefreshTitle();
    }

    public void ChooseCharacter(int characterIndex)
    {
        // -1 = random
        if (characterIndex < 0) characterIndex = Random.Range(0, characterCount);

        if (!choosingOpponent)
        {
            // step 1: this is the player's fighter, now pick the opponent
            MatchSettings.PlayerCharacter = characterIndex;
            choosingOpponent = true;
            RefreshTitle();
            return;
        }

        // step 2: this is the enemy, start the match (MatchSetup reads both choices when the scene loads)
        MatchSettings.EnemyCharacter = characterIndex;
        Time.timeScale = 1f;
        SceneManager.LoadScene(matchSceneName);
    }

    public void GoBack()
    {
        if (choosingOpponent)
        {
            // back to picking the fighter
            choosingOpponent = false;
            MatchSettings.EnemyCharacter = -1;
            RefreshTitle();
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void RefreshTitle()
    {
        if (titleText != null) titleText.text = choosingOpponent ? opponentTitle : fighterTitle;
    }
}