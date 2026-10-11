using UnityEngine;

public class EnemyPlatformScanner : MonoBehaviour
{
    //the layer containing all platform the AI is allowed to navigate on
    [SerializeField] private LayerMask groundLayer;

    [Header("scan area (above the enemy's feet)")]
    // how wide the search area is (centered on the enemy). If platforms are far apart, make this bigger
    [SerializeField] private float upperScanWidth = 120f;

    [Header("climbing")]
    // ON = the maximum climb height is worked out from his real jump (jump power and gravity),
    // so it stays right when the platforms or the jump are changed. OFF = use Max Climb Height below
    [SerializeField] private bool autoClimbHeight = true;
    // how much of the calculated jump height we trust (0.95 = 95%)
    [SerializeField] private float jumpSafety = 0.95f;
    // only used when Auto Climb Height is OFF. Measured from his FEET to the platform's top
    [SerializeField] private float maxClimbHeight = 30f;
    // distance from his pivot down to his feet (the Edge Check child sits at -10.8)
    [SerializeField] private float feetBelowPivot = 10.8f;
    // keeps the landing point away from the platform's edges
    [SerializeField] private float landingInset = 5f;

    [Header("dropping down")]
    // how far past a platform's edge we look for a lower platform to land on
    [SerializeField] private float dropReach = 15f;
    // how far down we look for that lower platform
    [SerializeField] private float dropCheckDistance = 60f;

    [Header("debug")]
    // writes to the Console why no platform was chosen (once every 2 seconds)
    [SerializeField] private bool debugLog = true;
    private float nextLogTime;

    private EnemyMovement movement;

    private void Awake()
    {
        movement = GetComponent<EnemyMovement>();
    }

    private float MaxRise()
    {
        if (autoClimbHeight && movement != null)
        {
            float h = movement.MaxJumpHeight() * jumpSafety;
            if (h > 1f) return h;
        }
        return maxClimbHeight;
    }

    // shows the search area in the Scene view (editor only, no effect on the game)
    private void OnDrawGizmosSelected()
    {
        float rise = Application.isPlaying ? MaxRise() : maxClimbHeight;
        float feetY = transform.position.y - feetBelowPivot;
        Vector3 center = new Vector3(transform.position.x, feetY + rise * 0.5f, transform.position.z);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, new Vector3(upperScanWidth, rise, 1f));
    }

    // picks the reachable platform whose landing point is closest to the target (the player)
    public bool TryFindingPlatformTowards(Vector2 targetPoint, out Vector2 platformPosition)
    {
        platformPosition = Vector2.zero;

        float maxRise = MaxRise();
        float feetY = transform.position.y - feetBelowPivot;

        // the area from his feet up to the highest point he can reach
        Vector2 scanCenter = new Vector2(transform.position.x, feetY + maxRise * 0.5f + 1f);
        Collider2D[] platforms = Physics2D.OverlapBoxAll(scanCenter, new Vector2(upperScanWidth, maxRise + 2f), 0f, groundLayer);

        float best = Mathf.Infinity;
        bool found = false;
        int tooLow = 0, tooHigh = 0;
        float lowestTooHigh = Mathf.Infinity;

        foreach (Collider2D platform in platforms)
        {
            float top = platform.bounds.max.y;
            float rise = top - feetY;

            // the platform he is standing on (or anything not really above his feet)
            if (rise <= 1.5f) { tooLow++; continue; }

            // too high for one jump
            if (rise > maxRise)
            {
                tooHigh++;
                if (rise < lowestTooHigh) lowestTooHigh = rise;
                continue;
            }

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

        if (!found && debugLog && Time.time >= nextLogTime)
        {
            nextLogTime = Time.time + 2f;

            // extra diagnosis: look much higher, and also at every layer, to find the platform he should be reaching
            string nearestOnLayer = "none on the Ground Layer";
            Collider2D[] tall = Physics2D.OverlapBoxAll(new Vector2(transform.position.x, feetY + 150f), new Vector2(upperScanWidth, 300f), 0f, groundLayer);
            float nearestRise = Mathf.Infinity;
            foreach (Collider2D c in tall)
            {
                float r = c.bounds.max.y - feetY;
                if (r > 1.5f && r < nearestRise) { nearestRise = r; nearestOnLayer = c.name + " needs a rise of " + r.ToString("F1"); }
            }

            string otherLayers = "";
            Collider2D[] everything = Physics2D.OverlapBoxAll(new Vector2(transform.position.x, feetY + 150f), new Vector2(upperScanWidth, 300f), 0f);
            foreach (Collider2D c in everything)
            {
                if (((1 << c.gameObject.layer) & groundLayer.value) == 0 && c.name.ToLower().Contains("platform"))
                    otherLayers += c.name + " (layer " + LayerMask.LayerToName(c.gameObject.layer) + ") ";
            }

           // Debug.Log("PLATFORM DIAGNOSIS: nearest platform above = " + nearestOnLayer
           //     + " | platforms NOT on the Ground Layer: " + (otherLayers == "" ? "none" : otherLayers));

           // Debug.Log("NO PLATFORM TO JUMP TO on " + name
             //   + " | max jump rise = " + maxRise.ToString("F1")
             //   + " | platforms in scan area = " + platforms.Length
             //   + " (own/too low " + tooLow + ", too high " + tooHigh
            //    + (tooHigh > 0 ? ", lowest of those needs " + lowestTooHigh.ToString("F1") : "") + ")"
            //    + " | scan width " + upperScanWidth);
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