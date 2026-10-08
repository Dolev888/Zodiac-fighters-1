using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "attack_aris_basic", menuName = "Attacks/aries/basic")]

public class attack_aris_basic : AttackPearent
{
    [SerializeField] private GameObject[] _hertBoxList;
    [SerializeField] private GameObject[] _hitBoxList;
    [SerializeField] private float[] _timePuse;
    
     
   

    public override IEnumerator UseMove(playerattack Playerattack, int ID, int ver)
    {
        switch (ver)
        {
            case 0:
                Playerattack.SetVelocity(Vector2.zero);
                Playerattack.pmain.PlayAttackAnimation(0);
                Playerattack.ChangeHitBox(_hitBoxList[0]);
                Playerattack.ChangeHertBox(_hertBoxList[0], ID);
                yield return new WaitForSeconds(_timePuse[0]);

                Playerattack.ChangeHertBox(_hertBoxList[1], ID);
                yield return new WaitForSeconds(_timePuse[1]);
                Playerattack.pmain.StopAttackAnimation();
                Playerattack.pmain.FinishAttack();
                Playerattack.DestroyHitBox();
                Playerattack.DestroyHertBox();
                break;
                default:
                Playerattack.SetVelocity(Vector2.zero);
                Playerattack.pmain.PlayAttackAnimation(0);
                Playerattack.ChangeHitBox(_hitBoxList[0]);
                Playerattack.ChangeHertBox(_hertBoxList[0], ID);
                yield return new WaitForSeconds(_timePuse[0]);

                Playerattack.ChangeHertBox(_hertBoxList[1], ID);
                yield return new WaitForSeconds(_timePuse[1]);
                Playerattack.pmain.StopAttackAnimation();
                Playerattack.pmain.FinishAttack();
                Playerattack.DestroyHitBox();
                Playerattack.DestroyHertBox();
                break;
        }
        
        
       
        
    }

    public override void anoncehit(Collider2D collision)
    {
        return;
    }


}
