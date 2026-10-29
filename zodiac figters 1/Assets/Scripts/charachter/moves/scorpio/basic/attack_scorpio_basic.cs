using GameAudio;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "attack_scorpio_basic", menuName = "Attacks/scorpio/basic")]


public class attack_scorpio_basic : AttackPearent
{
    [SerializeField] private GameObject[] _hertBoxList;
    [SerializeField] private GameObject[] _hitBoxList;
    [SerializeField] private float[] _timePuse;
    [SerializeField] private SoundEvent _attacksound;


    public override IEnumerator UseMove(playerattack Playerattack, int ID, int ver)
    {
        // start fase
        Playerattack.SetVelocity(Vector2.zero);
        //Playerattack.ChangeHitBox(_hitBoxList[0]);
        
        Playerattack.pmain.PlayAttackAnimation(0);
        _attacksound.Play();
        yield return new WaitForSeconds(_timePuse[0]);
        Playerattack.ChangeHertBox(_hertBoxList[0], ID);
        yield return new WaitForSeconds(_timePuse[1]);
        // finish fase 
        //Playerattack.DestroyHitBox();
        Playerattack.pmain.StopAttackAnimation();
        Playerattack.DestroyHertBox();
        Playerattack.pmain.FinishAttack();
    }
    public override void anoncehit(Collider2D collision)
    {
        return;
    }
}
