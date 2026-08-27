using UnityEngine;
using System.Collections;


[CreateAssetMenu(fileName = "attack_aris_basicJump", menuName = "Attacks/aries/basic jump")]
public class attack_aris_basicJump : AttackPearent
{
    [SerializeField] private GameObject[] _hertBoxList;
    [SerializeField] private GameObject[] _hitBoxList;
    [SerializeField] private float[] _timePuse;
    [SerializeField] private Vector2 _velocitiAdd;
    private bool _ifhit=false;
    private bool _canhit = false;


    public override IEnumerator UseMove(playerattack Playerattack,int ID)
    {
        Debug.Log(ID);
        Playerattack.SetVelocity(Vector2.zero);    
        Playerattack.ChangeHitBox(_hitBoxList[0]);
        Playerattack.ChangeHertBox(_hertBoxList[0], ID);
        yield return new WaitForSeconds(_timePuse[0]);
        Playerattack.pmain.PlayAttackAnimation(1);
        Playerattack.SetVelocity(_velocitiAdd);
        float clock = _timePuse[1];
        _canhit =true;
        while (!_ifhit && clock > 0 && !Playerattack.pmain.IsGrounded)
        {
            
            clock -= Time.deltaTime;
            yield return null;
        }
        _ifhit = false;
        _canhit = false;
        Playerattack.pmain.StopAttackAnimation();
        Playerattack.DestroyHitBox();
        Playerattack.DestroyHertBox();
        Playerattack.pmain.FinishAttack();
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
