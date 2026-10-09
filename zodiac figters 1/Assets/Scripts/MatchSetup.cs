using UnityEngine;
using Unity.Cinemachine;   // if this line errors (older Cinemachine), change it to: using Cinemachine;

// the choice made on the selection screen (it only has to set PlayerCharacter before loading the match scene)
public static class MatchSettings
{
    // same order as MatchSetup.characterPrefabs: 0 = Aries, 1 = Scorpio
    public static int PlayerCharacter = 0;
}

// One object in the match scene. Spawns both fighters, makes one the player and the other the enemy,
// and connects them to the match manager, the health boxes and the camera.
public class MatchSetup : MonoBehaviour
{
    [SerializeField] private GameObject[] characterPrefabs;   // 0 = Aries, 1 = Scorpio
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private Transform enemySpawn;

    [SerializeField] private MatchManager matchManager;
    [SerializeField] private CinemachineTargetGroup targetGroup;
    [SerializeField] private DamageDisplay playerHealth;      // the box at the bottom-left
    [SerializeField] private DamageDisplay enemyHealth;       // the box at the bottom-right

    [Header("Testing only (-1 = use the selection screen)")]
    [SerializeField] private int testPlayerCharacter = -1;
    [SerializeField] private int testEnemyCharacter = -1;

    private void Awake()
    {
        int playerIndex = testPlayerCharacter >= 0 ? testPlayerCharacter : MatchSettings.PlayerCharacter;
        playerIndex = Mathf.Clamp(playerIndex, 0, characterPrefabs.Length - 1);

        // by default the enemy is the other character
        int enemyIndex = testEnemyCharacter >= 0 ? testEnemyCharacter : (playerIndex + 1) % characterPrefabs.Length;

        FighterRole player = Spawn(playerIndex, playerSpawn, true);
        FighterRole enemy = Spawn(enemyIndex, enemySpawn, false);

        if (playerHealth != null) playerHealth.SetFighter(player.Damage);
        if (enemyHealth != null) enemyHealth.SetFighter(enemy.Damage);

        if (targetGroup != null)
        {
            targetGroup.AddMember(player.transform, 1f, 5f);
            targetGroup.AddMember(enemy.transform, 1f, 5f);
        }
    }

    private FighterRole Spawn(int index, Transform point, bool isPlayer)
    {
        GameObject fighter = Instantiate(characterPrefabs[index], point.position, point.rotation);
        FighterRole role = fighter.GetComponent<FighterRole>();
        role.Assign(isPlayer, matchManager);
        return role;
    }
}