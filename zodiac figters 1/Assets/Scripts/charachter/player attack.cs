using System.Collections;
using UnityEngine;

public class playerattack : MonoBehaviour
{
    [SerializeField] private playermain playermain;
    private int attackB;
    private int attackBA;
    private int attackS;
    private int attackSA;
    private GameObject player;
    [SerializeField] GameObject hertObject;
    [SerializeField] GameObject hitObject;

    private Coroutine IEBasicAttack;

    [SerializeField] AttackPearent[] _attackList;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = playermain.player;
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    public void BasicAttack()
    {
        IEBasicAttack = StartCoroutine(_attackList[0].UseMove(hertObject, hitObject));
        //BoxCollider2D hert = hertObject.AddComponent<BoxCollider2D>();
        //hert.isTrigger=true;
        //hert.offset = new Vector2(5, 2.5f);
        //hert.size = new Vector2(7,1.2f);

    }
    public void BasicAttackAir()
    {
        IEBasicAttack = StartCoroutine(_attackList[0].UseMove(hertObject,hitObject));
    }
    public void SpaicleAttack()
    {
        IEBasicAttack = StartCoroutine(_attackList[0].UseMove(hertObject, hitObject));
    }
    public void SpaicleAttackAir() 
    {
        IEBasicAttack = StartCoroutine(_attackList[0].UseMove(hertObject, hitObject));
    }
    
}
