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
        // nothing to do when the art was not set up (the bar then keeps the art it has)
        if (_healthbar == null || _charachterBarList == null || _charachterBarList.Length == 0 || fighterDamage == null) return;

        // found on the fighter itself: FighterDamage.pmain is only filled in Start(), which has not run yet when MatchSetup calls this
        playermain pmain = fighterDamage.GetComponent<playermain>();
        if (pmain == null) return;

        int index;
        switch (pmain.Type)
        {
            case (playermain.TYPE.FIRE):
                index = 0;
                break;
            case (playermain.TYPE.WATER):
                index = 1;
                break;
            case (playermain.TYPE.EARTH):
                index = 2;
                break;
            default:
                index = 0;
                break;
        }

        if (index >= _charachterBarList.Length || _charachterBarList[index] == null) return;
        Sprite art = _charachterBarList[index];

        // works with a normal Image (keeps the sprite's rect) and with a RawImage
        Image image = _healthbar.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = art;
            return;
        }

        RawImage rawImage = _healthbar.GetComponent<RawImage>();
        if (rawImage != null) rawImage.texture = art.texture;
    }
}