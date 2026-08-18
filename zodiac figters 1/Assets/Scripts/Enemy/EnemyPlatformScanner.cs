 using UnityEngine;

public class EnemyPlatformScanner : MonoBehaviour
{
    //the layer containing all platform the AI is allowed to navigate on
    [SerializeField] private LayerMask groundLayer;
    // how dar in front of the enemy we check
    [SerializeField] private float forwardCheckDistance = 3f;
    // how far downward we search for a platform
    [SerializeField] private float downwardCheckDistance = 4f;
    [SerializeField] private float upperCheckHorizontal = 8f;
    [SerializeField] private float upperCheckVertical = 12f;
    [SerializeField] private float lowerCheckHorizontal = 8f;
    [SerializeField] private float lowerCheckVertical = 20f;
    [SerializeField] private float upperScanWidth = 40f;
    [SerializeField] private float upperScanheight = 25f;

    // check wether is ground ahead in the requested direcction // should -1 for kest 1 for right
    public bool HasGroundAhead (int direction)
    {
        // start the ray slighly infront of the enemy
        Vector2 checkPosition = new Vector2(transform.position.x + forwardCheckDistance * direction, transform.position.y);
        // fire a ray downward looking for the ground layer
        RaycastHit2D groundHit = Physics2D.Raycast(checkPosition, Vector2.down, downwardCheckDistance, groundLayer);
        // true = there is platform ahead. // false = there is an edge/ empty space ahead
        return
            groundHit.collider != null;
    }

    //draw the scanner rays in the scene view. // this is only for helping us see/ debug the AI
    private void OnDrawGizmos()
    {
        // left-side check
        Vector3 leftStart = transform.position + Vector3.left * forwardCheckDistance;

        Gizmos.DrawLine(leftStart, leftStart + Vector3.down * downwardCheckDistance);

        //right side check
        Vector3 rightStart = transform.position + Vector3.right * forwardCheckDistance;

        Gizmos.DrawLine(rightStart, rightStart + Vector3.down * downwardCheckDistance);

        //upper left platform check
        Vector3 upperLeftStart = transform.position + Vector3.left * upperCheckHorizontal;

        Gizmos.DrawLine(upperLeftStart, upperLeftStart + Vector3.up * upperCheckVertical);

        //upper right platform check
        Vector3 upperRightStart = transform.position + Vector3.right * upperCheckHorizontal;

        Gizmos.DrawLine(upperRightStart, upperRightStart + Vector3.up * upperCheckVertical);

        // Lower-left platform check
        Vector3 lowerLeftStart =
            transform.position + Vector3.left * lowerCheckHorizontal;

        Gizmos.DrawLine(
            lowerLeftStart,
            lowerLeftStart + Vector3.down * lowerCheckVertical
        );

        // Lower-right platform check
        Vector3 lowerRightStart =
            transform.position + Vector3.right * lowerCheckHorizontal;

        Gizmos.DrawLine(
            lowerRightStart,
            lowerRightStart + Vector3.down * lowerCheckVertical
        );

        //draw the large search area doesn't affect gamplay and gives us a visual debug tool
        Vector3 upperScanCenter = new Vector3 (transform.position.x, transform.position.y + (upperScanheight/2f), transform.position.z);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(upperScanCenter, new Vector3(upperScanWidth, upperScanheight, 1f));
    }

    public bool HasPlatformAbove (int direction)
    {
        Vector2 startPosition = new Vector2(transform.position.x + upperCheckHorizontal * direction, transform.position.y);
        RaycastHit2D platformHit = Physics2D.Raycast(startPosition, Vector2.up, upperCheckVertical, groundLayer);
        return
            platformHit.collider != null;
    }

    public bool TryFindingUpperPlatform (out Vector2 platformPosition)
    {
        platformPosition = Vector2.zero;
        Vector2 scanCenter = new Vector2(transform.position.x, transform.position.y + (upperScanheight/2f));
        //creating an invisible rectangular search area that will give us every collider 2D inside it, that refers only to platforms and ignore everything else.
        Collider2D[] platforms = Physics2D.OverlapBoxAll(scanCenter, new Vector2(upperScanWidth, upperScanheight), 0f, groundLayer);
        //starting search from infinty so every valip platform found will turn to the closest 1 automatically.
        float closeDistance = Mathf.Infinity;
        bool foundPlatform = false;
        //a loop used for when we wants to perform something once for every item in the collectin
        // for every collider 2D stored in plaatforms call temporarily the current 1 platform and run the code
        foreach (Collider2D platform in platforms)
        {
            //Ignore anything that isn't actually above the enemy.
            if (platform.bounds.center.y <= transform.position.y)
            {
                continue;
            }
            //get a possible landing position at the top center of this platform.
            Vector2 candidatePosition = new Vector2(platform.bounds.center.x, platform.bounds.max.y);
            float distance = Vector2.Distance(transform.position, candidatePosition);
            if (distance < closeDistance)
            {
                closeDistance = distance;
                platformPosition = candidatePosition;
                foundPlatform = true;
            }
        }
        return foundPlatform;

    }

    public bool HasPlatformBelow(int direction)
    {
        Vector2 startPosition = new Vector2(
            transform.position.x + (lowerCheckHorizontal * direction),
            transform.position.y
        );

        RaycastHit2D platformHit = Physics2D.Raycast(
            startPosition,
            Vector2.down,
            lowerCheckVertical,
            groundLayer
        );

        return platformHit.collider != null;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
