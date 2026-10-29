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
    [SerializeField] private Vector2 _offSet;
    [SerializeField] private float _engel;
    [SerializeField] private SoundEvent _attacksound;

    public override IEnumerator UseMove(playerattack Playerattack, int ID, int ver)
    {
        Playerattack.SetVelocity(Vector2.zero);
        Playerattack.pmain.SetGravity(0);

        Playerattack.pmain.PlayAttackAnimation(3);

        yield return new WaitForSeconds(_timePuse[0]);
        Playerattack.ObjectInstantPlayer(_fireWaveOB, _offSet, _engel, ID);
        _attacksound.Play();
        yield return new WaitForSeconds(_timePuse[1]);
        Playerattack.pmain.StopAttackAnimation();
        Playerattack.pmain.ResetGravity();
        Playerattack.pmain.FinishAttack();
    }
    public override void anoncehit(Collider2D collision)
    {
        return;
    }
}
