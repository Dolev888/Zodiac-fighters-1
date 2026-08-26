using UnityEngine;


public class aries_firewave : projectileParent
{
    [SerializeField] private float _existensTime;
    [SerializeField] private float _existensSpeed;

    [Header("regular info")]
    [SerializeField] private float _damage;
    [SerializeField] private float _stanTime;
    [SerializeField] private float _knokBack;
    [SerializeField] private LayerMask hitlayer;
    [SerializeField] private Color _color;
    [SerializeField] private bool _hitFlag;
    [SerializeField] private float _coolDown;
    [SerializeField] private float _rotation;
    private Rigidbody2D rb;
    //public playermain pmain;
    public playerattack pattack;
    public string playerTag;
    private float _time;
    public int _moveID;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        float direction= _pmain.gameObject.transform.rotation.y;
        float adj =Mathf.Cos(_rotation * Mathf.Deg2Rad)* _existensSpeed;
        float ops =Mathf.Sin(_rotation * Mathf.Deg2Rad)* _existensSpeed;
        Debug.Log(direction);
        if (direction ==1 )
        {
            
            adj = -adj;
        }
        
        Vector2 angleSpeed = new Vector2(adj, ops);
        rb.linearVelocity = angleSpeed;
        gameObject.transform.rotation = Quaternion.Euler(0, direction*180, _rotation);
    }

    // Update is called once per frame
    void Update()
    {
        if (_time >= _existensTime)
        {
            Destroy(gameObject);
        }
        _time += Time.deltaTime;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ground"))
        {
            Destroy(gameObject);
        }
        if (collision.tag != playerTag && collision.gameObject.layer == LayerMask.NameToLayer("hit"))
        {
            collision.GetComponent<FighterDamage>().TakeDamage(_damage, _moveID, _coolDown);
            Destroy(gameObject);

        }
    }
    private void OnDrawGizmos()
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
}
