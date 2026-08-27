using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Attacks/Attack Pearent")]
public abstract class AttackPearent : ScriptableObject
{
    [SerializeField] public float _cooldown;
    public abstract IEnumerator UseMove(playerattack Playerattack, int ID);
    private static int nexstAttackId=0;
    public int GeneratAttackId()
    {
        return ++nexstAttackId;
    }
    public abstract void anoncehit(Collider2D collision);
    


}
