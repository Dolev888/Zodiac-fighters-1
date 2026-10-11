using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidP;
    [SerializeField] private float groundSpeed = 50f;
    [SerializeField] private float jumpPower = 70f;
    [SerializeField] private LayerMask groundLayer;

    // the enemy's own playermain and animator (found automatically, nothing to assign in the Inspector)
    private playermain pmain;
    private Animator animator;

    private static readonly int WalkingParam = Animator.StringToHash("walking");
    private static readonly int IdleParam = Animator.StringToHash("idel");

    void Awake()
    {
        pmain = GetComponent<playermain>();
        animator = GetComponentInChildren<Animator>();

        // start with isleft matching the way the enemy is currently facing
        if (pmain != null)
        {
            pmain.isleft = Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, 180f)) < 1f;
        }
    }

    // The player's walk animation is started by playermain.WalkHandel, which the AI never calls
    // (it moves the body directly), so the animation is driven from here: walk while he really moves on the ground.
    void Update()
    {
        if (animator == null || pmain == null) return;

        bool walking = pmain.CurentState == playermain.STATE.GROUND && Mathf.Abs(rigidP.linearVelocityX) > 1f;

        animator.SetFloat(WalkingParam, walking ? 1f : 0f);
        if (walking) animator.SetBool(IdleParam, false);   // leave the idle animation when he starts walking
    }

    public void GroundMove(float direction)
    {
        rigidP.linearVelocityX = groundSpeed * direction;

        FaceDirection(direction);
    }

    public void StopMove()
    {
        rigidP.linearVelocityX = 0;
    }

    // walks away from the player WITHOUT turning around (he keeps facing the player); slower than a normal walk
    [SerializeField] private float retreatSpeedMultiplier = 0.6f;

    public void Retreat(float awayDirection)
    {
        rigidP.linearVelocityX = groundSpeed * retreatSpeedMultiplier * awayDirection;
    }

    public void Jump()
    {
        rigidP.linearVelocityY = jumpPower;
    }

    // A small box at his feet, the same place and size as the ground-check box on his Playermain
    // (offset -0.17, -10.88 / size 2.56 x 0.69). It only sees ground UNDER him, so a platform's side
    // or underside no longer counts, and it doesn't depend on which collider is enabled or assigned.
    [Header("ground check (feet)")]
    [SerializeField] private Vector2 feetBoxOffset = new Vector2(-0.17f, -10.88f);
    [SerializeField] private Vector2 feetBoxSize = new Vector2(2.56f, 0.69f);
    // while he is rising faster than this he is never "grounded" (stops a wall or underside graze right after a jump)
    [SerializeField] private float maxRiseSpeedWhenGrounded = 1f;

    public bool IsGrounded()
    {
        if (rigidP.linearVelocityY > maxRiseSpeedWhenGrounded) return false;

        Vector2 center = (Vector2)transform.position + feetBoxOffset;
        return Physics2D.OverlapBox(center, feetBoxSize, 0f, groundLayer) != null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube((Vector2)transform.position + feetBoxOffset, feetBoxSize);
    }

    // turns the enemy to face a direction without moving (negative = left, positive = right)
    public void FaceTowards(float direction)
    {
        FaceDirection(direction);
    }

    private void FaceDirection(float direction)
    {
        if (direction < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            // playermain.isleft is what the attacks use to flip their push (playerattack.SetVelocity)
            if (pmain != null) pmain.isleft = true;
        }
        else if (direction > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            if (pmain != null) pmain.isleft = false;
        }
    }

}