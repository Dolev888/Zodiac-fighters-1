using GameAudio;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "attack_aris_spaiceljump" , menuName = "Attacks/aries/special jump")]


public class attack_aris_spaiceljump  : AttackPearent
{
    [SerializeField] private GameObject[] _hertBoxList;
    [SerializeField] private GameObject[] _hitBoxList;
    [SerializeField] private float[] _timePuse;
    [SerializeField] private GameObject _fireWaveOB;
    [SerializeField] private GameObject _fireWaveOBPlus;
    [SerializeField] private Vector2 _offSet;
    [SerializeField] private float _engel;
    [SerializeField] private SoundEvent _attacksound;

    public override IEnumerator UseMove(playerattack pattack, int ID, int ver)
    {
        pattack.SetVelocity(Vector2.zero);
        pattack.pmain.SetGravity(0);

        pattack.pmain.PlayAttackAnimation(3);

        yield return new WaitForSeconds(_timePuse[0]);
        if (pattack._upgradeBoolList[4]) { pattack.ObjectInstantPlayer(_fireWaveOBPlus, _offSet, _engel, ID); }
        else
        pattack.ObjectInstantPlayer(_fireWaveOB, _offSet, _engel, ID);
        _attacksound.Play();
        yield return new WaitForSeconds(_timePuse[1]);
        pattack.pmain.StopAttackAnimation();
        pattack.pmain.ResetGravity();
        pattack.pmain.FinishAttack();
    }
    public override void anoncehit(Collider2D collision)
    {
        return;
    }
}
