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
