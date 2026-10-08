using UnityEngine;

public class EnemyPlatformScanner : MonoBehaviour
{
    //the layer containing all platform the AI is allowed to navigate on
    [SerializeField] private LayerMask groundLayer;
    // size of the area above the enemy that is searched for platforms to jump to
    [SerializeField] private float upperScanWidth = 40f;
    [SerializeField] private float upperScanheight = 25f;

    [Header("climbing")]
    // platforms higher than this above the enemy's pivot are skipped (too high for one jump)
    [SerializeField] private float maxClimbHeight = 21f;
    // keeps the landing point away from the platform's edges
    [SerializeField] private float landingInset = 5f;

    [Header("dropping down")]
    // how far past a platform's edge we look for a lower platform to land on
    [SerializeField] private float dropReach = 15f;
    // how far down we look for that lower platform
    [SerializeField] private float dropCheckDistance = 60f;

    // shows the search area in the Scene view (editor only, no effect on the game)
    private void OnDrawGizmosSelected()
    {
        Vector3 center = new Vector3(transform.position.x, transform.position.y + (upperScanheight / 2f), transform.position.z);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, new Vector3(upperScanWidth, upperScanheight, 1f));
    }

    // picks the reachable platform whose landing point is closest to the target (the player)
    public bool TryFindingPlatformTowards(Vector2 targetPoint, out Vector2 platformPosition)
    {
        platformPosition = Vector2.zero;
        Vector2 scanCenter = new Vector2(transform.position.x, transform.position.y + (upperScanheight / 2f));
        Collider2D[] platforms = Physics2D.OverlapBoxAll(scanCenter, new Vector2(upperScanWidth, upperScanheight), 0f, groundLayer);

        float best = Mathf.Infinity;
        bool found = false;

        foreach (Collider2D platform in platforms)
        {
            float top = platform.bounds.max.y;
            float rise = top - transform.position.y;

            // skip platforms that are not above us, or too high for one jump
            if (rise <= 0f || rise > maxClimbHeight) continue;

            // landing point = the spot on this platform's top closest (in X) to the player, away from the edges
            float minX = platform.bounds.min.x + landingInset;
            float maxX = platform.bounds.max.x - landingInset;
            if (minX > maxX) minX = maxX = platform.bounds.center.x;   // very narrow platform
            float x = Mathf.Clamp(targetPoint.x, minX, maxX);
            Vector2 candidate = new Vector2(x, top);

            float d = Vector2.Distance(targetPoint, candidate);
            if (d < best)
            {
                best = d;
                platformPosition = candidate;
                found = true;
            }
        }
        return found;
    }

    // DROP DOWN: checks the platform the enemy stands on, and whether a lower platform
    // exists just past its edge in the given direction (-1 left, 1 right).
    // landingX = where to steer to while falling, originBottomY = underside of the platform we leave,
    // edgeX = X of the platform's edge in that direction.
    public bool TryFindingDropLanding(Vector2 feet, int direction, float targetX, out float landingX, out float originBottomY, out float edgeX)
    {
        landingX = 0f;
        originBottomY = 0f;
        edgeX = 0f;

        RaycastHit2D standing = Physics2D.Raycast(feet + Vector2.up * 0.5f, Vector2.down, 2f, groundLayer);
        if (standing.collider == null) return false;

        Bounds origin = standing.collider.bounds;
        originBottomY = origin.min.y;
        edgeX = direction < 0 ? origin.min.x : origin.max.x;

        // look for a platform below, a few units past the edge
        for (float d = 3f; d <= dropReach; d += 3f)
        {
            Vector2 start = new Vector2(edgeX + direction * d, origin.min.y - 0.5f);
            RaycastHit2D below = Physics2D.Raycast(start, Vector2.down, dropCheckDistance, groundLayer);
            if (below.collider != null)
            {
                Bounds b = below.collider.bounds;
                float minX = b.min.x + landingInset;
                float maxX = b.max.x - landingInset;
                if (minX > maxX) minX = maxX = b.center.x;
                landingX = Mathf.Clamp(targetX, minX, maxX);
                return true;
            }
        }
        return false;
    }
}