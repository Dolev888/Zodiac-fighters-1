using GameAudio;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "attack_aris_basic", menuName = "Attacks/aries/basic")]

public class attack_aris_basic : AttackPearent
{
    [SerializeField] private GameObject[] _hertBoxList;
    [SerializeField] private GameObject[] _hertBoxListUpgraded;
    [SerializeField] private GameObject[] _hitBoxList;
    [SerializeField] private float[] _timePuse;


    [SerializeField] private SoundEvent _attacksound;

    public override IEnumerator UseMove(playerattack pattack, int ID, int ver)
    {
        

        switch (ver)
        {
            case 0:
                _attacksound.Play();
                pattack.SetVelocity(Vector2.zero);
                pattack.pmain.PlayAttackAnimation(0);
                pattack.ChangeHitBox(_hitBoxList[0]);
                pattack.ChangeHertBox(_hertBoxList[0], ID);
                yield return new WaitForSeconds(_timePuse[0]);

                pattack.ChangeHertBox(_hertBoxList[1], ID);
                yield return new WaitForSeconds(_timePuse[1]);
                pattack.pmain.StopAttackAnimation();
                pattack.pmain.FinishAttack();
                pattack.DestroyHitBox();
                pattack.DestroyHertBox();
                break;
            case 2:
                _attacksound.Play();
                
                pattack.SetVelocity(Vector2.zero);
                pattack.pmain.PlayAttackAnimation(0);
                pattack.pmain.SetAttackAnimationVersion(2);
                pattack.ChangeHitBox(_hitBoxList[0]);
                pattack.ChangeHertBox(_hertBoxList[2], ID);
                yield return new WaitForSeconds(_timePuse[0]);
                if (pattack._upgradeBoolList[1]) pattack.ChangeHertBox(_hertBoxListUpgraded[0], ID);
                else
                       pattack.ChangeHertBox(_hertBoxList[3], ID);
                yield return new WaitForSeconds(_timePuse[1]);
                pattack.pmain.StopAttackAnimation();
                pattack.pmain.FinishAttack();
                pattack.DestroyHitBox();
                pattack.DestroyHertBox();

                break;
            default:
                _attacksound.Play();
                pattack.SetVelocity(Vector2.zero);
                pattack.pmain.PlayAttackAnimation(0);
                pattack.ChangeHitBox(_hitBoxList[0]);
                pattack.ChangeHertBox(_hertBoxList[0], ID);
                yield return new WaitForSeconds(_timePuse[0]);

                pattack.ChangeHertBox(_hertBoxList[1], ID);
                yield return new WaitForSeconds(_timePuse[1]);
                pattack.pmain.StopAttackAnimation();
                pattack.pmain.FinishAttack();
                pattack.DestroyHitBox();
                pattack.DestroyHertBox();
                break;
        }




    }

    public override void anoncehit(Collider2D collision)
    {
        return;
    }


}
