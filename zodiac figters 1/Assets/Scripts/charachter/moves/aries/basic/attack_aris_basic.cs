using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Attacks/aris/aris_basic")]

public class attack_aris_basic : AttackPearent
{
    [SerializeField] private GameObject[] _hertBoxList;
    [SerializeField] private GameObject[] _hitBoxList;
    [SerializeField] private float[] _timePous;
    public override IEnumerator UseMove(GameObject hert, GameObject hit) 
    {
        GameObject carentHitBox = Instantiate(_hitBoxList[0],hit.transform);
        GameObject carentHertBox= Instantiate(_hertBoxList[0],hert.transform);
        yield return new WaitForSeconds(_timePous[0]);
        Destroy(carentHertBox);
        carentHertBox =Instantiate(_hertBoxList[1], hert.transform);
        yield return new WaitForSeconds(_timePous[1]);
        Destroy(carentHertBox);
        Destroy(carentHitBox);
    }
}
