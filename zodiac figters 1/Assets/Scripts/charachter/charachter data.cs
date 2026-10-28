using UnityEngine;

[CreateAssetMenu(fileName = "charachterdata", menuName = "Scriptable Objects/charachterdata")]
public class charachterdata : ScriptableObject
{
    [Header("player main")]
    [SerializeField] Animator _animator;
    [SerializeField] GameObject[] _hitBoxlist;
    [SerializeField] GameObject _pushColideOb;
    [SerializeField] private playermain.TYPE type;
    [Header("player attack")]
    [SerializeField] AttackPearent[] _attackList;
    [SerializeField] AttackPearent _dash;
}
