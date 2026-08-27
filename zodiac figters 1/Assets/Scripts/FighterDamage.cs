using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class FighterDamage : MonoBehaviour
{
    [SerializeField] private float damagePercentage = 0f;
    [SerializeField] private float maxDamage = 100f;

    [SerializeField] private MatchManager matchManager;

    [SerializeField] private bool isPlayer;

    public float DamagePercentage => damagePercentage;
    public float MaxDamage => maxDamage;
    private HashSet<int> takenId = new HashSet<int>();

    private void Update()
    {
        if (Time.timeScale == 0f) return;
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10f,888,4);
        }
    }
    public void TakeDamage (float amount, int id,float cooldown)
    {
        
        if (CheckId(id)) return;
        if (cooldown != 0) 
        {
            StartCoroutine(countdown(cooldown,id));
        }
        if (amount <= 0f) return;
        damagePercentage += amount;

       if (damagePercentage >= maxDamage)
        {
            damagePercentage = maxDamage; //insurinng damage can't be higher then maxdamge;
            Lose(); // if reached max damge you lose
        }
        Debug.Log($"{gameObject.name} Damage: {damagePercentage} %");
    }
    private IEnumerator countdown(float time,int id)
    {
        takenId.Add(id);
        yield return new WaitForSeconds(time);
        takenId.Remove(id);
    }

    private void Lose()
    {
        Debug.Log($"{gameObject.name} has lost ") ;
        if (matchManager == null)
        {
            return;
           
        }
        // if this fighter is the player then player lost and if it is the enemy then player won.
        matchManager.EndMatch(!isPlayer, gameObject);
    }
    private bool CheckId(int id) 
    {
        if (takenId.Contains(id)) return true;
        return false;
    
    }

   
}
