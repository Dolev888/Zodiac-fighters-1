using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "dash_aris" , menuName = "Attacks/aries/dash")]


public class dash_aris  : AttackPearent
{
    [SerializeField] private GameObject[] _hertBoxList;
    [SerializeField] private GameObject[] _hitBoxList;
    [SerializeField] private float[] _timePuse;
    [SerializeField] private float _dashPower;


    public override IEnumerator UseMove(playerattack Playerattack, int ID, int ver)
    {
        // start fase
        switch (ver)
        {
            case 0:
                Playerattack.SetVelocity(_dashPower * Vector2.right);
                break;

            case 1:
                Playerattack.SetVelocity(_dashPower * Vector2.left);
                break;
        }
        Playerattack.pmain.Setinvincible(true);
        
        //Playerattack.pmain.playDash();
        //Playerattack.ChangeHitBox(_hitBoxList[0]);
        //Playerattack.ChangeHertBox(_hertBoxList[0],ID);

        yield return new WaitForSeconds(_timePuse[0]);
        Playerattack.SetVelocity(Vector2.zero);
        // finish fase 
        //Playerattack.DestroyHitBox();
        //Playerattack.DestroyHertBox();
        Playerattack.pmain.Setinvincible(false);
        Playerattack.pmain.FinishAttack();
    }
    public override void anoncehit(Collider2D collision)
    {

    }
}
