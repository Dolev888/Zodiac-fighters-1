
using UnityEngine;

public class taurus_rockwall : projectileParent
{
    [SerializeField] obstacleDamage _obDamage;
    private bool isWall;
    [SerializeField] private float _axeSpeed;
    [SerializeField] private GameObject _axeOb;
    [Header("wall stats")]
    [SerializeField] private float _wallHealth;
    [SerializeField] private Vector3 _wallOffset;
    Rigidbody2D rb;
    [SerializeField] private GameObject _wallOb;
    [SerializeField] private Collider2D _trigerColider;
    [SerializeField] private Collider2D[] _ignoredColider;
    private bool isleft;
    Collider2D[] wallColliders;
    Collider2D[] playerColliders;
    [Header("explosion things")]
    [SerializeField] private float _exploRadius;
    [SerializeField] private Vector3 _exploOffset;
    [SerializeField] private float _exlpoTime;
    [SerializeField] private float _expoDamageCooldown;
    [SerializeField] private GameObject _explosionOb;
    private bool isexplo = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //if (_pmain != null)
        //{
        //    isleft = _pmain.isleft;
        //    Collider2D[] ignorlist = _pmain.gameObject.GetComponentsInChildren<Collider2D>();
        //    foreach (Collider2D ignoring in ignorlist)
        //    {
        //        Physics2D.IgnoreCollision(_ignoredColider, ignoring);
        //    }

        //}
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(0, _axeSpeed * (-1));

        if (_pmain != null)
        {
            isleft = _pmain.isleft;
        }
            

        wallColliders = _ignoredColider;
        playerColliders = _pmain.gameObject.GetComponentsInChildren<Collider2D>();
        distroyThemAll();

    }

    // Update is called once per frame
    void Update()
    {
        if (isWall && !isexplo)
        {
            foreach (Collider2D wallCol in wallColliders)
            {
                foreach (Collider2D playerCol in playerColliders)
                {
                    if (playerCol.gameObject.layer == LayerMask.NameToLayer("hit") || playerCol.gameObject.layer == LayerMask.NameToLayer("push"))
                        Physics2D.IgnoreCollision(wallCol, playerCol, true);
                }
            }
        }
        if (isWall && !isexplo && _obDamage._health <= 0)
        {
            Destroy(gameObject);
        }
        timekiper();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isWall && collision.gameObject.CompareTag("ground"))
        {
            isWall = true;
            //Instantiate(_wallOb,transform.position+_wallOffset,Quaternion.identity);

            if (isleft)
            {
                _wallOffset = _wallOffset * (-1);
            }

            _wallOb.transform.position += _wallOffset;
            _wallOb.SetActive(true);
            Destroy(_axeOb);
            rb.bodyType = RigidbodyType2D.Static;
            rb.linearVelocity = Vector3.zero;
        }
        //else if (!isWall && collision.gameObject.layer == LayerMask.NameToLayer("hert") && collision.gameObject.GetComponentInParent<playermain>() == _pmain)
        //{
        //    explode();
        //}
    }
    public void explode()
    {
        Debug.Log("boom");
        _explosionOb.transform.position = _wallOb.transform.position;
        Destroy(_wallOb);
        _explosionOb.SetActive(true);
        isexplo = true;

    }
    private void Stan(Collider2D hit)
    {
        hit.GetComponentInParent<FighterDamage>().TakeStun(_stanTime);
    }
    private void Knocback(Collider2D hit)
    {
        float x = Mathf.Cos(_knockBackEngel * Mathf.Deg2Rad) * _knokBack;
        float y = Mathf.Sin(_knockBackEngel * Mathf.Deg2Rad) * _knokBack;

        if (hit.GetComponentInParent<FighterDamage>().transform.position.x < _explosionOb.transform.position.x)
        {
            x = -x;
        }

        Vector2 knockBackForce = new Vector2(x, y);
        hit.GetComponentInParent<FighterDamage>().TakeKnockback(knockBackForce);
    }
    private void OnRaycastHit(GameObject hiter)
    {
        playermain hitermain = hiter.GetComponentInParent<playermain>();
        if (hitermain != null && hitermain == _pmain)
        {
            explode();
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = _color;
        Gizmos.DrawWireSphere(_explosionOb.transform.position + _exploOffset, _exploRadius);

    }
    private void timekiper()
    {
        if (isexplo)
        {
            _exlpoTime -= Time.deltaTime;
            Collider2D[] hit = Physics2D.OverlapCircleAll(_explosionOb.transform.position + _exploOffset, _exploRadius);

            if (hit != null)
            {
                foreach (Collider2D collision in hit)
                {
                    if (collision.tag != playerTag && collision.gameObject.layer == LayerMask.NameToLayer("hit") && collision.GetComponentInParent<FighterDamage>() != null)
                    {
                        collision.GetComponentInParent<FighterDamage>().TakeDamage(_damage, _moveID, _expoDamageCooldown);
                        if (_stanTime != 0)
                        {
                            Stan(collision);
                        }
                        if (_knokBack != 0)
                        {
                            Knocback(collision);
                        }

                    }
                }
            }
            if (_exlpoTime <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
    private void distroyThemAll()
    {
        taurus_rockwall[] imposters = FindObjectsByType<taurus_rockwall>(FindObjectsSortMode.None);
        foreach (taurus_rockwall die in imposters) 
        {
            if(die != this && die._pmain ==_pmain)
            {
                Destroy(die.gameObject);
            }
        }
    }
}
