using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "attack_aris_spaiceljump", menuName = "Attacks/aries/special")]

public class attack_aries_spaicel : AttackPearent
{
    [SerializeField] private GameObject[] _hertBoxList;
    [SerializeField] private GameObject[] _hitBoxList;
    [SerializeField] private float[] _timePuse;
    [SerializeField] private GameObject _firePunchOB;
    [SerializeField] private Vector2 _offSet;
    [SerializeField] private float _engel;


    public override IEnumerator UseMove(playerattack pattack, int ID, int ver)
    {
        pattack.SetVelocity(Vector2.zero);

        pattack.PlaySound(0);
        pattack.ObjectInstantPlayer(_firePunchOB, _offSet, _engel, ID);

        yield return new WaitForSeconds(_timePuse[0]);
      
        
        pattack.pmain.FinishAttack();
    }
    public override void anoncehit(Collider2D collision)
    {
        return;
    }
}
