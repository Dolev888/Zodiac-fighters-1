using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "attak_scorpio_spaiceljump" , menuName = "Attacks/scorpio/spaiceljump")]


public class attak_scorpio_spaiceljump  : AttackPearent
{
    [SerializeField] private GameObject[] _hertBoxList;
    [SerializeField] private GameObject[] _hitBoxList;
    [SerializeField] private float[] _timePuse;
    [SerializeField] private GameObject _icaGazer;
    private bool _ifhit = false;
    private bool _canhit = false;


    public override IEnumerator UseMove(playerattack Playerattack, int ID, int ver)
    {
        // start fase
        Playerattack.SetVelocity(Vector2.zero);
        Playerattack.pmain.SetGravity(0);
        //Playerattack.ChangeHitBox(_hitBoxList[0]);
        //Playerattack.ChangeHertBox(_hertBoxList[0], ID);

        yield return new WaitForSeconds(_timePuse[0]);
        Playerattack.ObjectInstantWorld(_icaGazer,Playerattack.pmain.transform.position,0,ID);
        float clock = _timePuse[1];
        _canhit = true;
        while (!_ifhit && clock>0  )
        {
            clock -=Time.deltaTime;
            yield return null;
        }
        // finish fase 
        //Playerattack.DestroyHitBox();
        //Playerattack.DestroyHertBox();
        Playerattack.pmain.StopAttackAnimation();
        Playerattack.pmain.ResetGravity();
        Playerattack.pmain.FinishAttack();
    }
    public override void anoncehit(Collider2D collision)
    {
        if (!_canhit || collision == null) return;
        if (collision.CompareTag("ground") || collision.gameObject.layer == LayerMask.NameToLayer("hit"))
        {

            _ifhit = true;
        }
    }
}
