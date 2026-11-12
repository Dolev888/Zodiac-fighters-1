using UnityEngine;
using Unity.Cinemachine;   // if this line errors (older Cinemachine), change it to: using Cinemachine;

// the choices made on the selection screen (it sets both before loading the match scene)
public static class MatchSettings
{
    // same order as MatchSetup.characterPrefabs: 0 = Aries, 1 = Scorpio, (2 = the next character...)
    public static int PlayerCharacter = 0;

    // -1 = nobody was picked (for example the match scene was started directly): the enemy is then the "next" character
    public static int EnemyCharacter = -1;
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
    [SerializeField] private GameObject _upgradeManager;

    // the health box art of one character. The two boxes have different colors (left = red, right = blue),
    // so every character has one set for each side. An empty slot leaves that box as it is.
    [System.Serializable]
    public class HealthBoxArt
    {
        public string characterName;   // only a label for the Inspector (Aries, Scorpio, Taurus)

        [Header("Left box (player 1, red)")]
        public Sprite frame;           // the bar with the face on it
        public Sprite face;            // optional separate face

        [Header("Right box (player 2 / the enemy, blue)")]
        public Sprite rightFrame;
        public Sprite rightFace;
    }

    [Header("Health box art per character, same order as Character Prefabs (leave empty to keep the current art)")]
    [SerializeField] private HealthBoxArt[] healthBoxArt;

    [Header("Safety net: a fighter that flies this far from the middle of the stage goes back to his spawn point")]
    [SerializeField] private float maxDistanceFromCenter = 300f;

    [Header("Player vs Player: which device each fighter uses")]
    [SerializeField] private bool player1UsesController = false;   // Player 1 = keyboard
    [SerializeField] private int player1PadIndex = -1;             // -1 = the pad used last, 0 = first pad, 1 = second pad
    [SerializeField] private bool player2UsesController = true;    // Player 2 = controller
    [SerializeField] private int player2PadIndex = -1;

    [Header("Testing only (-1 = use the selection screen)")]
    [SerializeField] private int testPlayerCharacter = -1;
    [SerializeField] private int testEnemyCharacter = -1;

    [HideInInspector] public FighterRole player;
    [HideInInspector] public FighterRole enemy;
    private Vector2 stageCenter;


    private void Awake()
    {
        int playerIndex = testPlayerCharacter >= 0 ? testPlayerCharacter : MatchSettings.PlayerCharacter;
        playerIndex = Mathf.Clamp(playerIndex, 0, characterPrefabs.Length - 1);

        // the enemy is the one picked on the selection screen; if none was picked, the next character in the list
        int enemyIndex;
        if (testEnemyCharacter >= 0) enemyIndex = testEnemyCharacter;
        else if (MatchSettings.EnemyCharacter >= 0) enemyIndex = MatchSettings.EnemyCharacter;
        else enemyIndex = (playerIndex + 1) % characterPrefabs.Length;
        enemyIndex = Mathf.Clamp(enemyIndex, 0, characterPrefabs.Length - 1);

        player = Spawn(playerIndex, playerSpawn, true);
        enemy = Spawn(enemyIndex, enemySpawn, false);

        CollectedDataAnalysis.MatchStart(playerIndex, enemyIndex, player.Damage, enemy.Damage);

        stageCenter = (playerSpawn.position + enemySpawn.position) * 0.5f;

        if (playerHealth != null) playerHealth.SetFighter(player.Damage);
        if (enemyHealth != null) enemyHealth.SetFighter(enemy.Damage);

        ApplyHealthArt(playerHealth, playerIndex, false);
        ApplyHealthArt(enemyHealth, enemyIndex, true);

        if (targetGroup != null)
        {
            targetGroup.AddMember(player.transform, 1f, 5f);
            targetGroup.AddMember(enemy.transform, 1f, 5f);
        }
        _upgradeManager.SetActive(true);
    }

    // gives a health box the art of the character it belongs to
    private void ApplyHealthArt(DamageDisplay box, int characterIndex, bool rightBox)
    {
        if (box == null || healthBoxArt == null) return;
        if (characterIndex < 0 || characterIndex >= healthBoxArt.Length) return;

        HealthBoxArt art = healthBoxArt[characterIndex];
        if (art == null) return;

        if (rightBox) box.SetArt(art.rightFrame, art.rightFace);
        else box.SetArt(art.frame, art.face);
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

        // Player vs Player: tell each fighter which device to read
        if (GameMode.Versus)
        {
            playerimput input = fighter.GetComponent<playerimput>();
            if (input != null)
                input.SetDevice(isPlayer ? player1UsesController : player2UsesController,
                                isPlayer ? player1PadIndex : player2PadIndex);
        }
        return role;
    }
}