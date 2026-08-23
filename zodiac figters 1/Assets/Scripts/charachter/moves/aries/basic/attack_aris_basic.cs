using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "attack_aris_basic", menuName = "Attacks/aris/basic")]

public class attack_aris_basic : AttackPearent
{
    [SerializeField] private GameObject[] _hertBoxList;
    [SerializeField] private GameObject[] _hitBoxList;
    [SerializeField] private float[] _timePuse;


    public override IEnumerator UseMove(playerattack Playerattack, int ID)
    {
        Playerattack.SetVelocity(Vector2.zero);
        Playerattack.ChangeHitBox(_hitBoxList[0]);
        Playerattack.ChangeHertBox(_hertBoxList[0]);
        yield return new WaitForSeconds(_timePuse[0]);
        Playerattack.ChangeHertBox(_hertBoxList[1]);
        yield return new WaitForSeconds(_timePuse[1]);
        Playerattack.DestroyHitBox();
        Playerattack.DestroyHertBox();
        Playerattack.pmain.FinishAttack();
    }
    

}
