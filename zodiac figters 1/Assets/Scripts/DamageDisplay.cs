using TMPro;
using UnityEngine;


public class DamageDisplay : MonoBehaviour
{
    [SerializeField] private FighterDamage fighterDamage;
    [SerializeField] private TMP_Text damageText;

    [Header("Stripe bar (optional)")]
    // the image with all the stripes: Image Type = Filled, Fill Method = Horizontal
    [SerializeField] private GameObject[] stripeBar;
    [SerializeField] private int stripeCount = 20;        // how many stripes the art has
    [SerializeField] private int percentPerStripe = 5;    // 1 stripe disappears for every 5% damage

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

        if (stripeBar != null)
        {

            int stripchek = current / percentPerStripe;
            for (int i = 0; i < stripeBar.Length; i++)
            {
                if (stripchek < stripeCount-i)
                {
                    stripeBar[i].SetActive(true);    
                }
                else
                {
                    stripeBar[i].SetActive(false);
                }
            }
           
                       // always lands on a stripe edge
        }
    }
}