using UnityEngine;

public class projectileParent : MonoBehaviour
{
    public string playerTag;
    
    public playerattack _pattack;
    public playermain _pmain;
    public int _moveID;
    [SerializeField] public float _damage;
    
    [SerializeField] public float _stanTime;
    [SerializeField] public float _knokBack;
    [SerializeField] public float _knockBackEngel;

    [SerializeField] public LayerMask hitlayer;
    [SerializeField] public Color _color;
    [SerializeField] public bool _hitFlag;
    [SerializeField] public float _coolDown;
    
    
    [SerializeField] public Vector4[] _hitBoxParmeter;
    [SerializeField] public GameObject[] _hitBoxOb;
}
