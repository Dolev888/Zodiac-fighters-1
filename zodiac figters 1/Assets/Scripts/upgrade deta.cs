using UnityEngine;

[CreateAssetMenu(fileName = "upgradedeta", menuName = "Scriptable Objects/upgradedeta")]
public class upgradedeta : ScriptableObject
{
    [SerializeField] public string _title;
    [SerializeField] public string _text;
    [SerializeField] public int _effectNum;
    [SerializeField] public playermain.TYPE _type;
}
