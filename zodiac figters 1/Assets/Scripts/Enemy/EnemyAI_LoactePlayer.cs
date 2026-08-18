using UnityEngine;

public class EnemyAI_LoactePlayer : MonoBehaviour
{
    [SerializeField] private playermain pmain;
    [SerializeField] private float stopDistance = 5f;
    [SerializeField] private Transform edgeCheck;
    [SerializeField] private float edgeCheckDistance = 3f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float edgeCheckOffset = 3f;
    [SerializeField] private EnemyMovement enemyMovement;
    [SerializeField] private float samePlatformHeight = 5f;
    [SerializeField] private float JumpHorizontalRange = 10f;
    [SerializeField] private float jumpCooldown = 0.5f;
    [SerializeField] private EnemyPlatformScanner platformScanner;
    [SerializeField] private float jumpHorizontalRange = 10f;

    private float nextJumpTime;

    private Transform player;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (gameObject != null )
        {
            player = playerObject.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
            return;

        float distanceX = player.position.x - transform.position.x;
        float distanceY = player.position.y - transform.position.y;

       
        // PLAYER IS ON ROUGHLY THE SAME PLATFORM HEIGHT
        
        if (Mathf.Abs(distanceY) <= samePlatformHeight)
        {
            if (Mathf.Abs(distanceX) > stopDistance)
            {
                int direction = distanceX < 0 ? -1 : 1;

                Vector2 checkPosition = new Vector2(
                    transform.position.x + (edgeCheckOffset * direction),
                    edgeCheck.position.y
                );

                RaycastHit2D groundAhead = Physics2D.Raycast(
                    checkPosition,
                    Vector2.down,
                    edgeCheckDistance,
                    groundLayer
                );

                if (groundAhead.collider != null)
                {
                    enemyMovement.GroundMove(direction);
                }
                else
                {
                    enemyMovement.StopMove();
                }
            }
            else
            {
                enemyMovement.StopMove();
            }
        }

       
        // PLAYER IS ABOVE THE ENEMY
       
        else if (distanceY > samePlatformHeight)
        {
            Vector2 upperPlatformposition;
            bool foundUpperplatform = platformScanner.TryFindingUpperPlatform(out upperPlatformposition);
            

            Debug.Log("| new scanner found: " + foundUpperplatform + "| platform position: " +  upperPlatformposition);
            
            bool platformAboveLeft =
                platformScanner.HasPlatformAbove(-1);

            bool platformAboveRight =
                platformScanner.HasPlatformAbove(1);

            Debug.Log(
                "Above left: " + platformAboveLeft +
                " Right: " + platformAboveRight +
                " distanceX: " + distanceX +
                " grounded: " + enemyMovement.IsGrounded()
            );

            int chosenDirection = 0;

            int preferredDirection =
                distanceX < 0 ? -1 : 1;

            if (preferredDirection == -1 && platformAboveLeft)
            {
                chosenDirection = -1;
            }
            else if (preferredDirection == 1 && platformAboveRight)
            {
                chosenDirection = 1;
            }
            else if (platformAboveLeft)
            {
                chosenDirection = -1;
            }
            else if (platformAboveRight)
            {
                chosenDirection = 1;
            }

           
            // Even while searching for an upper route,
            // do not allow the enemy to walk off the current platform.
            if (chosenDirection != 0)
            {
                Vector2 checkPosition = new Vector2(
                    transform.position.x +
                    (edgeCheckOffset * chosenDirection),

                    edgeCheck.position.y
                );

                RaycastHit2D groundAhead =
                    Physics2D.Raycast(
                        checkPosition,
                        Vector2.down,
                        edgeCheckDistance,
                        groundLayer
                    );

                if (groundAhead.collider != null)
                {
                    enemyMovement.GroundMove(chosenDirection);
                }
                else
                {
                    enemyMovement.StopMove();
                }
            }
            else
            {
                enemyMovement.StopMove();
            }
        }

       
        // PLAYER IS BELOW THE ENEMY
        
        else
        {
            enemyMovement.StopMove();
        }
    }
}
        
    