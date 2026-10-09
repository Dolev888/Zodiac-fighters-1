using UnityEngine;
using TMPro;

public class DamageDisplay : MonoBehaviour
{
    [SerializeField] private FighterDamage fighterDamage;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private GameObject[] _healthBarsList;
    private int barNumber;

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
        SetBars();
    }
    private void SetBars()
    {
        float divide = lastShown / 100;
        if (true)
        {
            if (divide > 0.95f)
            {
                barNumber = 20;
            }
            else if (divide > 0.90f)
            {
                barNumber = 19;
            }
            else if (divide > 0.85f)
            {
                barNumber = 18;
            }
            else if (divide > 0.80f)
            {
                barNumber = 17;
            }
            else if (divide > 0.75f)
            {
                barNumber = 16;
            }
            else if (divide > 0.70f)
            {
                barNumber = 15;
            }
            else if (divide > 0.65f)
            {
                barNumber = 14;
            }
            else if (divide > 0.60f)
            {
                barNumber = 13;
            }
            else if (divide > 0.55f)
            {
                barNumber = 12;
            }
            else if (divide > 0.50f)
            {
                barNumber = 11;
            }
            else if (divide > 0.45f)
            {
                barNumber = 10;
            }
            else if (divide > 0.40f)
            {
                barNumber = 9;
            }
            else if (divide > 0.35f)
            {
                barNumber = 8;
            }
            else if (divide > 0.30f)
            {
                barNumber = 7;
            }
            else if (divide > 0.25f)
            {
                barNumber = 6;
            }
            else if (divide > 0.20f)
            {
                barNumber = 5;
            }
            else if (divide > 0.15f)
            {
                barNumber = 4;
            }
            else if (divide > 0.10f)
            {
                barNumber = 3;
            }
            else if (divide > 0.05f)
            {
                barNumber = 2;
            }
            else if (divide > 0f)
            {
                barNumber = 1;
            }
            else
            {
                barNumber = 0;
            }
        }
        for (int i = 0; i < _healthBarsList.Length  ; i++)
        {
            if (i < barNumber)
            {
                _healthBarsList[i].SetActive(true);
            }
            else
            {
                _healthBarsList[i].SetActive(false);
            }
        }
       
    }
}