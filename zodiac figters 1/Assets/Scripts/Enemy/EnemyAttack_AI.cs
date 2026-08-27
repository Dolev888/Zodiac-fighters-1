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
    [SerializeField] private float nextGN_AtackTime = 0f; // stores the next yime enemy allowd to attack

    [SerializeField] private float attackRangeGS = 15f; // GS = ground Special

    [SerializeField] private float attackCoolDownGS = 5f;

    private float nextGSAttackTime;

    [SerializeField] private float attackRangeAN = 12f; //AN = aerial normal

    [SerializeField] private float attackCoolDownAN = 3f;

    private float nextANAttackTime;

    [SerializeField] private float attackRangeAS = 18f; // AS = aerial special

    [SerializeField] private float attackCoolDownAS = 5f;

    private float nextASAttackTime;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // making sure the reference for locate player exist
        if (locatePlayer == null)
        {
            return;
        }
        // find the horizontal distance between enemy and player's collider
        float distanceX = targetCollider.transform.position.x - transform.position.x;
        float absDistanceX = Mathf.Abs(distanceX);
        Debug.Log("Enemy state: " + pmain.CurentState);
        if(pmain.CurentState == playermain.STATE.AIR)
        {
            Debug.Log("AirDistance: " + absDistanceX + " AN range: " + attackRangeAN + "AS range" + attackRangeAS);
            
            // aerial normal
            if (absDistanceX <= attackRangeAN)
            {
                if(Time.time >= nextANAttackTime)
                {
                    Debug.Log("AttackAI: aerial normal");
                    pmain.AttackHandel(1); // 1= normal, the air state check determines weather we will use the aerial or grounded normal
                    nextANAttackTime = Time.time + attackCoolDownAN;
                }
            }
            else if (absDistanceX <= attackRangeAS)
            {
                if (Time.time >= nextASAttackTime)
                {
                    Debug.Log("AttackAI: aerial special");
                    pmain.AttackHandel(2); // 2= special
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
                    Debug.Log("AttackAI: ground normal");
                    pmain.AttackHandel(1);
                    nextGN_AtackTime = Time.time + attackCoolDownGN;
                }
            }
            else if (absDistanceX <= attackRangeGS)
            {
                if (Time.time >= nextGSAttackTime)
                {
                    Debug.Log("AttackAI: ground special");
                    pmain.AttackHandel(2);
                    nextGSAttackTime = Time.time + attackCoolDownGS;
                }
            }
        }


    }
}
