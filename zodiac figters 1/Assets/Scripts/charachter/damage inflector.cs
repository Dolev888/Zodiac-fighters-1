using UnityEngine;

public class damageinflector : MonoBehaviour
{

    [SerializeField]private float _damage;
    [SerializeField] private float _stanTime;
    [SerializeField] private float _knokBack;
    [SerializeField] private LayerMask hitlayer;
    [SerializeField] private Color _color;
    [SerializeField] private bool _hitFlag;
    private playermain pmain;
    private playerattack pattack;    
    
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
        
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_hitFlag && pmain != null)
        {
            pattack.AttackHitDetected(collision, _moveID);
        }
        if (collision.contactCaptureLayers != hitlayer) return;
        collision.GetComponent<FighterDamage>().TakeDamage(_damage);
        if(_stanTime != 0)
        {
            Stan();
        }
        if (_knokBack != 0)
        {
            Knocback();
        }
        
    }
    private void Stan()
    {

    }
    private void Knocback()
    {

    }
    private void OnDrawGizmos()
    {
        PolygonCollider2D polygon = GetComponent<PolygonCollider2D>();

        if (polygon == null)
            return;
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
}
