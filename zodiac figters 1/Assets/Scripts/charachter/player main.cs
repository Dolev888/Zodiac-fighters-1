using UnityEngine;
using static UnityEngine.UI.Image;

public class playermain : MonoBehaviour
{
    [SerializeField] private playermovment pmove;

    [SerializeField] private playerimput pimput;
    [SerializeField] private playerattack pattack;
    [SerializeField] private LayerMask _groundLayer;
    public GameObject player;
    public Rigidbody2D rigidP;
    
    private STATE curentState;
    public STATE CurentState=> curentState;
    private STATE previesState;
    public STATE PreviesState=> previesState;

    [SerializeField] private int _airJumpMax;
    private int _airJumpCounter;

    public enum STATE
    {
        GROUND,
        AIR,
        KNOKCBACK,
        STUN,
        ATTACK
    }

    [Header("ground check")]
    [SerializeField] private Vector2 _groundCheckSise;
    [SerializeField] private Vector3 _groundCheckOffSet;
    
    void Start()
    {
       
    }
    
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground")&& GroundCheck())
        {
            
            ChangeState(STATE.GROUND);
            
        }
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            ChangeState(STATE.AIR);
        }
    }
    private void ChangeState(STATE state)
    {
        ExitState(state);
        previesState = curentState;
        curentState = state;
        EnterState (curentState);
    }
    private void ExitState(STATE state)
    {
        switch (state)
        {
            case STATE.GROUND:

                break;

            case STATE.AIR:
                break;

            case STATE.KNOKCBACK:
                break;

            case STATE.STUN:
                break;

            case STATE.ATTACK:
                break;
        }
    }
    private void EnterState(STATE state)
    {
        switch (state)
        {
            case STATE.GROUND:
                _airJumpCounter = _airJumpMax;
                break;

            case STATE.AIR:
                break;

            case STATE.KNOKCBACK:
                break;

            case STATE.STUN:
                break;

            case STATE.ATTACK:
                break;
        }

    }   
    public void WalkHandel(float direction)
    {
        switch (curentState)
        {
            case STATE.GROUND:
                pmove.GroundMove(direction);
                break;

            case STATE.AIR:
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
    private bool GroundCheck()
    {
        RaycastHit2D hit = Physics2D.BoxCast(player.transform.position + _groundCheckOffSet, _groundCheckSise, 0, Vector2.zero,0, _groundLayer);
        return hit.collider != null;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(player.transform.position + _groundCheckOffSet, _groundCheckSise);
    }
}
