using System.Collections;
using UnityEngine;
using System.Collections.Generic;

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

    private Coroutine IEBasicAttack;

    [SerializeField] AttackPearent[] _attackList;

    private GameObject carentHitBox;
    private GameObject carentHertBox;
    private  int AttackId = 0;
    private int AttackDitectId;
    private Collider2D AttackDitectCollider;

    private HashSet<ColInt> colidDetectList;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = pmain.player;
    }

    // Update is called once per frame
    void Update()
    {
    }
   
    public void BasicAttack()
    {
        
        AttackId = _attackList[0].GeneratAttackId();
        IEBasicAttack = StartCoroutine(_attackList[0].UseMove(this, AttackId));
       

    }
    public void BasicAttackAir()
    {
        AttackId = _attackList[1].GeneratAttackId();
        IEBasicAttack = StartCoroutine(_attackList[1].UseMove(this, AttackId));
    }
    public void SpaicleAttack()
    {
        AttackId = _attackList[2].GeneratAttackId();
        IEBasicAttack = StartCoroutine(_attackList[2].UseMove(this, AttackId));
    }
    public void SpaicleAttackAir() 
    {
        AttackId = _attackList[3].GeneratAttackId();
        IEBasicAttack = StartCoroutine(_attackList[3].UseMove(this, AttackId));
    }
    public void ChangeHitBox(GameObject frame)
    {
        if (carentHitBox != null)
        {
            Destroy(carentHitBox);
        }
        carentHitBox = Instantiate(frame, hitObject.transform);
        
    }
    public void DestroyHitBox()
    {
        Destroy(carentHitBox);
        carentHitBox = null;
    }
    public void ChangeHertBox(GameObject frame)
    {
        if (carentHertBox != null)
        {
            Destroy(carentHertBox);
        }
        carentHertBox = Instantiate(frame, hertObject.transform);
        
        carentHertBox.GetComponent<damageinflector>()._moveID = AttackId;
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
        AttackDitectId = id;
        AttackDitectCollider = collision;
    }
    public void ObjectInstantPlayer(GameObject Ob, Vector2 offset, float routate )
    {
       GameObject iob = Instantiate(Ob);
        iob.transform.position = new Vector2 (pmain.transform.position.x + offset.x, pmain.transform.position.y + offset.y);
        iob.transform.rotation = new Quaternion(0, 0, routate, 0);
    }
    public void ObjectInstantWorld(GameObject Ob, Vector2 position, float routate)
    {
        GameObject iob = Instantiate(Ob);
        iob.transform.position=position;
        iob.transform.rotation = Quaternion.Euler(0, 0, routate);
    }
}
