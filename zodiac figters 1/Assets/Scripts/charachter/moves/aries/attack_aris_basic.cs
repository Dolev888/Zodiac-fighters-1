using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Attacks/aris/aris_basic")]

public class attack_aris_basic : AttackPearent
{
    public override IEnumerator UseMove() 
    {
        yield return new WaitForSeconds(4f);
    }
}
