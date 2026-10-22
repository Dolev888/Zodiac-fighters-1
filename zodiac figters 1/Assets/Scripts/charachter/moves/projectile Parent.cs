using UnityEngine;

public class projectileParent : MonoBehaviour
{
    public string playerTag;
    
    public playerattack _pattack;
    public playermain _pmain;
    public int _moveID;
    [SerializeField] public float _damage;
}
