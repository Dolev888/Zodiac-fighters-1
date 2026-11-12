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
    [SerializeField] private int stripeCount = 20;        // how many stripes the art has
    [SerializeField] private int percentPerStripe = 5;    // 1 stripe disappears for every 5% damage

    [Header("Character art (optional, MatchSetup fills it in)")]
    [SerializeField] private Image frameImage;   // the bar's frame art (the one with the face on it)
    [SerializeField] private Image faceImage;    // optional: a separate face image, if the face is not part of the frame

    private int lastShown = -1;

    // called by MatchSetup once the fighters exist, so each box shows the right fighter
    public void SetFighter(FighterDamage fighter)
    {
        fighterDamage = fighter;
        lastShown = -1;   // force a refresh
    }

    // called by MatchSetup with the art of the character this box belongs to.
    // a missing sprite or a missing Image is skipped, so the box keeps what it already has
    public void SetArt(Sprite frame, Sprite face)
    {
        if (frame != null && frameImage != null) frameImage.sprite = frame;
        if (face != null && faceImage != null) faceImage.sprite = face;
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
}