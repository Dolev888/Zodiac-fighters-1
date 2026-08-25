using UnityEngine;
using System.Collections;


[CreateAssetMenu(fileName = "attack_aris_basicJump", menuName = "Attacks/aries/basic jump")]
public class attack_aris_basicJump : AttackPearent
{
    [SerializeField] private GameObject[] _hertBoxList;
    [SerializeField] private GameObject[] _hitBoxList;
    [SerializeField] private float[] _timePuse;
    [SerializeField] private Vector2 _velocitiAdd;


    public override IEnumerator UseMove(playerattack Playerattack,int ID)
    {
        Debug.Log(ID);
        Playerattack.ChangeHitBox(_hitBoxList[0]);
        Playerattack.ChangeHertBox(_hertBoxList[0]);
        yield return new WaitForSeconds(_timePuse[0]);
        Playerattack.SetVelocity(_velocitiAdd);
        float clock = _timePuse[1];
        while (!CheckHit(Playerattack, ID)&& clock>0)
        {
            clock -= Time.deltaTime;
            yield return null;
        }
        Playerattack.DestroyHitBox();
        Playerattack.DestroyHertBox();
        Playerattack.pmain.FinishAttack();
    }
    private bool CheckHit(playerattack Playerattack, int ID)
    {
        Collider2D collision = Playerattack.ColideCheck(ID);
        if (collision == null) return false;
        if (collision.CompareTag("ground") || collision.gameObject.layer == LayerMask.NameToLayer("hit"))
        {
            return true;
        }
        return false;
    }
    
}
