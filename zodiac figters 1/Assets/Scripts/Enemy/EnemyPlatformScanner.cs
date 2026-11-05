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
    // how far past a platform's edge a lower platform may be and still be a drop target (raise it if he picks a nearer platform instead of the one the player is on)
    [SerializeField] private float dropReach = 15f;
    // how far down we look for that lower platform
    [SerializeField] private float dropCheckDistance = 60f;

    [Header("debug")]
    // writes to the Console why no platform was chosen (once every 2 seconds)
    [SerializeField] private bool debugLog = true;
    private float nextLogTime;
    private float nextDropLogTime;

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

            Debug.Log("PLATFORM DIAGNOSIS: nearest platform above = " + nearestOnLayer
                + " | platforms NOT on the Ground Layer: " + (otherLayers == "" ? "none" : otherLayers));

            Debug.Log("NO PLATFORM TO JUMP TO on " + name
                + " | max jump rise = " + maxRise.ToString("F1")
                + " | platforms in scan area = " + platforms.Length
                + " (own/too low " + tooLow + ", too high " + tooHigh
                + (tooHigh > 0 ? ", lowest of those needs " + lowestTooHigh.ToString("F1") : "") + ")"
                + " | scan width " + upperScanWidth);
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

        // the platform under his feet (a generous ray, so a few units of error in the feet position don't matter)
        // use the body's X, not the Edge Check's: the Edge Check sits in front of him and is already past the platform's edge when he stops there
        Vector2 underBody = new Vector2(transform.position.x, feet.y);
        RaycastHit2D standing = Physics2D.Raycast(underBody + Vector2.up * 3f, Vector2.down, 8f, groundLayer);
        if (standing.collider == null)
        {
            if (debugLog && Time.time >= nextDropLogTime)
            {
                nextDropLogTime = Time.time + 2f;
                Debug.Log("NO DROP: no ground found under his feet at " + feet + " (checked 3 above to 5 below, layer mask " + groundLayer.value + ")");
            }
            return false;
        }

        Bounds origin = standing.collider.bounds;
        originBottomY = origin.min.y;
        edgeX = direction < 0 ? origin.min.x : origin.max.x;

        // every platform below this one that sticks out into the area past the edge (up to Drop Reach, down to Drop Check Distance)
        float boxLeft = direction < 0 ? edgeX - dropReach : edgeX;
        Vector2 boxCenter = new Vector2(boxLeft + dropReach * 0.5f, origin.min.y - 0.5f - dropCheckDistance * 0.5f);
        Collider2D[] below = Physics2D.OverlapBoxAll(boxCenter, new Vector2(dropReach, dropCheckDistance), 0f, groundLayer);

        float best = Mathf.Infinity;
        bool found = false;
        foreach (Collider2D c in below)
        {
            if (c == standing.collider) continue;
            Bounds b = c.bounds;
            if (b.max.y > origin.min.y) continue;               // not below the platform we stand on

            // the platform's point closest to our edge must be within reach
            float nearestX = direction < 0 ? b.max.x : b.min.x;
            if (direction * (nearestX - edgeX) > dropReach) continue;

            float minX = b.min.x + landingInset;
            float maxX = b.max.x - landingInset;
            if (minX > maxX) minX = maxX = b.center.x;

            // aim at the player, but never farther than we can travel from the edge
            float x = Mathf.Clamp(targetX, minX, maxX);
            float limit = edgeX + direction * dropReach;
            if (direction < 0) x = Mathf.Max(x, limit); else x = Mathf.Min(x, limit);
            x = Mathf.Clamp(x, b.min.x, b.max.x);

            float d = Mathf.Abs(x - targetX);
            if (d < best)
            {
                best = d;
                landingX = x;
                found = true;
            }
        }
        if (found) return true;

        if (debugLog && Time.time >= nextDropLogTime)
        {
            nextDropLogTime = Time.time + 2f;
            string report = "";
            Collider2D[] allBelow = Physics2D.OverlapBoxAll(new Vector2(edgeX, origin.min.y - 100f), new Vector2(400f, 200f), 0f, groundLayer);
            foreach (Collider2D c in allBelow)
            {
                if (c == standing.collider || c.bounds.max.y > origin.min.y) continue;
                float nx = direction < 0 ? c.bounds.max.x : c.bounds.min.x;
                report += c.name + " (gap " + (direction * (nx - edgeX)).ToString("F1") + ", " + (origin.min.y - c.bounds.max.y).ToString("F1") + " below) ";
            }
            Debug.Log("NO DROP " + (direction < 0 ? "LEFT" : "RIGHT") + " from '" + standing.collider.name + "' edgeX " + edgeX.ToString("F1")
                + " | Drop Reach " + dropReach + " | platforms below: " + (report == "" ? "none found" : report));
        }
        return false;
    }
}