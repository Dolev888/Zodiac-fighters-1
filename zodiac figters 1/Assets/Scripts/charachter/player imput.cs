using UnityEngine;
using UnityEngine.InputSystem;   // controller (needs the Input System package)

public class playerimput : MonoBehaviour
{
    [SerializeField] private playermovment pmove;
    [SerializeField] private playermain pmain;
    [SerializeField] private float _jumpBuferTime;
    [HideInInspector] public float _jumpBuferTick = 0;
    [SerializeField] private float _basicAttackBuferTime;
    [HideInInspector] public float _basicAttackBuferTick = 0;
    [SerializeField] private float _specialAttackBuferTime;
    [HideInInspector] public float _specialAttackBuferTick = 0;
    [SerializeField] private float _dashBuferTime;
    [HideInInspector] public float _dashBuferTick;

    [Header("controller")]
    // the left stick has to move at least this far (0 to 1) before it counts, so he doesn't drift when the stick is released
    [SerializeField] private float _stickDeadzone = 0.2f;
    // pushing the stick down this far while pressing dash = back dash (same as holding the Down arrow)
    [SerializeField] private float _stickDownForBackDash = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _jumpBuferTick = -0.1f;
        _basicAttackBuferTick = -0.1f;
        _specialAttackBuferTick = -0.1f;
        _dashBuferTick = -0.1f;
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
        // Controller: read it once per frame. While the game is paused (timeScale 0) the buttons belong to the menus, so they are ignored here.
        Gamepad pad = Time.timeScale > 0f ? Gamepad.current : null;

        float padMove = 0f;
        bool padStickDown = false;
        bool padJump = false, padBasic = false, padSpecial = false, padDash = false;

        if (pad != null)
        {
            Vector2 stick = pad.leftStick.ReadValue();

            // digital like the keyboard: -1, 0 or 1 (so the walk speed and animation feel the same)
            if (stick.x <= -_stickDeadzone) padMove = -1f;
            else if (stick.x >= _stickDeadzone) padMove = 1f;

            padStickDown = stick.y <= -_stickDownForBackDash;

            padJump = pad.buttonSouth.wasPressedThisFrame;        // X (PlayStation) / A (Xbox)
            padBasic = pad.buttonWest.wasPressedThisFrame;        // Square / X
            padSpecial = pad.buttonNorth.wasPressedThisFrame;     // Triangle / Y
            padDash = pad.leftShoulder.wasPressedThisFrame;       // L1 / LB
        }

        // Movement (keyboard first, the stick is used when no arrow is held)
        if (Input.GetKey(KeyCode.LeftArrow))
            pmain.WalkHandel(-1);
        else if (Input.GetKey(KeyCode.RightArrow))
            pmain.WalkHandel(1);
        else
            pmain.WalkHandel(padMove);

        // Record inputs immediately
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || padJump)
            _jumpBuferTick = _jumpBuferTime;

        if (Input.GetKeyDown(KeyCode.Z) || padBasic)
            _basicAttackBuferTick = _basicAttackBuferTime;

        if (Input.GetKeyDown(KeyCode.X) || padSpecial)
            _specialAttackBuferTick = _specialAttackBuferTime;
        if (Input.GetKeyDown(KeyCode.C) || padDash)
            _dashBuferTick = _dashBuferTime;

        // Attempt buffered actions until they succeed or expire
        if (_jumpBuferTick > 0)
            pmain.JumpHandle();

        if (_basicAttackBuferTick > 0)
            pmain.AttackHandel(1);

        if (_specialAttackBuferTick > 0)
            pmain.AttackHandel(2);
        if (_dashBuferTick > 0)
        {
            if (Input.GetKey(KeyCode.DownArrow) || padStickDown)
            {
                pmain.DashHandle(true);
            }
            pmain.DashHandle(false);
        }


        // Rotation lock
        if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift))
            pmain.routatelock = true;

        if (Input.GetKeyUp(KeyCode.RightShift) || Input.GetKeyUp(KeyCode.LeftShift))
            pmain.routatelock = false;
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
        if (_dashBuferTick > 0)
        {
            _dashBuferTick -= Time.deltaTime;
        }
    }
    public void JumpSucseed()
    {
        _jumpBuferTick = -0.1f;
    }
    public void BasicAttackSucseed()
    {
        _basicAttackBuferTick = -0.1f;
    }
    public void SpeicelAttackSucseed()
    {
        _specialAttackBuferTick = -0.1f;
    }
    public void DashSucseed()
    {
        _dashBuferTick = -0.1f;
    }
}