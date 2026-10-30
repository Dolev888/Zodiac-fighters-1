using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class FighterDamage : MonoBehaviour
{
    [SerializeField] private float damagePercentage = 0f;
    [SerializeField] private float maxDamage = 100f;

    [SerializeField] private float _fetiugPrecent;

    [SerializeField] private MatchManager matchManager;

    [SerializeField] private bool isPlayer;
    private playermain pmain;
    private float stunClock;
    private float stunTime;

    public float DamagePercentage => damagePercentage;
    public float MaxDamage => maxDamage;
    private HashSet<int> takenId = new HashSet<int>();

    private void Start()
    {
        pmain = GetComponent<playermain>();
    }
    private void Update()
    {
        if (Time.timeScale == 0f) return;
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10f, -888, 0);
        }
        switch (pmain.CurentState)
        {
            case playermain.STATE.STUN:
                StunTimer();
                break;
            default:
                break;
        }
    }
    public void TakeDamage(float amount, int id, float cooldown)
    {

        if (CheckId(id) || pmain._isInvincable) return;
        if (cooldown != 0)
        {
            StartCoroutine(countdown(cooldown, id));
        }
        if (amount <= 0f) return;
        if (id != -1) CollectedDataAnalysis.Hit(isPlayer, amount);   // playtest log: id -1 is the fall damage, not a hit from the other fighter
        damagePercentage += amount;
        pmain.playhert();

        if (damagePercentage >= maxDamage)
        {
            damagePercentage = maxDamage; //insurinng damage can't be higher then maxdamge;
            Lose(); // if reached max damge you lose
        }

        if (damagePercentage >= _fetiugPrecent)
        {
            pmain.SetIdleAnim(1);
        }
        else
        {
            pmain.SetIdleAnim(0);
        }
        Debug.Log($"{gameObject.name} Damage: {damagePercentage} %");
    }
    private IEnumerator countdown(float time, int id)
    {
        takenId.Add(id);
        yield return new WaitForSeconds(time);
        takenId.Remove(id);
    }

    private void Lose()
    {
        Debug.Log($"{gameObject.name} has lost ");
        if (matchManager == null)
        {
            return;

        }
        // if this fighter is the player then player lost and if it is the enemy then player won.
        matchManager.EndMatch(!isPlayer, gameObject);
    }
    private bool CheckId(int id)
    {
        if (takenId.Contains(id)) return true;
        return false;

    }
    public void TakeStun(float stunDuretion)
    {
        if (pmain == null || pmain._isInvincable) return;
        stunTime = stunDuretion;
        stunClock = 0;
        pmain.EnterStun();

    }
    private void StunTimer()
    {
        if (stunClock >= stunTime)
        {
            stunTime = 0;
            stunClock = 0;
            pmain.ExitStun();
        }
        stunClock += Time.deltaTime;
    }
    public void TakeKnockback(Vector2 nockForce)
    {
        if (pmain._isInvincable) return;
        pmain.rigidP.linearVelocity = nockForce;
    }
    public void addKnockback(Vector2 nockForce)
    {
        if (pmain._isInvincable) return;
        if (pmain.CurentState == playermain.STATE.KNOKCBACK || pmain.CurentState == playermain.STATE.STUN)
        {
            pmain.rigidP.linearVelocity += nockForce;
        }
        else
        {
            TakeKnockback(nockForce);
        }
    }

    public void Setup(MatchManager mm, bool player)
    {
        matchManager = mm;
        isPlayer = player;
    }

}