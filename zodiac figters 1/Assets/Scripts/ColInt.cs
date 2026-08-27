using UnityEngine;

[System.Serializable]
public struct ColInt
{
    public int num;
    public Collider2D collider;

    public ColInt(int id, Collider2D collider)
    {
        this.num = id;
        this.collider = collider;
    }
}
