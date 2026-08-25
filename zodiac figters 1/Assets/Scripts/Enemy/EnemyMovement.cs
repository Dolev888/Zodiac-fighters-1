using UnityEngine;
using UnityEngine.Rendering;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidP;
    [SerializeField] private float groundSpeed = 50f;
    [SerializeField] private float airSpeed = 3f;
    [SerializeField] private float jumpPower = 70f;
    [SerializeField] private Collider2D enemyCollider;
    [SerializeField] private LayerMask groundLayer;

    public void GroundMove(float direction)
    {
        rigidP.linearVelocityX = groundSpeed * direction;

        FaceDirection(direction);
    }

    public void StopMove()
    {
        rigidP.linearVelocityX = 0;
    }

    public void Jump()
    {
        rigidP.linearVelocityY = jumpPower;
    }

    public void JumpTowards(float direction)
    {
        // jump upward while also moving horizantally -1 for left 1 for right
        // giving the enemy horizantal speed towrad the target platform
        rigidP.linearVelocityX = groundSpeed * direction;
        // upward speed
        rigidP.linearVelocityY = jumpPower;
        // turn the enemy so it faces the direction it is jumping
        FaceDirection(direction) ;
    }

    public bool IsGrounded()
    {
        return
        enemyCollider.IsTouchingLayers(groundLayer);

    }

    private void FaceDirection(float direction)
    {
        if (direction < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else if (direction > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
    
}
