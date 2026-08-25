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


    public override IEnumerator UseMove(playerattack Playerattack, int ID)
    {
        Playerattack.SetVelocity(Vector2.zero);

        Playerattack.ObjectInstantPlayer(_firePunchOB, _offSet, _engel);

        yield return new WaitForSeconds(_timePuse[0]);
      
        
        Playerattack.pmain.FinishAttack();
    }
}
