
using UnityEngine;

public class damageinflector : MonoBehaviour
{

    [SerializeField]public float _damage; 
    [SerializeField] private float _stnTime;
    [SerializeField] private float _knokBack;
    [SerializeField] private float _knockBackEngel;
    
    [SerializeField] private LayerMask hitlayer;
    [SerializeField] private Color _color;
    [SerializeField] private bool _hitFlag;
    [SerializeField] private float _coolDown;
    private playermain pmain;
    private playerattack pattack;
    [SerializeField] private Vector4[] _hitBoxParmeter;
    [SerializeField] private GameObject[] _hitBoxOb;
    private LayerMask obstacleMask;
    public int _moveID;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hitlayer |= (1 << LayerMask.NameToLayer("obstecal"));
        obstacleMask = (1 << LayerMask.NameToLayer("obstecal"));
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
    private void Stan(Collider2D hit)
    {
        hit.GetComponentInParent<FighterDamage>().TakeStun(_stnTime);
    }
    private void Knocback(Collider2D hit)
    {
        float x = Mathf.Cos(_knockBackEngel * Mathf.Deg2Rad) * _knokBack;
        float y = Mathf.Sin(_knockBackEngel * Mathf.Deg2Rad) * _knokBack;

        if (hit.GetComponentInParent<FighterDamage>().transform.position.x< pmain.transform.position.x)
        {
            x = -x;
        }

        Vector2 knockBackForce = new Vector2(x, y);
        hit.GetComponentInParent<FighterDamage>().TakeKnockback(knockBackForce);
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
                BoxCollider2D box = _hitBoxOb[i].GetComponent<BoxCollider2D>();
                Vector2 point = box.transform.TransformPoint(box.offset);
                Vector2 size = box.bounds.size;
                Collider2D[] hits = Physics2D.OverlapBoxAll(point, size, 0f, hitlayer);

                foreach (Collider2D hit in hits)
                {
                    Vector2 targetPoint = hit.bounds.center;
                    Vector2 start = new Vector2(point.x + (pmain.isleft ? size.x / 2f : -size.x / 2f), point.y);
                    Vector2 direction = targetPoint - start;
                   
                    float distance = direction.magnitude;
                    RaycastHit2D obstacle = Physics2D.Raycast(start, direction.normalized, distance, obstacleMask);
                    if (obstacle.collider != null)
                    {
                        if (hit.gameObject.layer == LayerMask.NameToLayer("obstecal"))
                        {
                            obstacleHit(hit);
                        }
                        continue;
                    }
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
            else if(_hitBoxOb[i].GetComponent<CircleCollider2D>() != null)
            {
                CircleCollider2D circle = _hitBoxOb[i].GetComponent<CircleCollider2D>();
                Vector2 point = circle.transform.TransformPoint(circle.offset);


                float size = circle.radius;
                Collider2D[] hits = Physics2D.OverlapCircleAll(point, size, hitlayer);
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
        }
        
    }
    private void DrawHitBox()
    {
        Gizmos.color = _color;
        for (int i = 0; i < _hitBoxOb.Length; i++)
        {
            if (_hitBoxOb[i].GetComponent<BoxCollider2D>() != null)
            {
                DrawBox(i);
            }
            else if (_hitBoxOb[i].GetComponent<PolygonCollider2D>() != null)
            {
                DrawPoligon(i);
            }
            else if (_hitBoxOb[i].GetComponent<CircleCollider2D>() !=null)
            {
                DrawCircle(i);
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
    private void DrawBox(int obje)
    {
        BoxCollider2D box = _hitBoxOb[obje].GetComponent<BoxCollider2D>();
        Vector2 point = box.transform.TransformPoint(box.offset);


        Vector2 size = box.bounds.size;
        Gizmos.DrawCube(point, size);
    }
    private void DrawCircle(int obje)
    {
        CircleCollider2D circle = _hitBoxOb[obje].GetComponent<CircleCollider2D>();
        Vector2 point = circle.transform.TransformPoint(circle.offset);


        float size = circle.radius;
        Gizmos.DrawWireSphere(point, size);
    }
    private void DamgaeFlag(Collider2D collision)
    {
       
        if (_hitFlag && pattack != null && collision.gameObject.tag != pmain.gameObject.tag)
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
        hit.gameObject.SendMessage("OnRaycastHit", GetComponent<GameObject>(), SendMessageOptions.DontRequireReceiver);
        hit.GetComponentInParent<FighterDamage>().TakeDamage(_damage, _moveID, _coolDown);
        
        if (_stnTime != 0)
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
            ObD.takeDamage(_damage, _moveID, _coolDown);
        }
        else 
        {
            ObD = hit.gameObject.GetComponent<obstacleDamage>();
            if (ObD != null)
            {
                ObD.takeDamage(_damage, _moveID, _coolDown);
            }
        }
        
        DamgaeFlag(hit);
    }
}
