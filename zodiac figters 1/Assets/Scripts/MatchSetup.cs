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

    [Header("Safety net: a fighter that flies this far from the middle of the stage goes back to his spawn point")]
    [SerializeField] private float maxDistanceFromCenter = 300f;

    [Header("Testing only (-1 = use the selection screen)")]
    [SerializeField] private int testPlayerCharacter = -1;
    [SerializeField] private int testEnemyCharacter = -1;

    private FighterRole player;
    private FighterRole enemy;
    private Vector2 stageCenter;

    private void Awake()
    {
        int playerIndex = testPlayerCharacter >= 0 ? testPlayerCharacter : MatchSettings.PlayerCharacter;
        playerIndex = Mathf.Clamp(playerIndex, 0, characterPrefabs.Length - 1);

        // by default the enemy is the other character
        int enemyIndex = testEnemyCharacter >= 0 ? testEnemyCharacter : (playerIndex + 1) % characterPrefabs.Length;

        player = Spawn(playerIndex, playerSpawn, true);
        enemy = Spawn(enemyIndex, enemySpawn, false);

        stageCenter = (playerSpawn.position + enemySpawn.position) * 0.5f;

        if (playerHealth != null) playerHealth.SetFighter(player.Damage);
        if (enemyHealth != null) enemyHealth.SetFighter(enemy.Damage);

        if (targetGroup != null)
        {
            targetGroup.AddMember(player.transform, 1f, 5f);
            targetGroup.AddMember(enemy.transform, 1f, 5f);
        }
    }

    private void Update()
    {
        KeepInBounds(player, playerSpawn);
        KeepInBounds(enemy, enemySpawn);
    }

    // if a fighter ends up far outside the stage, put him back at his spawn point
    private void KeepInBounds(FighterRole fighter, Transform spawn)
    {
        if (fighter == null) return;

        float distance = ((Vector2)fighter.transform.position - stageCenter).magnitude;
        if (distance <= maxDistanceFromCenter) return;

        Rigidbody2D body = fighter.GetComponent<Rigidbody2D>();
        playermain main = fighter.GetComponent<playermain>();

        // what he looked like when he got out (this tells us why)
        Debug.Log("OUT OF BOUNDS: " + fighter.name
            + " | isPlayer=" + fighter.IsPlayer
            + " | pos=" + fighter.transform.position
            + " | velocity=" + (body != null ? body.linearVelocity.ToString() : "-")
            + " | gravityScale=" + (body != null ? body.gravityScale.ToString() : "-")
            + " | state=" + (main != null ? main.CurentState.ToString() : "-")
            + " -> back to spawn");

        fighter.transform.position = spawn.position;

        if (body != null) body.linearVelocity = Vector2.zero;
        if (main != null) main.ResetGravity();
    }

    private FighterRole Spawn(int index, Transform point, bool isPlayer)
    {
        GameObject fighter = Instantiate(characterPrefabs[index], point.position, point.rotation);
        FighterRole role = fighter.GetComponent<FighterRole>();
        role.Assign(isPlayer, matchManager);
        return role;
    }
}