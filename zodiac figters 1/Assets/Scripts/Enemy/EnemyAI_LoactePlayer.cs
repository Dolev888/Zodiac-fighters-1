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
    [SerializeField] private float jumpHorizontalRange = 10f; // no longer used, kept so the Inspector doesn't change
    // height of the enemy's body (feet to head), used so he doesn't steer into the platform he just left
    [SerializeField] private float headHeight = 20f;
    // tick this in the Inspector to print the AI's decisions to the Console
    [SerializeField] private bool debugLog = false;

    private float nextJumpTime;

    // --- jump state ---
    private bool inJump;            // true from the moment we jump until we land
    private Vector2 jumpTarget;     // landing point on the target platform
    private float jumpStartTime;    // safety timeout so we can never get stuck in "inJump"

    // --- drop-down state ---
    private int dropDir;            // direction we are walking off the platform (0 = no drop chosen)
    private bool dropAirborne;      // true once we have left the platform we are dropping from
    private float dropLandingX;     // X we steer to while falling
    private float dropOriginBottom; // underside of the platform we are leaving

    private Transform player;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    public bool isPlayerOnSamePlatform()
    {
        if (player == null) return false;
        float distanceY = player.position.y - transform.position.y;
        return Mathf.Abs(distanceY) <= samePlatformHeight;
    }

    // starts a jump: straight up first, sideways steering happens later in Update
    private void StartJump(Vector2 target)
    {
        enemyMovement.StopMove();           // remove the walking speed before jumping
        enemyMovement.Jump();               // straight up, no sideways speed
        jumpTarget = target;
        nextJumpTime = Time.time + jumpCooldown;
        jumpStartTime = Time.time;
        inJump = true;
    }

    // looks at both edges of the platform we stand on and commits to one of them
    private bool ChooseDropEdge()
    {
        float leftLand, leftBottom, leftEdge;
        float rightLand, rightBottom, rightEdge;
        float playerX = player.position.x;

        bool leftOk = platformScanner.TryFindingDropLanding(edgeCheck.position, -1, playerX, out leftLand, out leftBottom, out leftEdge);
        bool rightOk = platformScanner.TryFindingDropLanding(edgeCheck.position, 1, playerX, out rightLand, out rightBottom, out rightEdge);

        if (!leftOk && !rightOk) return false;

        int dir;
        if (leftOk && rightOk)
        {
            // prefer the landing closest to the player, with a small penalty for a long walk
            float leftScore = Mathf.Abs(leftLand - playerX) + 0.5f * Mathf.Abs(leftEdge - transform.position.x);
            float rightScore = Mathf.Abs(rightLand - playerX) + 0.5f * Mathf.Abs(rightEdge - transform.position.x);
            dir = leftScore <= rightScore ? -1 : 1;
        }
        else
        {
            dir = leftOk ? -1 : 1;
        }

        dropDir = dir;
        dropLandingX = dir < 0 ? leftLand : rightLand;
        dropOriginBottom = dir < 0 ? leftBottom : rightBottom;
        dropAirborne = false;

        if (debugLog) Debug.Log($"DROP | chose dir:{dir} landingX:{dropLandingX} (left ok:{leftOk}, right ok:{rightOk})");

        enemyMovement.GroundMove(dir);
        return true;
    }

    private void HandleDropDown()
    {
        bool grounded = enemyMovement.IsGrounded();

        // landed on the lower platform: forget the finished drop so the next one can be chosen
        if (dropDir != 0 && dropAirborne && grounded && edgeCheck.position.y < dropOriginBottom)
        {
            if (debugLog) Debug.Log("DROP | landed");
            dropDir = 0;
            dropAirborne = false;
        }

        // no drop chosen yet: pick an edge
        if (dropDir == 0)
        {
            if (!grounded) return;      // falling for some other reason: leave the velocity alone

            if (!ChooseDropEdge())
            {
                if (debugLog) Debug.Log("DROP | no platform below either edge");
                enemyMovement.StopMove();
            }
            return;
        }

        // committed and still on the platform: keep walking, even when the edge check would say "stop"
        if (!dropAirborne)
        {
            if (grounded)
            {
                enemyMovement.GroundMove(dropDir);
                return;
            }
            dropAirborne = true;        // we have left the platform
            if (debugLog) Debug.Log("DROP | left the platform");
        }

        // in the air: steer toward the landing spot
        float dx = dropLandingX - transform.position.x;
        int steer = dx < 0 ? -1 : 1;
        bool clearOfOrigin = edgeCheck.position.y < dropOriginBottom - headHeight;

        if (Mathf.Abs(dx) <= 2f)
            enemyMovement.StopMove();               // above the landing spot, fall straight down
        else if (steer == dropDir || clearOfOrigin)
            enemyMovement.GroundMove(steer);        // steer toward the lower platform
        else
            enemyMovement.StopMove();               // would go back under the platform we left, wait
    }

    void Update()
    {
        if (player == null)
            return;

        // ------------------------------------------------------------
        // JUMP IN PROGRESS: the rest of the AI is skipped until we land
        // ------------------------------------------------------------
        if (inJump)
        {
            bool landed = Time.time >= nextJumpTime && enemyMovement.IsGrounded();
            bool timedOut = Time.time > jumpStartTime + 3f;

            if (landed || timedOut)
            {
                inJump = false;             // normal AI resumes below
            }
            else
            {
                float dx = jumpTarget.x - transform.position.x;
                // are our feet above the top surface of the target platform?
                bool aboveTop = edgeCheck.position.y > jumpTarget.y + 1f;

                if (!aboveTop)
                    enemyMovement.StopMove();                    // keep rising straight up
                else if (Mathf.Abs(dx) > 2f)
                    enemyMovement.GroundMove(dx < 0 ? -1 : 1);   // steer over the platform
                else
                    enemyMovement.StopMove();                    // over the target, drop down

                return;                     // skip the rest of the AI while airborne
            }
        }

        float distanceX = player.position.x - transform.position.x;
        float distanceY = player.position.y - transform.position.y;

        // a drop only lasts while the player is below us
        if (distanceY > -samePlatformHeight)
        {
            dropDir = 0;
            dropAirborne = false;
        }

        // ------------------------------------------------------------
        // PLAYER IS ON ROUGHLY THE SAME PLATFORM HEIGHT
        // ------------------------------------------------------------
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

        // ------------------------------------------------------------
        // PLAYER IS ABOVE THE ENEMY
        // ------------------------------------------------------------
        else if (distanceY > samePlatformHeight)
        {
            Vector2 upperPlatformposition;
            bool foundUpperplatform = platformScanner.TryFindingPlatformTowards(player.position, out upperPlatformposition);

            if (!foundUpperplatform)
            {
                if (debugLog) Debug.Log("ABOVE | no reachable platform found");
                enemyMovement.StopMove();
                return;
            }

            float platformDistanceX = upperPlatformposition.x - transform.position.x;
            int chosenDirection = platformDistanceX < 0 ? -1 : 1;

            Vector2 checkPosition = new Vector2(
                transform.position.x + (edgeCheckOffset * chosenDirection),
                edgeCheck.position.y
            );

            RaycastHit2D groundAhead = Physics2D.Raycast(
                checkPosition,
                Vector2.down,
                edgeCheckDistance,
                groundLayer
            );

            if (debugLog)
                Debug.Log($"ABOVE | target:{upperPlatformposition} enemyX:{transform.position.x} groundAhead:{groundAhead.collider != null} grounded:{enemyMovement.IsGrounded()} cooldownReady:{Time.time >= nextJumpTime}");

            if (groundAhead.collider != null)
            {
                enemyMovement.GroundMove(chosenDirection);
            }
            else
            {
                if (enemyMovement.IsGrounded() && Time.time >= nextJumpTime)
                {
                    StartJump(upperPlatformposition);
                }
                else if (enemyMovement.IsGrounded())
                {
                    enemyMovement.StopMove();
                }
            }
        }

        // ------------------------------------------------------------
        // PLAYER IS BELOW THE ENEMY: walk off an edge and land on a lower platform
        // ------------------------------------------------------------
        else
        {
            HandleDropDown();
        }
    }
}