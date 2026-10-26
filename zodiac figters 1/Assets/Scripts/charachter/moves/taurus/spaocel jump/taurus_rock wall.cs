using UnityEngine;

public class taurus_rockwall : projectileParent
{
    private bool isWall;
    [SerializeField] private float _axeSpeed;
    [SerializeField] private GameObject _axeOb;
    [Header("wall stats")]
    [SerializeField] private float _wallHealth;
    [SerializeField] private Vector3 _wallOffset;
    Rigidbody2D rb ;
    [SerializeField] private GameObject _wallOb;
    [SerializeField] private Collider2D _trigerColider;
    [SerializeField] private Collider2D _ignoredColider;
    private bool isleft;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_pmain != null)
        {
            isleft = _pmain.isleft;
            Collider2D[] ignorlist = _pmain.gameObject.GetComponentsInChildren<Collider2D>();
            foreach (Collider2D ignoring in ignorlist)
            {
                Physics2D.IgnoreCollision(_ignoredColider, ignoring);
            }
           
        }
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity= new Vector2(0,_axeSpeed * (-1));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isWall && collision.gameObject.CompareTag("ground"))    
        {
            isWall=true;
            //Instantiate(_wallOb,transform.position+_wallOffset,Quaternion.identity);
          
            if (isleft)
            {
                _wallOb.transform.position -= _wallOffset ;
            }
            else
            {
                _wallOb.transform.position += _wallOffset ;
            }
            _wallOb.SetActive(true);
            Destroy(_axeOb);
            rb.bodyType = RigidbodyType2D.Static;
            rb.linearVelocity = Vector3.zero;
        }
    }
}
