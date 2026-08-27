using UnityEngine;



public class playermain : MonoBehaviour
{
    [SerializeField] private playermovment pmove;

    [SerializeField] private playerimput pimput;
    [SerializeField] private playerattack pattack;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Animator _animator;
    [Header("hit box list")]
    [SerializeField] public GameObject[] _hitBoxlist;
    [Header("else")]
    public GameObject player;
    public Rigidbody2D rigidP;
    private bool _isGrounded;
    public bool IsGrounded => _isGrounded;

    private STATE curentState;
    public STATE CurentState => curentState;
    private STATE previesState;
    public STATE PreviesState => previesState;

    [SerializeField] private int _airJumpMax;
    private int _airJumpCounter;
    public bool routatelock;
    public bool isleft;
    public bool canAirAttack;

    public enum STATE
    {
        GROUND,
        AIR,
        KNOKCBACK,
        STUN,
        ATTACK,
        NUTRAL
    }

    [Header("ground check")]
    [SerializeField] private Vector2 _groundCheckSise;
    [SerializeField] private Vector3 _groundCheckOffSet;

    void Start()
    {
        curentState = STATE.AIR;
        hitboxttagset();
        SetHitbox(0);
    }

    void Update()
    {
        
            
        
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if ((collision.gameObject.CompareTag("ground") && GroundCheck()))
        {
            
            _isGrounded = true;
            
            switch (curentState)
            {
                case STATE.AIR:
                    ChangeState(STATE.GROUND);
                    break;
                default:
                    break;
            }

        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("ground"))
        {
            _isGrounded = false;
            switch (curentState)
            {
                case STATE.GROUND:
                    ChangeState(STATE.AIR);
                    break;
                default:
                    break;
            }
        }
    }
    private void ChangeState(STATE state)
    {
        ExitState(curentState);
        previesState = curentState;
        curentState = state;
        EnterState(curentState);
    }

    private void ExitState(STATE state)
    {
        
        switch (state)
        {
            case STATE.GROUND:
                idelAnimation(false);
                break;

            case STATE.AIR:
                break;

            case STATE.KNOKCBACK:
                break;

            case STATE.STUN:
                break;

            case STATE.ATTACK:
                break;
            case STATE.NUTRAL:
                break;
        }
    }
    private void EnterState(STATE state)
    {
        Debug.Log("enter"+state);
        switch (state)
        {
            case STATE.GROUND:
                _airJumpCounter = _airJumpMax;
                canAirAttack = true;
                _isGrounded = true;
                break;

            case STATE.AIR:
                _isGrounded = false;
                break;

            case STATE.KNOKCBACK:
                break;

            case STATE.STUN:
                break;

            case STATE.ATTACK:
                break;
            case STATE.NUTRAL:
                if (_isGrounded)
                {
                    ChangeState(STATE.GROUND);
                }
                else
                {
                    ChangeState(STATE.AIR);
                }
                break;
        }

    }
    public void WalkHandel(float direction)
    {
        switch (curentState)
        {
            case STATE.GROUND:
                if (direction == 0)
                {
                    
                    idelAnimation(true);
                }
                else
                {
                    idelAnimation(false);
                }

                pmove.GroundMove(direction);
                break;

            case STATE.AIR:
                idelAnimation(false);
                if (direction == 0)
                {
                    pmove.idle();
                }
                pmove.AirMove(direction);
                break;
            default:

                break;
        }
    }
    public void RotationHandel()
    {

    }
    public void JumpHandle()
    {
        switch (curentState)
        {
            case STATE.GROUND:
                pmove.Jump();
                break;

            case STATE.AIR:
                if (_airJumpCounter > 0)
                {
                    _airJumpCounter--;
                    pmove.AirJump();
                }
                break;
            default:

                break;
        }
    }
    public void AttackHandel(int imp)
    {

        switch (curentState)
        {
            case STATE.GROUND:
                ChangeState(STATE.ATTACK);
                if (imp == 1)
                {
                    pattack.BasicAttack();
                }
                else
                {
                    pattack.SpaicleAttack();
                }
                break;
            case STATE.AIR:
                if (!canAirAttack) { break; }
                ChangeState(STATE.ATTACK);
                canAirAttack = false;
                if (imp == 1)
                {
                    pattack.BasicAttackAir();
                }
                else
                {
                    pattack.SpaicleAttackAir();
                }
                break;
            default:
                break;
        }
    }
    public void FinishAttack()
    {

        SetHitbox(0);
        ChangeState(playermain.STATE.NUTRAL);


    }
    private bool GroundCheck()
    {
        RaycastHit2D hit = Physics2D.BoxCast(player.transform.position + _groundCheckOffSet, _groundCheckSise, 0, Vector2.zero, 0, _groundLayer);
        return hit.collider != null;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(player.transform.position + _groundCheckOffSet, _groundCheckSise);
    }
    public void PlayAttackAnimation(int A)
    {
        _animator.Play("defult");
        _animator.SetInteger("move choose", A);
        _animator.SetBool("attaking", true);
    }
    public void StopAttackAnimation()
    {
        _animator.SetBool("attaking", false);
    }
    private void hitboxttagset()
    {
        for (int i = 0; i < _hitBoxlist.Length; i++)
        {
            _hitBoxlist[i].gameObject.tag = gameObject.tag;
        }

    }
    public void SetHitbox(int hitbox)
    {

        for (int i = 0; i < _hitBoxlist.Length; i++)
        {
            _hitBoxlist[i].gameObject.SetActive(false);
        }


        if (hitbox >= 0 && hitbox< _hitBoxlist.Length)
        {
            Debug.Log(hitbox);
            _hitBoxlist[hitbox].gameObject.SetActive(true);
        }
    }
    private void idelAnimation(bool B)
    {
        _animator.SetBool("idel", B);  
    }

}
