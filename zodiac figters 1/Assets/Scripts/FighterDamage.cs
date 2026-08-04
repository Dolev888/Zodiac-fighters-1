using UnityEngine;

public class FighterDamage : MonoBehaviour
{
    [SerializeField] private float damagePercentage = 0f;
    [SerializeField] private float maxDamage = 100f;

    public float DamagePercentage => damagePercentage;
    public float MaxDamage => maxDamage;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10f);
        }
    }
    public void TakeDamage (float amount)
    {
        if (amount <= 0f) return;
        damagePercentage += amount;

       if (damagePercentage >= maxDamage)
        {
            damagePercentage = maxDamage; //insurinng damage can't be higher then maxdamge;
            Lose(); // if reached max damge you lose
        }
        Debug.Log($"{gameObject.name} Damage: {damagePercentage} %");
    }

    private void Lose()
    {
        Debug.Log($"{gameObject.name} has lost ") ;
    }

   
}
