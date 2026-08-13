using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Attacks/Attack Pearent")]
public abstract class AttackPearent : ScriptableObject
{
    public abstract IEnumerator UseMove(playerattack Playerattack, int ID);
    private static int nexstAttackId=0;
    public int GeneratAttackId()
    {
        return ++nexstAttackId;
    }

   
}
