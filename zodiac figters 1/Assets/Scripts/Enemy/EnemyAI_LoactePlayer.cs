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
    [SerializeField] private float jumpCooldown = 0.5f;
    [SerializeField] private EnemyPlatformScanner platformScanner;
    [SerializeField] private float jumpHorizontalRange = 10f;

    private float nextJumpTime;

    private Transform player;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null )
        {
            player = playerObject.transform;
        }
    }

    public bool isPlayerOnSamePlatform()
    {
        if (player == null) return false;
        //find the vertical distance between plater and enemy
        float distanceY = player.position.y - transform.position.y;
        return
            Mathf.Abs(distanceY) <= samePlatformHeight;
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
            

            //Debug.Log("| new scanner found: " + foundUpperplatform + "| platform position: " +  upperPlatformposition);
            
            bool platformAboveLeft =
                platformScanner.HasPlatformAbove(-1);

            bool platformAboveRight =
                platformScanner.HasPlatformAbove(1);

           /* Debug.Log(
                "Above left: " + platformAboveLeft +
                " Right: " + platformAboveRight +
                " distanceX: " + distanceX +
                " grounded: " + enemyMovement.IsGrounded()
            );*/

            // decide which horizontal direction leads towrad the platform selected by the scanner
            int chosenDirection = 0;

            // calculate how far the detected platform is from the enemy horizontaly
            float platformDistanceX = upperPlatformposition.x - transform.position.x;

            // if valip upper platform found move towrads it horizontal
            if (foundUpperplatform)
            {
                if (platformDistanceX < 0)
                {
                    // platform is to the left
                    chosenDirection = -1;
                }
                else if(platformDistanceX > 0)
                {
                    // platform is to the right
                    chosenDirection = 1;
                }

            }
           
            // Even while searching for an upper route,
            // do not allow the enemy to walk off the current platform.
            if (chosenDirection != 0)
            {
                Vector2 checkPosition = new Vector2(
                    transform.position.x + (edgeCheckOffset * chosenDirection), upperPlatformposition.y);

                RaycastHit2D groundAhead =
                    Physics2D.Raycast( checkPosition, Vector2.down,edgeCheckDistance, groundLayer );

                if (groundAhead.collider != null)
                {
                    //continue moving horizantally towards the platform selected by the canner.
                    enemyMovement.GroundMove(chosenDirection);
                    //check how close the enemy is to the horizantal center of the platform
                    float distanceToPlatformX = Mathf.Abs(upperPlatformposition.x - transform.position.x);
                    // when the enemy is close enough to target platform & and is currently standing on the ground jump
                    if (distanceToPlatformX <= jumpHorizontalRange && enemyMovement.IsGrounded())
                    {
                        enemyMovement.JumpTowards(chosenDirection);
                    }
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
        
    