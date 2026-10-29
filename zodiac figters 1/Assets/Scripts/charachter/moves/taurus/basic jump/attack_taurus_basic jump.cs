using GameAudio;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "attack_taurus_basicjump" , menuName = "Attacks/taurus/basic jump")]


public class attack_taurus_basicjump  : AttackPearent
{
    [SerializeField] private GameObject[] _hertBoxList;
    [SerializeField] private GameObject[] _hitBoxList;
    [SerializeField] private float[] _timePuse;
    [SerializeField] private Vector2 _downForce;
    private bool _ifhit = false;
    private bool _canhit = false;
    [SerializeField] private SoundEvent _attacksound;
    public override IEnumerator UseMove(playerattack pattack, int ID, int ver)
    {
        // start fase
        pattack.SetVelocity(Vector2.zero);
        pattack.pmain.SetGravity(0);
        pattack.pmain.PlayAttackAnimation(1);
        yield return new WaitForSeconds(_timePuse[0]);
        pattack.pmain.ResetGravity();
        pattack.SetVelocity(_downForce);
        pattack.pmain.Setinvincible(true);
        pattack.ChangeHertBox(_hertBoxList[0], ID);
        
        float clock = _timePuse[1];
        _canhit = true;
        _ifhit = false;
        while (!_ifhit && clock > 0 && !pattack.pmain.IsGrounded)
        {

            clock -= Time.deltaTime;
            yield return null;
        }
        Debug.Log(_ifhit + " " + clock + " " + pattack.pmain.IsGrounded);
        _ifhit = false;
        _canhit = false;
        pattack.ChangeHertBox(_hertBoxList[1], ID);
        _attacksound.Play();
        yield return new WaitForSeconds(_timePuse[2]);
        pattack.pmain.Setinvincible(false);
        // finish fase 
        pattack.DestroyHitBox();
        pattack.DestroyHertBox();
        pattack.pmain.FinishAttack();
        pattack.pmain.StopAttackAnimation();
    }
    public override void anoncehit(Collider2D collision)
    {
        if (!_canhit || collision == null) return;
        if (collision.CompareTag("ground") || collision.gameObject.layer == LayerMask.NameToLayer("hit"))
        {

            _ifhit = true;
        }
    }
}
