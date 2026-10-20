using GameAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class playerattack : MonoBehaviour
{
    [SerializeField] public playermain pmain;
    private int attackB;
    private int attackBA;
    private int attackS;
    private int attackSA;
    public GameObject player;
    [SerializeField] GameObject hertObject;
    [SerializeField] GameObject hitObject;
    private int activeAtack;

    [SerializeField] private SoundEvent[] _SoundList;

    private Coroutine[] IEActiveAttack=new Coroutine[4];
    private Coroutine IEDash;

    [SerializeField] AttackPearent[] _attackList;
    [SerializeField] AttackPearent _dash;

    private GameObject carentHitBox;
    private GameObject carentHertBox;
    private  int[] AttackId = new int[5];
    private int AttackDitectId;
    private Collider2D AttackDitectCollider;
    private float[] _coldownList= new float[5];
    private float[] _coldowntick= new float[5];
    private float[] _ChainBafer = new float[5];
    private float[] _chainBaferTick= new float[5];
    private float[] _chainCount= new float[5];

    private HashSet<ColInt> colidDetectList;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = pmain.player;
        for (int i = 0; i < _attackList.Length; i++)
        {
            _coldownList[i] = _attackList[i]._cooldown;
            _ChainBafer[i] = _attackList[i]._chainCount;
            _chainBaferTick[i] = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        cooldownCounter();
    }
   
    public void BasicAttack()
    {
        
        int D = 0;
        if (_coldowntick[D] < _coldownList[D])
        {
            pmain.FinishAttack();
            return;
        }
        if(pmain.pimput != null)
        {
            

            pmain.pimput.BasicAttackSucseed();
        }
        _coldowntick[D] = 0;
        AttackId[D] = _attackList[D].GeneratAttackId();

        switch (_chainCount[D])
        {
            case 0:
                IEActiveAttack[D] = StartCoroutine(_attackList[D].UseMove(this, AttackId[D], 0));
                _chainBaferTick[D] = _ChainBafer[D];
                _chainCount[D] = 1;
                break;
            case 1:
                IEActiveAttack[D] = StartCoroutine(_attackList[D].UseMove(this, AttackId[D], 1));
                _chainBaferTick[D] = _ChainBafer[D];
                _chainCount[D] = 2;
                break;
            case 2:
                IEActiveAttack[D] = StartCoroutine(_attackList[D].UseMove(this, AttackId[D], 2));
                _chainBaferTick[D] = _ChainBafer[D];
                _chainCount[D] = 0;
                break;
        }
        
    }
    public void BasicAttackAir()
    {
        int D = 1;
        if (_coldowntick[D] < _coldownList[D])
        {
            pmain.FinishAttack();
            return;
        }
        if (pmain.pimput != null)
        {
            pmain.pimput.BasicAttackSucseed();
        }
        _coldowntick[D] = 0;
        AttackId[D] = _attackList[D].GeneratAttackId();
        IEActiveAttack[D] = StartCoroutine(_attackList[D].UseMove(this, AttackId[D], 0));
        
    }
    public void SpaicleAttack()
    {
        int D = 2;
        if (_coldowntick[D] < _coldownList[D])
        {
            pmain.FinishAttack();
            return;
        }
        if (pmain.pimput != null)
        {
            pmain.pimput.SpeicelAttackSucseed();
        }
        _coldowntick[D] = 0;
        AttackId[D] = _attackList[D].GeneratAttackId();
        IEActiveAttack[D] = StartCoroutine(_attackList[D].UseMove(this, AttackId[D], 0));
    }
    public void SpaicleAttackAir() 
    {
        int D = 3;
        if (_coldowntick[D] < _coldownList[D])
        {
            pmain.FinishAttack();
            return;
        }
        if (pmain.pimput != null)
        {
            pmain.pimput.SpeicelAttackSucseed();
        }
        _coldowntick[D] = 0;
        AttackId[D] = _attackList[D].GeneratAttackId();
        IEActiveAttack[D] = StartCoroutine(_attackList[D].UseMove(this, AttackId[D], 0));
    }
    public void Dash(int ver)
    {
        int D = 4;
        if (_coldowntick[D] < _coldownList[D])
        {            
            return;
        }
        if (pmain.pimput != null)
        {
            pmain.pimput.DashSucseed();
        }
        _coldowntick[D] = 0;
        AttackId[D] = _dash.GeneratAttackId();
        IEDash= StartCoroutine(_dash.UseMove(this, AttackId[D], ver));
    }
    public void ChangeHitBox(GameObject frame)
    {
        if (carentHitBox != null)
        {
            Destroy(carentHitBox);
        }
        carentHitBox = Instantiate(frame, hitObject.transform);
        carentHitBox.gameObject.tag = pmain.gameObject.tag;
        pmain.SetHitbox(-1);
        Collider2D[] tempcolid = frame.GetComponents<Collider2D>();
        pmain.setColiders(tempcolid);


    }
    public void DestroyHitBox()
    {
        Destroy(carentHitBox);
        carentHitBox = null;
    }
    public void ChangeHertBox(GameObject frame,int id)
    {
        if (carentHertBox != null)
        {
            Destroy(carentHertBox);
        }
        carentHertBox = Instantiate(frame, hertObject.transform);
        
        carentHertBox.GetComponent<damageinflector>()._moveID = id;
    }
    public void DestroyHertBox()
    {
        Destroy(carentHertBox);
        carentHertBox = null;
    }
    public void SetVelocity(Vector2 velocityDirection)
    {
        player.GetComponent<Rigidbody2D>().linearVelocity =Vector2.zero;
        if (pmain.isleft)
        {
            velocityDirection.x = velocityDirection.x * (-1);
        }
        
        player.GetComponent<Rigidbody2D>().AddForce(velocityDirection,ForceMode2D.Impulse);

    }
    public Collider2D ColideCheck(int id)
    {
        Collider2D collision = null;
        if (AttackDitectId == id)
        {
            collision = AttackDitectCollider;
            
        }
        
        return collision;


    }
    public void AttackHitDetected(Collider2D collision, int id)
    {
        //AttackDitectId = id;
        //AttackDitectCollider = collision;
        //colidDetectList.Add(new ColInt(id,collision));
        for (int i = 0; i < _attackList.Length; i++) 
        {
            if (AttackId[i] == id)
            {
                _attackList[i].anoncehit(collision);
            }
        }
    }
    public void ObjectInstantPlayer(GameObject Ob, Vector2 offset, float routate,int ID )
    {
       GameObject iob = Instantiate(Ob);
        if (pmain.gameObject.transform.rotation.y != 0)
        {
            offset.x = offset.x * (-1);
        }
        iob.transform.position = new Vector2 (pmain.transform.position.x + offset.x, pmain.transform.position.y + offset.y);
        iob.transform.rotation = Quaternion.Euler(0, 0, routate);
        if (iob.GetComponent<projectileParent>() !=null )
        {
            projectileParent projectile = iob.GetComponent<projectileParent>();
            projectile._pmain = pmain;
            projectile._pattack = this;
            projectile.playerTag= gameObject.tag;
            projectile._moveID = ID;


        }

    }
    public void ObjectInstantWorld(GameObject Ob, Vector2 position, float routate, int ID)
    {
        //if (pmain.gameObject.transform.rotation.y > 0)
        //{
        //    position.x = position.x * (-1);
        //}
        GameObject iob = Instantiate(Ob);
        iob.transform.position=position;
        iob.transform.rotation = Quaternion.Euler(0, 0, routate);
        if (iob.GetComponent<projectileParent>() != null)
        {
            projectileParent projectile = iob.GetComponent<projectileParent>();
            projectile._pmain = pmain;
            projectile._pattack = this;
            projectile.playerTag = gameObject.tag;
            projectile._moveID = ID;

        }
    }
    private void cooldownCounter()
    {
        for (int i = 0; i<_coldownList.Length; i++) 
        {
            if (_coldowntick[i]< _coldownList[i])
            {

                _coldowntick[i] += Time.deltaTime;
            }

        }
        for (int i = 0; i < _chainBaferTick.Length; i++)
        {
            if (_chainBaferTick[i] >0)
            {
                _chainBaferTick[i] -= Time.deltaTime;
                if(_chainBaferTick[i] <= 0)
                {
                    _chainCount[i] = 0;
                }
            }
            
        }
    }
    public void StopAttack()
    {
        for (int i = 0; i < IEActiveAttack.Length; i++)
        {
            if (IEActiveAttack[i] != null)
            {
                StopCoroutine(IEActiveAttack[i]);
                
            }
        }
        pmain.StopAttackAnimation();
        
    }
    public void PlaySound(int Sound)
    {
        _SoundList[Sound].Play();
    }


}
