using UnityEngine;

public class EnemyAttack_AI : MonoBehaviour
{
    [SerializeField] private playermain pmain;
    // the player collider that the enemy will check before deciding if the player is close enough to attack
    [SerializeField] private Collider2D targetCollider;
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

    // the enemy's own movement script (found automatically, nothing to assign in the Inspector)
    private EnemyMovement enemyMovement;

    void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
    }

    // turn toward the player right before attacking, so the attack (and its push) goes the right way
    private void FaceTarget(float distanceX)
    {
        if (enemyMovement != null) enemyMovement.FaceTowards(distanceX);
    }

    void Update()
    {
        // making sure the references exist
        if (locatePlayer == null || targetCollider == null)
        {
            return;
        }

        // find the horizontal distance between enemy and player's collider
        float distanceX = targetCollider.transform.position.x - transform.position.x;
        float absDistanceX = Mathf.Abs(distanceX);

        // only GROUND and AIR can start an attack; in any other state (ATTACK, NUTRAL, STUN...) a request would be ignored
        // by AttackHandel but would still use up the cooldown, so do nothing
        if (pmain.CurentState != playermain.STATE.GROUND && pmain.CurentState != playermain.STATE.AIR)
            return;

        if (pmain.CurentState == playermain.STATE.AIR)
        {
            // aerial normal
            if (absDistanceX <= attackRangeAN)
            {
                if (Time.time >= nextANAttackTime)
                {
                    FaceTarget(distanceX); pmain.AttackHandel(1); // 1= normal, the air state check determines weather we will use the aerial or grounded normal
                    nextANAttackTime = Time.time + attackCoolDownAN;
                }
            }
            else if (absDistanceX <= attackRangeAS)
            {
                if (Time.time >= nextASAttackTime)
                {
                    FaceTarget(distanceX); pmain.AttackHandel(2); // 2= special
                    nextASAttackTime = Time.time + attackCoolDownAS;
                }
            }
            return;
        }

        //ground attacks using locatePlayer
        if (locatePlayer.isPlayerOnSamePlatform())
        {
            //ground normal
            if (absDistanceX <= attackRangeGN)
            {
                if (Time.time >= nextGN_AtackTime)
                {
                    FaceTarget(distanceX); pmain.AttackHandel(1);
                    nextGN_AtackTime = Time.time + attackCoolDownGN;
                }
            }
            else if (absDistanceX >= minSpecialDistanceGS && absDistanceX <= attackRangeGS)
            {
                if (Time.time >= nextGSAttackTime)
                {
                    FaceTarget(distanceX); pmain.AttackHandel(2);
                    nextGSAttackTime = Time.time + attackCoolDownGS;
                }
            }
        }
    }
}