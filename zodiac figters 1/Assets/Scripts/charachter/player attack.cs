using System.Collections;
using UnityEngine;

public class playerattack : MonoBehaviour
{
    private int attackB;
    private int attackBA;
    private int attackS;
    private int attackSA;

    private Coroutine IEBasicAttack;

    [SerializeField] AttackPearent[] _attackList;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    public void BasicAttack()
    {
        IEBasicAttack = StartCoroutine(_attackList[0].UseMove());
    }
    public void BasicAttackAir()
    {
        IEBasicAttack = StartCoroutine(_attackList[1].UseMove());
    }
    public void SpaicleAttack()
    {
        IEBasicAttack = StartCoroutine(_attackList[2].UseMove());
    }
    public void SpaicleAttackAir() 
    {
        IEBasicAttack = StartCoroutine(_attackList[3].UseMove());
    }
    
}
