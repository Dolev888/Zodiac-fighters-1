
using UnityEngine;

public class universalAnimationControler : MonoBehaviour
{
    [SerializeField] private GameObject[] _ObList;
    [SerializeField] private GameObject[] _SummonList;
    [SerializeField] private Vector2[] _spawnList;

    public void summonOb(int Id)
    {
        Vector2 locetion = _spawnList[Id];
        GameObject Ob= _SummonList[Id];
        Instantiate(Ob,locetion,Quaternion.identity);
    }

    public void summonChiledOb(int Id)
    {
        Vector2 locetion = _spawnList[Id];
        GameObject Ob = _SummonList[Id];

        Ob = Instantiate(Ob, transform);
        Ob.transform.localPosition = locetion;
    }
    public void DestroyChiled()
    {

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
