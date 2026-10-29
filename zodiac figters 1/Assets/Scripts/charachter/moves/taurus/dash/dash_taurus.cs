using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "dash_taurus" , menuName = "Attacks/taurus/dash")]


public class dash_taurus  : AttackPearent
{
    [SerializeField] private GameObject[] _hertBoxList;
    [SerializeField] private GameObject[] _hitBoxList;
    [SerializeField] private float[] _timePuse;
    [SerializeField] private float _dashPower;

    public override IEnumerator UseMove(playerattack pattack, int ID, int ver)
    {
        // start fase
        
        switch (ver)
        {
            case 0:
                pattack.SetVelocity(_dashPower * Vector2.right);
                pattack.ChangeHertBox(_hertBoxList[0],ID);
                pattack.pmain.PlayAttackAnimation(4);
                break;

            case 1:
                pattack.SetVelocity(_dashPower * Vector2.left);
                break;
        }
        pattack.pmain.Setinvincible(true);
        pattack.pmain.SetPush(false);

        yield return new WaitForSeconds(_timePuse[0]);
        pattack.SetVelocity(Vector2.zero);

        // finish fase 
        pattack.DestroyHitBox();
        pattack.DestroyHertBox();
        pattack.pmain.Setinvincible(false);
        pattack.pmain.SetPush(true);
        pattack.pmain.FinishAttack();
        pattack.pmain.StopAttackAnimation();
    }
    public override void anoncehit(Collider2D collision)
    {

    }
}
