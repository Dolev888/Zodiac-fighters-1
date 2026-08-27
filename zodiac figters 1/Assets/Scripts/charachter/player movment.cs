using UnityEngine;

public class playermovment : MonoBehaviour
{
    
    [SerializeField] private playermain pmain;
    [SerializeField] private playerimput pimput;
    public float _groundSpeed;
    public float _airSpeed;
    public float _jumpPower;
    public float _doubleJumpPower;
    public GameObject player;
    public Rigidbody2D rigidP;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GroundMove(float dirct)
    {
        
        rigidP.linearVelocityX = _groundSpeed * dirct;
        routate(dirct);
    }
    public void AirMove(float dirct)
    {
        rigidP.linearVelocityX = Mathf.Clamp(rigidP.linearVelocityX + (_airSpeed * dirct), _groundSpeed * (-1), _groundSpeed);
        routate(dirct);
    }
    public void Jump()
    {
        rigidP.linearVelocityY = _jumpPower;
    }
    public void AirJump()
    {
        rigidP.linearVelocityY = _doubleJumpPower;
    }
    public void idle()
    {
        rigidP.linearVelocityX = 0;
    }
    public void routate(float dirct)
    {
        if (!pmain.routatelock) {
            if (dirct < 0)
            {
                rigidP.transform.rotation = new Quaternion(0, 180, 0, 0);
                pmain.isleft = true;
            }
            else if (dirct > 0)
            {
                rigidP.transform.rotation = new Quaternion(0, 0, 0, 0);
                pmain.isleft = false;
            }
        }
    }
}
