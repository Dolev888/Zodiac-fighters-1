using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DamageDisplay : MonoBehaviour
{
    [SerializeField] private FighterDamage fighterDamage;
    [SerializeField] private TMP_Text damageText;

    [Header("Stripe bar (optional)")]
    // the image with all the stripes: Image Type = Filled, Fill Method = Horizontal
    [SerializeField] private GameObject[] stripeBar;
    [SerializeField] private GameObject _healthbar;
    [SerializeField] private int stripeCount = 20;        // how many stripes the art has
    [SerializeField] private int percentPerStripe = 5;    // 1 stripe disappears for every 5% damage
    [Header("charachters")]
    [SerializeField] private Sprite[] _charachterBarList;

    private int lastShown = -1;

    // called by MatchSetup once the fighters exist, so each box shows the right fighter
    public void SetFighter(FighterDamage fighter)
    {
        fighterDamage = fighter;
        lastShown = -1;   // force a refresh
        setBar();
    }

    private void Update()
    {
        if (fighterDamage == null || damageText == null) return;

        int current = Mathf.RoundToInt(fighterDamage.DamagePercentage);
        if (current == lastShown) return;

        lastShown = current;

        // the number counts DOWN: 100% at the start, 0% when the fighter loses
        damageText.text = Mathf.Max(0, Mathf.RoundToInt(fighterDamage.MaxDamage - fighterDamage.DamagePercentage)) + "%";

        if (stripeBar != null)
        {

            int stripchek = current / percentPerStripe;
            for (int i = 0; i < stripeBar.Length; i++)
            {
                if (stripchek < stripeCount - i)
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
    private void setBar()
    {
        RawImage image = _healthbar.GetComponent<RawImage>();
        playermain pmain = fighterDamage.pmain;
        if (pmain == null) return;
        switch (pmain.Type)
        {
            case (playermain.TYPE.FIRE):
                image.texture = _charachterBarList[0].texture;
                break;
            case (playermain.TYPE.WATER):
                image.texture = _charachterBarList[1].texture;
                break;
            case (playermain.TYPE.EARTH):
                image.texture = _charachterBarList[2].texture;
                break;
            default:
                image.texture = _charachterBarList[0].texture; break;

        }

    }
}