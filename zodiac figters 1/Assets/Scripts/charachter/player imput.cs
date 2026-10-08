using UnityEngine;

public class playerimput : MonoBehaviour
{
    [SerializeField] private playermovment pmove;
    [SerializeField] private playermain pmain;
    [SerializeField] private float _jumpBuferTime;
    public float _jumpBuferTick;
    [SerializeField] private float _basicAttackBuferTime;
    public float _basicAttackBuferTick;
    [SerializeField] private float _specialAttackBuferTime;
    public float _specialAttackBuferTick;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //    switch (pmain.CurentState)
        //    {
        //        case playermain.STATE.STUN:

        //            break;
        //        default:
        InputHandel();
        //        break;
        //}
        bafferTimer();


    }
    private void InputHandel()
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
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || _jumpBuferTick>0)
        {
            
            pmain.JumpHandle();
            
            if (_jumpBuferTick <= 0 && pmain._airJumpCounter <1 && pmain.CurentState== playermain.STATE. AIR)
            {
                _jumpBuferTick = _jumpBuferTime;
                
            }
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            pmain.AttackHandel(1);
            
            if (_basicAttackBuferTick <= 0 && pmain.CurentState == playermain.STATE.ATTACK)
            {
                _basicAttackBuferTick = _basicAttackBuferTime;

            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            pmain.AttackHandel(2);
        }
        if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift))
        {
            pmain.routatelock = true;
        }
        if (Input.GetKeyUp(KeyCode.RightShift) || Input.GetKeyUp(KeyCode.LeftShift))
        {
            pmain.routatelock = false;
        }

    }
    private void bafferTimer()
    {
        if (_jumpBuferTick > 0)
        {
            _jumpBuferTick -= Time.deltaTime;
        }
        if (_basicAttackBuferTick > 0)
        {
            _basicAttackBuferTick -= Time.deltaTime;
        }
        if (_specialAttackBuferTick > 0)
        {
            _specialAttackBuferTick -= Time.deltaTime;
        }
    }
    public void JumpSucseed()
    {
        _jumpBuferTick = 0;
    }
    public void BasicAttackSucseed()
    {
        _basicAttackBuferTick = 0;
    }
    public void SpeicelAttackSucseed()
    {
        _specialAttackBuferTick = 0;
    }

}
