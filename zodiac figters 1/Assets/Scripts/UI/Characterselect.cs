using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;   // controller
using TMPro;

// Goes on any object in the "Character Selection Screen" scene.
// The SAME character buttons are used twice: first you pick your fighter, then you pick your opponent.
// Each character button calls ChooseCharacter in its OnClick list:
//   Aries button   -> ChooseCharacter, argument 0
//   Scorpio button -> ChooseCharacter, argument 1
//   Taurus button  -> ChooseCharacter, argument 2   (its prefab goes in MatchSetup's Character Prefabs list, in the same order)
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

    // how many characters exist (only used by the Random button). Raise it when a new character is added
    [SerializeField] private int characterCount = 3;

    [Header("Who the PLAYER is allowed to pick (0 = Aries, 1 = Scorpio, 2 = Taurus). Empty = everyone.")]
    [Tooltip("The upgrade screen needs upgrade assets for the player's character. Add a number here when that character's assets are ready. The opponent can always be anyone.")]
    [SerializeField] private int[] playableCharacters = { };

    [Header("Optional: the character buttons in character order (Aries, Scorpio, Taurus...)")]
    [Tooltip("If filled, the buttons of locked characters are greyed out while the player picks his fighter, and all buttons work again for the opponent step.")]
    [SerializeField] private Button[] characterButtons;

    private bool choosingOpponent;

    private void Start()
    {
        Time.timeScale = 1f;
        choosingOpponent = false;
        MatchSettings.EnemyCharacter = -1;
        Refresh();
    }

    private void Update()
    {
        // East button (Circle on PlayStation, B on Xbox) = Back
        if (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame)
            GoBack();
    }

    private bool IsPlayable(int characterIndex)
    {
        if (playableCharacters == null || playableCharacters.Length == 0) return true;

        foreach (int allowed in playableCharacters)
        {
            if (allowed == characterIndex) return true;
        }
        return false;
    }

    public void ChooseCharacter(int characterIndex)
    {
        // -1 = random (while picking the fighter, only among the characters the player may use)
        if (characterIndex < 0)
        {
            if (!choosingOpponent && playableCharacters != null && playableCharacters.Length > 0)
                characterIndex = playableCharacters[Random.Range(0, playableCharacters.Length)];
            else
                characterIndex = Random.Range(0, characterCount);
        }

        if (!choosingOpponent)
        {
            // a locked character can't be the player's fighter: ignore the click
            if (!IsPlayable(characterIndex)) return;

            // step 1: this is the player's fighter, now pick the opponent
            MatchSettings.PlayerCharacter = characterIndex;
            CollectedDataAnalysis.PlayerPick(characterIndex);
            choosingOpponent = true;
            Refresh();
            return;
        }

        // step 2: this is the enemy, start the match (MatchSetup reads both choices when the scene loads)
        MatchSettings.EnemyCharacter = characterIndex;
        CollectedDataAnalysis.EnemyPick(characterIndex);
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
            Refresh();
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // updates the title text and which buttons can be pressed
    private void Refresh()
    {
        if (titleText != null) titleText.text = choosingOpponent ? opponentTitle : fighterTitle;

        if (characterButtons == null) return;
        for (int i = 0; i < characterButtons.Length; i++)
        {
            if (characterButtons[i] != null)
                characterButtons[i].interactable = choosingOpponent || IsPlayable(i);
        }
    }
}