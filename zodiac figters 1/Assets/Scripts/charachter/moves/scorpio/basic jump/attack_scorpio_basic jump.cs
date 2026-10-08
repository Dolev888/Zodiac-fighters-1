using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "attack_scorpio_basicjump" , menuName = "Attacks/scorpio/basicjump")]


public class attack_scorpio_basicjump  : AttackPearent
{
    [SerializeField] private GameObject[] _hertBoxList;
    [SerializeField] private GameObject[] _hitBoxList;
    [SerializeField] private float[] _timePuse;


    public override IEnumerator UseMove(playerattack Playerattack, int ID, int ver)
    {
        // start fase
        Playerattack.SetVelocity(Vector2.zero);
        Playerattack.pmain.SetGravity(0);
        Playerattack.ChangeHitBox(_hitBoxList[0]);
        Playerattack.ChangeHertBox(_hertBoxList[0], ID);
        Playerattack.pmain.PlayAttackAnimation(1);
        yield return new WaitForSeconds(_timePuse[0]);
        Playerattack.ChangeHitBox(_hitBoxList[1]);
        yield return new WaitForSeconds(_timePuse[1]);

        // finish fase 
        Playerattack.pmain.StopAttackAnimation();
        Playerattack.pmain.ResetGravity();
        Playerattack.DestroyHitBox();
        Playerattack.DestroyHertBox();
        Playerattack.pmain.FinishAttack();
    }
    public override void anoncehit(Collider2D collision)
    {
        return;
    }
}
