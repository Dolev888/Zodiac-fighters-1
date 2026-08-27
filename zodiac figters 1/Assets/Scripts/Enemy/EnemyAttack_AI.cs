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
        // ask EnemyAI_LocatePlayer if the player is currently on the same platform
        if(locatePlayer.isPlayerOnSamePlatform())
        {
           // find the horizontal distance between enemy and player's collider
           float distanceX = targetCollider.transform.position.x - transform.position.x;
            if (Mathf.Abs(distanceX) <= attackRangeGN)
            {
                // only attack if cooldown has finished
                if (Time.time >= nextGN_AtackTime)
                {
                    Debug.Log(" AttackAI: using ground normal attack");
                    pmain.AttackHandel(1);
                    nextGN_AtackTime = Time.time + attackCoolDownGN; // start cooldown before the next attack
                }
            }
            else if(Mathf.Abs(distanceX) <= attackRangeGS)
            {
                if (Time.time >= nextGSAttackTime)
                {
                    Debug.Log("AttackAI: using ground special");
                    pmain.AttackHandel(2);
                    nextGN_AtackTime = Time.time + attackCoolDownGS;
                }
            }
        }
        

    }
}
