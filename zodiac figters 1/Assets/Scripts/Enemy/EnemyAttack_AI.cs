using UnityEngine;

public class EnemyAttack_AI : MonoBehaviour
{
    [SerializeField] private playermain pmain;
    // the player he attacks: found by the "Player" tag when this script switches on (nothing to assign)
    private Transform target;
    [SerializeField] private float samePlatformHeight = 5f;
    [SerializeField] private float attackRangeGN = 5f; //GN stands for ground normal since it has diffrent range then the aerial
    [SerializeField] private EnemyAI_LoactePlayer locatePlayer;
    [SerializeField] private float attackCoolDownGN = 1.5f; // how long between ground normal attack to wait
    private float nextGN_AtackTime; // stores the next yime enemy allowd to attack

    [SerializeField] private float attackRangeGS = 15f; // GS = ground Special

    [SerializeField] private float attackCoolDownGS = 5f;

    private float nextGSAttackTime;

    [SerializeField] private float attackRangeAN = 12f; //AN = aerial normal

    [SerializeField] private float attackCoolDownAN = 3f;

    private float nextANAttackTime;

    [SerializeField] private float attackRangeAS = 18f; // AS = aerial special

    [SerializeField] private float attackCoolDownAS = 5f;

    private float nextASAttackTime;

    // the ground special is only used when the player is at least this far away (so up close he uses the normal attack)
    [SerializeField] private float minSpecialDistanceGS = 10f;

    [Header("Dash attacks")]
    // he dashes at the player from this distance range (too close = normal attack, too far = keeps walking)
    [SerializeField] private float dashMinDistance = 7f;
    [SerializeField] private float dashMaxDistance = 24f;
    [SerializeField] private float attackCoolDownDash = 4f;
    // how far a dash carries him. He only dashes when there is ground under the whole length (so he can't dash off the map).
    // measure it once in the Scene view; if he still goes off an edge, make this bigger
    [SerializeField] private float dashTravelDistance = 20f;
    private float nextDashTime;

    [Header("Aggression (makes him harder without changing every number above)")]
    // 1 = the cooldowns above as they are. 0.7 = he attacks about 40% more often. 0.5 = twice as often
    [SerializeField, Range(0.3f, 1.5f)] private float cooldownScale = 0.7f;
    // 1 = the ranges above as they are. 1.2 = he starts attacking from 20% farther away
    [SerializeField, Range(0.8f, 1.6f)] private float rangeScale = 1.15f;

    // the enemy's own movement script (found automatically, nothing to assign in the Inspector)
    private EnemyMovement enemyMovement;
    private EnemyPlatformScanner scanner;

    void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        scanner = GetComponent<EnemyPlatformScanner>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) target = playerObject.transform;
    }

    // false when a dash toward the player would carry him off the platform
    private bool DashIsSafe(float distanceX, bool airborne)
    {
        if (scanner == null) return true;
        return scanner.IsDashSafe(distanceX < 0 ? -1 : 1, dashTravelDistance, airborne);
    }

    // turn toward the player right before attacking, so the attack (and its push) goes the right way
    private void FaceTarget(float distanceX)
    {
        if (enemyMovement != null) enemyMovement.FaceTowards(distanceX);
    }

    void Update()
    {
        // making sure the references exist
        if (locatePlayer == null || target == null)
        {
            return;
        }

        // find the horizontal distance between enemy and player
        float distanceX = target.position.x - transform.position.x;
        float absDistanceX = Mathf.Abs(distanceX);

        // aggression: the same numbers as before, scaled
        float rangeGN = attackRangeGN * rangeScale;
        float rangeGS = attackRangeGS * rangeScale;
        float rangeAN = attackRangeAN * rangeScale;
        float rangeAS = attackRangeAS * rangeScale;

        // only GROUND and AIR can start an attack; in any other state (ATTACK, NUTRAL, STUN...) a request would be ignored
        // by AttackHandel but would still use up the cooldown, so do nothing
        if (pmain.CurentState != playermain.STATE.GROUND && pmain.CurentState != playermain.STATE.AIR)
            return;

        if (pmain.CurentState == playermain.STATE.AIR)
        {
            // aerial normal
            if (absDistanceX <= rangeAN)
            {
                if (Time.time >= nextANAttackTime)
                {
                    FaceTarget(distanceX); pmain.AttackHandel(1); // 1= normal, the air state check determines weather we will use the aerial or grounded normal
                    nextANAttackTime = Time.time + attackCoolDownAN * cooldownScale;
                }
            }
            else if (absDistanceX <= rangeAS)
            {
                if (Time.time >= nextASAttackTime)
                {
                    FaceTarget(distanceX); pmain.AttackHandel(2); // 2= special
                    nextASAttackTime = Time.time + attackCoolDownAS * cooldownScale;
                }
            }
            else if (absDistanceX <= dashMaxDistance && pmain._airDashCounter > 0 && Time.time >= nextDashTime && DashIsSafe(distanceX, true))
            {
                // too far for the aerial attacks: dash at the player
                FaceTarget(distanceX);
                pmain.DashHandle(false);
                nextDashTime = Time.time + attackCoolDownDash * cooldownScale * Random.Range(0.8f, 1.4f);
            }
            return;
        }

        //ground attacks using locatePlayer
        if (locatePlayer.isPlayerOnSamePlatform())
        {
            //ground normal
            if (absDistanceX <= rangeGN)
            {
                if (Time.time >= nextGN_AtackTime)
                {
                    FaceTarget(distanceX); pmain.AttackHandel(1);
                    nextGN_AtackTime = Time.time + attackCoolDownGN * cooldownScale;
                }
            }
            else
            {
                bool specialReady = absDistanceX >= minSpecialDistanceGS && absDistanceX <= rangeGS && Time.time >= nextGSAttackTime;
                bool dashReady = absDistanceX >= dashMinDistance && absDistanceX <= dashMaxDistance && Time.time >= nextDashTime && DashIsSafe(distanceX, false);

                // when both are ready, pick one at random so he is not predictable
                if (specialReady && (!dashReady || Random.value < 0.5f))
                {
                    FaceTarget(distanceX); pmain.AttackHandel(2);
                    nextGSAttackTime = Time.time + attackCoolDownGS * cooldownScale;
                }
                else if (dashReady)
                {
                    FaceTarget(distanceX); pmain.DashHandle(false);   // false = forward dash, toward the player
                    nextDashTime = Time.time + attackCoolDownDash * cooldownScale * Random.Range(0.8f, 1.4f);
                }
            }
        }
    }
}