using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Attacks/Attack Pearent")]
public abstract class AttackPearent : ScriptableObject
{
    [SerializeField] public float _cooldown;
    [SerializeField] public float _chainCount;
    public abstract IEnumerator UseMove(playerattack Playerattack, int ID,int ver);
    private static int nexstAttackId=0;
    public int GeneratAttackId()
    {
        return ++nexstAttackId;
    }
    public abstract void anoncehit(Collider2D collision);
    


}
