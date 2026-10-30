using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "dash_scorpio" , menuName = "Attacks/scorpio/dash")]


public class dash_scorpio  : AttackPearent
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
                break;

            case 1:
                pattack.SetVelocity(_dashPower * Vector2.left);
                break;
        }
        pattack.pmain.Setinvincible(true);
        pattack.pmain.SetPush(false);

        //Playerattack.pmain.playDash();
        //Playerattack.ChangeHitBox(_hitBoxList[0]);
        //Playerattack.ChangeHertBox(_hertBoxList[0],ID);

        yield return new WaitForSeconds(_timePuse[0]);
        pattack.SetVelocity(Vector2.zero);
        // finish fase 
        //Playerattack.DestroyHitBox();
        //Playerattack.DestroyHertBox();
        pattack.pmain.Setinvincible(false);
        pattack.pmain.SetPush(true);
        pattack.pmain.FinishAttack();
    }
    public override void anoncehit(Collider2D collision)
    {

    }
}
