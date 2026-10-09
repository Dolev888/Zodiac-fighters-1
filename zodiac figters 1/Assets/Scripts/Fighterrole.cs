using UnityEngine;

// Goes on EACH character prefab (Aries and Scorpio).
// MatchSetup calls Assign() right after spawning to decide if this fighter is the player or the enemy.
// Scripts are found by their NAME, so there is nothing to drag into lists.
public class FighterRole : MonoBehaviour
{
    // switched ON only for the player: "playerimput" is your script that reads the keyboard
    // ("PlayerInput" is Unity's own Player Input component, in case a prefab has one)
    private static readonly string[] playerOnlyDefault = { "playerimput", "PlayerInput" };

    // switched ON only for the enemy (the AI scripts)
    private static readonly string[] enemyOnlyDefault =
    {
        "EnemyMovement", "EnemyAI_LoactePlayer", "EnemyPlatformScanner", "EnemyAttack_AI"
    };

    [Header("Optional: extra script names (type them, don't drag)")]
    [SerializeField] private string[] extraPlayerOnly;
    [SerializeField] private string[] extraEnemyOnly;

    [SerializeField] private FighterDamage fighterDamage;

    public bool IsPlayer { get; private set; }
    public FighterDamage Damage { get { return fighterDamage; } }

    public void Assign(bool isPlayer, MatchManager matchManager)
    {
        IsPlayer = isPlayer;

        // the tag is set first: the AI scripts look for the "Player" tag when they start
        gameObject.tag = isPlayer ? "Player" : "Enemy";

        SetEnabled(playerOnlyDefault, isPlayer);
        SetEnabled(extraPlayerOnly, isPlayer);
        SetEnabled(enemyOnlyDefault, !isPlayer);
        SetEnabled(extraEnemyOnly, !isPlayer);

        if (fighterDamage != null)
            fighterDamage.Setup(matchManager, isPlayer);
    }

    private void SetEnabled(string[] typeNames, bool value)
    {
        if (typeNames == null) return;

        foreach (Behaviour b in GetComponentsInChildren<Behaviour>(true))
        {
            string typeName = b.GetType().Name;
            foreach (string wanted in typeNames)
            {
                if (typeName == wanted) b.enabled = value;
            }
        }
    }
}