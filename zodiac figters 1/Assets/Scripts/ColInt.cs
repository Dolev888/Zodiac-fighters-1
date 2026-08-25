using UnityEngine;

[System.Serializable]
public struct ColInt
{
    public int id;
    public Collider2D collider;

    public ColInt(int id, Collider2D collider)
    {
        this.id = id;
        this.collider = collider;
    }
}
