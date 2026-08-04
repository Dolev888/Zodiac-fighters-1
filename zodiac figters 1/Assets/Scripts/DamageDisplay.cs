using UnityEngine;
using TMPro;

public class DamageDisplay : MonoBehaviour
{
    [SerializeField] private FighterDamage fighterDamage;
    [SerializeField] private TMP_Text damageText;

    private void Update()
    {
        if (fighterDamage == null || damageText == null) return;
        damageText.text = Mathf.RoundToInt(fighterDamage.DamagePercentage) + "%";
    }
}
