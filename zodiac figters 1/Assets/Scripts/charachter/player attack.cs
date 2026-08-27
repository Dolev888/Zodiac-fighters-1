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

    private Coroutine[] IEBasicAttack=new Coroutine[4];

    [SerializeField] AttackPearent[] _attackList;

    private GameObject carentHitBox;
    private GameObject carentHertBox;
    private  int[] AttackId = new int[4];
    private int AttackDitectId;
    private Collider2D AttackDitectCollider;
    private float[] _coldownList= new float[4];
    private float[] _coldowntick= new float[4];

    private HashSet<ColInt> colidDetectList;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = pmain.player;
        for (int i = 0; i < _attackList.Length; i++)
        {
            _coldownList[i] = _attackList[i]._cooldown;
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
        _coldowntick[D] = 0;
        AttackId[D] = _attackList[D].GeneratAttackId();
        
        IEBasicAttack[D] = StartCoroutine(_attackList[D].UseMove(this, AttackId[D]));
    }
    public void BasicAttackAir()
    {
        int D = 1;
        if (_coldowntick[D] < _coldownList[D])
        {
            pmain.FinishAttack();
            return;
        }
        _coldowntick[D] = 0;
        AttackId[D] = _attackList[D].GeneratAttackId();
        IEBasicAttack[D] = StartCoroutine(_attackList[D].UseMove(this, AttackId[D]));
        
    }
    public void SpaicleAttack()
    {
        int D = 2;
        if (_coldowntick[D] < _coldownList[D])
        {
            pmain.FinishAttack();
            return;
        }
        _coldowntick[D] = 0;
        AttackId[D] = _attackList[D].GeneratAttackId();
        IEBasicAttack[D] = StartCoroutine(_attackList[D].UseMove(this, AttackId[D]));
    }
    public void SpaicleAttackAir() 
    {
        int D = 3;
        if (_coldowntick[D] < _coldownList[D])
        {
            pmain.FinishAttack();
            return;
        }
        _coldowntick[D] = 0;
        AttackId[D] = _attackList[D].GeneratAttackId();
        IEBasicAttack[D] = StartCoroutine(_attackList[D].UseMove(this, AttackId[D]));
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
    public void ObjectInstantPlayer(GameObject Ob, Vector2 offset, float routate )
    {
       GameObject iob = Instantiate(Ob);
        if (pmain.gameObject.transform.rotation.y > 0)
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
        }
    }
    public void ObjectInstantWorld(GameObject Ob, Vector2 position, float routate)
    {
        if (pmain.gameObject.transform.rotation.y > 0)
        {
            position.x = position.x * (-1);
        }
        GameObject iob = Instantiate(Ob);
        iob.transform.position=position;
        iob.transform.rotation = Quaternion.Euler(0, 0, routate);
        if (iob.GetComponent<projectileParent>() != null)
        {
           
            iob.GetComponent<projectileParent>()._pmain = pmain;
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
    }
}
