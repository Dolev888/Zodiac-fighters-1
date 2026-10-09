using UnityEngine;
using TMPro;

public class DamageDisplay : MonoBehaviour
{
    [SerializeField] private FighterDamage fighterDamage;
    [SerializeField] private TMP_Text damageText;

    private int lastShown = -1;

    // called by MatchSetup once the fighters exist, so each box shows the right fighter
    public void SetFighter(FighterDamage fighter)
    {
        fighterDamage = fighter;
        lastShown = -1;   // force a refresh
    }

    private void Update()
    {
        if (fighterDamage == null || damageText == null) return;

        int current = Mathf.RoundToInt(fighterDamage.DamagePercentage);
        if (current == lastShown) return;

        lastShown = current;
        damageText.text = current + "%";
    }
}