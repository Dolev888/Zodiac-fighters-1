using GameAudio;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "attack_aris_spaiceljump", menuName = "Attacks/aries/special")]

public class attack_aries_spaicel : AttackPearent
{
    [SerializeField] private GameObject[] _hertBoxList;
    [SerializeField] private GameObject[] _hitBoxList;
    [SerializeField] private float[] _timePuse;
    [SerializeField] private GameObject _firePunchOB;
    [SerializeField] private Vector2 _offSet;
    [SerializeField] private float _engel;
    [SerializeField] private SoundEvent _attacksound;

    public override IEnumerator UseMove(playerattack pattack, int ID, int ver)
    {
        pattack.SetVelocity(Vector2.zero);

        _attacksound.Play();
        pattack.ObjectInstantPlayer(_firePunchOB, _offSet, _engel, ID);
        if (pattack._upgradeBoolList[3])
        {
            pattack.ObjectInstantPlayer(_firePunchOB, _offSet, _engel+45, ID);
            pattack.ObjectInstantPlayer(_firePunchOB, _offSet, _engel-45, ID);
        }

        yield return new WaitForSeconds(_timePuse[0]);
      
        
        pattack.pmain.FinishAttack();
    }
    public override void anoncehit(Collider2D collision)
    {
        return;
    }
}
