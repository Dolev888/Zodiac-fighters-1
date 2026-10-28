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
    public void DrawHitBox()
    {
        Gizmos.color = _color;
        for (int i = 0; i < _hitBoxOb.Length; i++)
        {
            if (_hitBoxOb[i].GetComponent<BoxCollider2D>() != null)
            {
                BoxCollider2D box = _hitBoxOb[i].GetComponent<BoxCollider2D>();
                Vector2 point = box.transform.TransformPoint(box.offset);


                Vector2 size = box.bounds.size;
                Gizmos.DrawCube(point, size);
            }
            else if (_hitBoxOb[i].GetComponent<PolygonCollider2D>() != null)
            {
                DrawPoligon(i);
            }
        }
    }
    private void DrawPoligon(int obje)
    {

        PolygonCollider2D polygon = _hitBoxOb[obje].GetComponent<PolygonCollider2D>();
        if (polygon == null) return;

        Gizmos.color = _color;
        Gizmos.matrix = _hitBoxOb[obje].transform.localToWorldMatrix;

        for (int p = 0; p < polygon.pathCount; p++)
        {
            Vector2[] path = polygon.GetPath(p);

            for (int i = 0; i < path.Length; i++)
            {
                Vector2 current = path[i] + polygon.offset;
                Vector2 next = path[(i + 1) % path.Length] + polygon.offset;

                Gizmos.DrawLine(current, next);
            }
        }
    }
    public void hitcheck()
    {
        for (int i = 0; i < _hitBoxOb.Length; i++)
        {

            if (_hitBoxOb[i].GetComponent<BoxCollider2D>() != null)
            {
                BoxCollider2D box = _hitBoxOb[i].GetComponent<BoxCollider2D>();
                Vector2 point = box.transform.TransformPoint(box.offset);
                Vector2 size = box.bounds.size;
                Collider2D[] hits = Physics2D.OverlapBoxAll(point, size, 0f, hitlayer);

                foreach (Collider2D hit in hits)
                {
                    Debug.Log(hit.gameObject.name);
                    if (hit.GetComponentInParent<FighterDamage>() != null)
                    {
                        Hit(hit);
                    }
                    else if (hit.gameObject.layer == LayerMask.NameToLayer("obstecal"))
                    {

                        obstacleHit(hit);
                    }

                }
            }
            else if (_hitBoxOb[i].GetComponent<PolygonCollider2D>() != null)
            {
                ContactFilter2D filter = new ContactFilter2D();
                filter.SetLayerMask(hitlayer);
                filter.useLayerMask = true;

                Collider2D[] results = new Collider2D[10];

                int amount = _hitBoxOb[i].GetComponent<PolygonCollider2D>().Overlap(filter, results);

                for (int p = 0; p < amount; p++)
                {
                    Collider2D hit = results[p];


                    if (hit.gameObject.layer == LayerMask.NameToLayer("obstecal"))
                    {
                        obstacleHit(hit);
                    }
                    else
                    {
                        Hit(hit);
                    }
                }
            }
        }

    }
    private void Hit(Collider2D hit)
    {
        DamgaeFlag(hit);
        if (hit.gameObject.tag == _pmain.gameObject.tag || hit.GetComponentInParent<FighterDamage>() == null)
        {
            return;
        }
        hit.gameObject.SendMessage("OnRaycastHit", GetComponent<GameObject>(), SendMessageOptions.DontRequireReceiver);
        hit.GetComponentInParent<FighterDamage>().TakeDamage(_damage, _moveID, _coolDown);

        if (_stanTime != 0)
        {
            Stan(hit);
        }
        if (_knokBack != 0)
        {
            Knocback(hit);
        }
    }
    private void obstacleHit(Collider2D hit)
    {
        if (hit.transform.parent != null)
            hit.transform.parent.gameObject.SendMessage("OnRaycastHit", gameObject, SendMessageOptions.DontRequireReceiver);
        else
        {
            hit.gameObject.SendMessage("OnRaycastHit", gameObject, SendMessageOptions.DontRequireReceiver);
        }
        obstacleDamage ObD = hit.gameObject.GetComponentInParent<obstacleDamage>();
        if (ObD != null)
        {

        }
        else
        {
            ObD = hit.gameObject.GetComponent<obstacleDamage>();
            if (ObD != null)
            {

            }
        }

        DamgaeFlag(hit);
    }
    private void DamgaeFlag(Collider2D collision)
    {

        if (_hitFlag && _pattack != null)
        {
            _pattack.AttackHitDetected(collision, _moveID);
        }
    }
    private void Stan(Collider2D hit)
    {
        hit.GetComponentInParent<FighterDamage>().TakeStun(_stanTime);
    }
    private void Knocback(Collider2D hit)
    {
        float x = Mathf.Cos(_knockBackEngel * Mathf.Deg2Rad) * _knokBack;
        float y = Mathf.Sin(_knockBackEngel * Mathf.Deg2Rad) * _knokBack;

        if (hit.GetComponentInParent<FighterDamage>().transform.position.x < _pmain.transform.position.x)
        {
            x = -x;
        }

        Vector2 knockBackForce = new Vector2(x, y);
        hit.GetComponentInParent<FighterDamage>().TakeKnockback(knockBackForce);
    }
}
