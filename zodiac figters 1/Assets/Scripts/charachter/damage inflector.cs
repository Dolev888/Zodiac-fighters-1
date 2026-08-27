using UnityEngine;

public class damageinflector : MonoBehaviour
{

    [SerializeField]private float _damage; 
    [SerializeField] private float _stanTime;
    [SerializeField] private float _knokBack;
    [SerializeField] private LayerMask hitlayer;
    [SerializeField] private Color _color;
    [SerializeField] private bool _hitFlag;
    [SerializeField] private float _coolDown;
    private playermain pmain;
    private playerattack pattack;
    [SerializeField] private Vector4[] _hitBoxParmeter;
    [SerializeField] private GameObject[] _hitBoxOb;

    public int _moveID;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       pmain = GetComponentInParent<playermain>();
        pattack = GetComponentInParent<playerattack>();
        
    }

    // Update is called once per frame
    void Update()
    {
        hitcheck();
    }
    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    Debug.Log("go");
    //    if (_hitFlag && pmain != null)
    //    {
    //        pattack.AttackHitDetected(collision, _moveID);
    //    }
    //    if (collision.contactCaptureLayers != hitlayer) return;
    //    Debug.Log("punch");
    //    collision.GetComponent<FighterDamage>().TakeDamage(_damage,_moveID, _coolDown);
    //    if(_stanTime != 0)
    //    {
    //        Stan();
    //    }
    //    if (_knokBack != 0)
    //    {
    //        Knocback();
    //    }
        
    //}
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Debug.Log("Enter triger 2D");
    //}
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Debug.Log("Enter2D");
    //}
    private void Stan()
    {

    }
    private void Knocback()
    {

    }
    private void OnDrawGizmos()
    {
        DrawHitBox();
        
    }
    
    private void hitcheck()
    {
        for (int i = 0; i < _hitBoxOb.Length; i++)
        {
            
            if (_hitBoxOb[i].GetComponent<BoxCollider2D>() !=null )
            {
                Vector2 point = new Vector2(_hitBoxOb[i].transform.position.x, _hitBoxOb[i].transform.position.y);

                BoxCollider2D box = _hitBoxOb[i].GetComponent<BoxCollider2D>();
                Vector2 size = box.bounds.size;
                Collider2D[] hits = Physics2D.OverlapBoxAll(point, size, 0f, hitlayer);

                foreach (Collider2D hit in hits)
                {
                    Debug.Log(hit.gameObject.name);
                    if (hit.GetComponent<FighterDamage>() != null)
                    {
                        Hit(hit);
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

                    Hit(hit);
                }
            }
        }
        
    }
    private void DrawHitBox()
    {
        Gizmos.color = Color.yellow;
        for (int i = 0; i < _hitBoxOb.Length; i++)
        {
            if (_hitBoxOb[i].GetComponent<BoxCollider2D>() != null)
            {
                Vector2 point = new Vector2(_hitBoxOb[i].transform.position.x, _hitBoxOb[i].transform.position.y);

                BoxCollider2D box = _hitBoxOb[i].GetComponent<BoxCollider2D>();
                Vector2 size = box.bounds.size;
                Gizmos.DrawCube(point, size);
            }
            else if (_hitBoxOb[i].GetComponent<PolygonCollider2D>() != null)
            {
                DrawPoligon();
            }
        }
    }
    private void DrawPoligon()
    {
        PolygonCollider2D polygon = GetComponent<PolygonCollider2D>();

        if (polygon == null) return;
        Gizmos.color = _color;
        Gizmos.matrix = transform.localToWorldMatrix;

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
    private void DamgaeFlag(Collider2D collision)
    {
       
        if (_hitFlag && pattack != null)
        {
            pattack.AttackHitDetected(collision, _moveID);
        }
    }
    private void Hit(Collider2D hit)
    {
        DamgaeFlag(hit);
        if ( hit.gameObject.tag == pmain.gameObject.tag || hit.GetComponentInParent<FighterDamage>()== null)
        {
            return;
        }
        hit.GetComponentInParent<FighterDamage>().TakeDamage(_damage, _moveID, _coolDown);
        
        if (_stanTime != 0)
        {
            Stan();
        }
        if (_knokBack != 0)
        {
            Knocback();
        }
    }
}
