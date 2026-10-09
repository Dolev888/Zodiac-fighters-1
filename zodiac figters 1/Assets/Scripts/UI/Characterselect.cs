using UnityEngine;
using UnityEngine.SceneManagement;

// Goes on any object in the CharacterSelectionScreen scene.
// Each character button calls ChooseCharacter in its OnClick list:
//   Aries button   -> ChooseCharacter, argument 0
//   Scorpio button -> ChooseCharacter, argument 1
//   (a future character just gets the next number, and its prefab goes in MatchSetup's Character Prefabs list)
// The Back button calls GoBack.
public class CharacterSelect : MonoBehaviour
{
    // both names must match the scene names exactly, and both scenes must be in the Build Profiles scene list
    [SerializeField] private string matchSceneName = "GamePlayScene";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public void ChooseCharacter(int characterIndex)
    {
        // MatchSetup reads this when the match scene loads (0 = Aries, 1 = Scorpio)
        MatchSettings.PlayerCharacter = characterIndex;

        Time.timeScale = 1f;
        SceneManager.LoadScene(matchSceneName);
    }

    public void GoBack()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}