using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "attack_taurus_basic" , menuName = "Attacks/taurus/basic")]


public class attack_taurus_basic  : AttackPearent
{
    [SerializeField] private GameObject[] _hertBoxList;
    [SerializeField] private GameObject[] _hitBoxList;
    [SerializeField] private float[] _timePuse;


    public override IEnumerator UseMove(playerattack Playerattack, int ID, int ver)
    {
        switch (ver)
        {
            case 0:
                // start fase
                Playerattack.SetVelocity(Vector2.zero);
                Playerattack.pmain.PlayAttackAnimation(0);
                yield return new WaitForSeconds(_timePuse[0]);
                Playerattack.ChangeHertBox(_hertBoxList[0], ID);
                yield return new WaitForSeconds(_timePuse[1]);
                Playerattack.ChangeHertBox(_hertBoxList[1], ID);
                yield return new WaitForSeconds(_timePuse[2]);

                // finish fase 
                Playerattack.DestroyHitBox();
                Playerattack.DestroyHertBox();
                Playerattack.pmain.StopAttackAnimation();
                Playerattack.pmain.FinishAttack();
                break;
            case 1:
                // start fase
                Playerattack.SetVelocity(Vector2.zero);
                Playerattack.pmain.PlayAttackAnimation(0);
                Playerattack.pmain.SetAttackAnimationVersion(1);
                yield return new WaitForSeconds(_timePuse[3]);
                Playerattack.ChangeHertBox(_hertBoxList[2], ID);
                yield return new WaitForSeconds(_timePuse[4]);
                Playerattack.ChangeHertBox(_hertBoxList[3], ID);
                yield return new WaitForSeconds(_timePuse[5]);

                // finish fase 
                Playerattack.DestroyHitBox();
                Playerattack.DestroyHertBox();
                Playerattack.pmain.StopAttackAnimation();
                Playerattack.pmain.FinishAttack();
                break;
            default:
                Playerattack.SetVelocity(Vector2.zero);
                Playerattack.pmain.PlayAttackAnimation(0);
                yield return new WaitForSeconds(_timePuse[0]);
                Playerattack.ChangeHertBox(_hertBoxList[0], ID);
                yield return new WaitForSeconds(_timePuse[1]);
                Playerattack.ChangeHertBox(_hertBoxList[1], ID);
                yield return new WaitForSeconds(_timePuse[2]);

                // finish fase 
                Playerattack.DestroyHitBox();
                Playerattack.DestroyHertBox();
                Playerattack.pmain.StopAttackAnimation();
                Playerattack.pmain.FinishAttack();
                break; 
        }
    }
    public override void anoncehit(Collider2D collision)
    {
        return;
    }
}
