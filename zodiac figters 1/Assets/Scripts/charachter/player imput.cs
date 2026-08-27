using UnityEngine;

public class playerimput : MonoBehaviour
{
    [SerializeField] private playermovment pmove;
    [SerializeField] private playermain pmain;
  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            pmain.WalkHandel(-1);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            pmain.WalkHandel(1);
        }
        else
        {
            pmain.WalkHandel(0);
        }
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            pmain.JumpHandle();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            pmain.AttackHandel(1);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            pmain.AttackHandel(2);
        }
        if(Input.GetKeyDown(KeyCode.RightShift)|| Input.GetKeyDown(KeyCode.LeftShift))
        {
            pmain.routatelock=true;
        }
        if (Input.GetKeyUp(KeyCode.RightShift) || Input.GetKeyUp(KeyCode.LeftShift))
        {
            pmain.routatelock = false;
        }
        
        
    }

}
