using UnityEngine;

public class aris_firePunch : MonoBehaviour
{
    [SerializeField]  private float _existensTime;
    [SerializeField] private float _existensSpeed;

    [Header("regular info")]
    [SerializeField] private float _damage;
    [SerializeField] private float _stanTime;
    [SerializeField] private float _knokBack;
    [SerializeField] private LayerMask hitlayer;
    [SerializeField] private Color _color;
    [SerializeField] private bool _hitFlag;
    [SerializeField] private float _coolDown;
    private Rigidbody2D rb;
    public playermain pmain;
    public playerattack pattack;
    public string playerTag;
    private float _time;
    public int _moveID;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocityX = _existensSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if(_time >= _existensTime)
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
        if(collision.tag !=playerTag && collision.gameObject.layer == LayerMask.NameToLayer("hit"))
        {
            collision.GetComponent<FighterDamage>().TakeDamage(_damage, _moveID, _coolDown);
            Destroy(gameObject);

        }
    }
}
