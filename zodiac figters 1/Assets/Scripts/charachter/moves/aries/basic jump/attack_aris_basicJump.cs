using GameAudio;
using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "attack_aris_basicJump", menuName = "Attacks/aries/basic jump")]
public class attack_aris_basicJump : AttackPearent
{
    [SerializeField] private GameObject[] _hertBoxList;
    [SerializeField] private GameObject[] _hertBoxListUpgrade;
    [SerializeField] private GameObject[] _hitBoxList;
    [SerializeField] private float[] _timePuse;
    [SerializeField] private Vector2 _velocitiAdd;
    private bool _ifhit=false;
    private bool _canhit = false;
    [SerializeField] private SoundEvent _attacksound;

    public override IEnumerator UseMove(playerattack pattack, int ID, int ver)
    {
        Debug.Log(ID);
        pattack.SetVelocity(Vector2.zero);
        pattack.ChangeHitBox(_hitBoxList[0]);
        if (pattack._upgradeBoolList[2]) pattack.ChangeHertBox(_hertBoxListUpgrade[0], ID);
        else
            pattack.ChangeHertBox(_hertBoxList[0], ID);
        yield return new WaitForSeconds(_timePuse[0]);
        pattack.pmain.PlayAttackAnimation(1);
        pattack.SetVelocity(_velocitiAdd);
        _attacksound.Play();
        float clock = _timePuse[1];
        _canhit = true;
        while (!_ifhit && clock > 0 && !pattack.pmain.IsGrounded)
        {

            clock -= Time.deltaTime;
            yield return null;
        }
       
        _ifhit = false;
        _canhit = false;
        pattack.pmain.StopAttackAnimation();
        pattack.DestroyHitBox();
        pattack.DestroyHertBox();
        pattack.pmain.FinishAttack();
    }
    private bool CheckHit(playerattack Playerattack, int ID)
    {
        Collider2D collision = Playerattack.ColideCheck(ID);
        if (collision == null) return false;
        if (collision.CompareTag("ground") || collision.gameObject.layer == LayerMask.NameToLayer("hit"))
        {
            return true;
        }
        return false;
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
