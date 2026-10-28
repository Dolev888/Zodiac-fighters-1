using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "attack_taurus_spaiceljump" , menuName = "Attacks/taurus/spaiceljump")]


public class attack_taurus_spaiceljump  : AttackPearent
{
    [SerializeField] private GameObject[] _hertBoxList;
    [SerializeField] private GameObject[] _hitBoxList;
    [SerializeField] private float[] _timePuse;
    [SerializeField] private GameObject _rockWallOb;
    [SerializeField] private Vector2 _WallOffset;
    


    public override IEnumerator UseMove(playerattack pattack, int ID, int ver)
    {
        // start fase
        pattack.SetVelocity(Vector2.zero);
        pattack.pmain.SetGravity(0);
        pattack.pmain.PlayAttackAnimation(3);
        yield return new WaitForSeconds(_timePuse[0]);
        pattack.ObjectInstantPlayer(_rockWallOb, _WallOffset, 0, ID);
        yield return new WaitForSeconds(_timePuse[1]);


        // finish fase 
        pattack.pmain.ResetGravity();
        pattack.DestroyHitBox();
        pattack.DestroyHertBox();
        pattack.pmain.FinishAttack();
        pattack.pmain.StopAttackAnimation();
    }
    public override void anoncehit(Collider2D collision)
    {
        return;
    }
}
