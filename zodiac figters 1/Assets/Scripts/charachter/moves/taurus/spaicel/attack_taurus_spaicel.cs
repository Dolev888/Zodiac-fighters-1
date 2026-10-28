using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "attack_taurus_spaicel" , menuName = "Attacks/taurus/spaicel")]


public class attack_taurus_spaicel  : AttackPearent
{
    [SerializeField] private GameObject[] _hertBoxList;
    [SerializeField] private GameObject[] _hitBoxList;
    [SerializeField] private float[] _timePuse;
    [SerializeField] private GameObject _earthWave;
    [SerializeField] private Vector2 _offSet;
    [SerializeField] private float _engel;


    public override IEnumerator UseMove(playerattack pattack, int ID, int ver)
    {
        // start fase
        pattack.SetVelocity(Vector2.zero);
        //Playerattack.ChangeHitBox(_hitBoxList[0]);
        //Playerattack.ChangeHertBox(_hertBoxList[0], ID);

        pattack.pmain.PlayAttackAnimation(2);
        yield return new WaitForSeconds(_timePuse[0]);
        pattack.ObjectInstantPlayer(_earthWave, _offSet, _engel, ID);
        yield return new WaitForSeconds(_timePuse[1]);

        // finish fase 
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
