using System.Collections;
using UnityEngine;

public abstract class AttackPearent : ScriptableObject
{
    public abstract IEnumerator UseMove(GameObject hert, GameObject hit); 

    //public BoxCollider2D GenerateHertBox(GameObject hertObject,Vector2 offSet, Vector2 size)
    //{
    //    BoxCollider2D hert = hertObject.AddComponent<BoxCollider2D>();
    //    hert.isTrigger = true;
    //    hert.offset = offSet;
    //    hert.size = size;
    //    return hert;
    //}
}
