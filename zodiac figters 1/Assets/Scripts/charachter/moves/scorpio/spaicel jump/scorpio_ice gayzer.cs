using System.Collections;
using UnityEngine;


public class scorpio_icegayzer : projectileParent
{
    [Header("regular info")]
    [SerializeField] private float _damage;
    [SerializeField] private float _stanTime;
    [SerializeField] private float _knokBack;
    [SerializeField] private float _knockBackEngel;
    [SerializeField] private LayerMask hitlayer;
    [SerializeField] private Color _color;
    [SerializeField] private bool _hitFlag;
    [SerializeField] private float _coolDown;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector3 _offset;

    [Header("Lazer")]
    [SerializeField] Vector2 _origin;
    [SerializeField] Vector2 _size;
    [SerializeField] float _lazerSpeed;
    [SerializeField] private GameObject _lazer;
    
   
    [Header("Gazer")]
    [SerializeField] float _gayzerSpeed;
    [SerializeField] float _gayzerRetractionSpeed;
    [SerializeField] private int _gayzerCount;
    [SerializeField] private float _gayzerSpred;
    [SerializeField] private Vector2 _gayzerSize;
    [SerializeField] private float _gayzerDeminish;
    [SerializeField] private float _gayzerDelay;
    [SerializeField] private GameObject _gazer;
    [SerializeField] private GameObject _upFoum;
    [SerializeField] private GameObject _downFoum;
    private GameObject Lazer;


    private Vector2 hitPosition;
    private bool lazerOn =true;
    private Vector2 drawSize;
    private Vector2 drawPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_pmain.isleft)
        {
            _offset.x = _offset.x*-1;
        }
       _origin = transform.position + _offset;
       
        if (_pmain.isleft)
        {
            _gayzerSpred = _gayzerSpred * -1;
            
        }
        Lazer = Instantiate(_lazer,_origin, Quaternion.Euler(0,0,-90));
        
        Lazer.transform.localScale = new Vector2(_size.x, 0); 
        Lazer.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        if (lazerOn)
        {
            extend();
        }
        else
        {

        }
    }
    private void extend()
    {
        Vector2 boxCenter = _origin + Vector2.up * (_size.y );
        
        Collider2D hit = Physics2D.OverlapBox( boxCenter, _size, 0f, groundLayer | hitlayer);
        
        Lazer.transform.localScale = new Vector2(drawSize.y/3.6f, _size.x);
        Lazer.transform.position = new Vector2(_origin.x, _origin.y + drawSize.y/2);

        _size.y -= _lazerSpeed * Time.deltaTime;
        drawSize = _size;
        drawPoint = _origin + Vector2.up * _size.y / 2f;
        if (hit !=null && hit.gameObject.CompareTag("ground"))
        {
            GroundHit(hit);
    
            hitPosition = hit.ClosestPoint(_origin);
           
            lazerOn = false;
            Vector2 pointCheck = hitPosition;
            pointCheck.x += _gayzerSpred;
            pointCheck.y -= 0.1f;
           
            Collider2D ground = Physics2D.OverlapPoint(pointCheck, groundLayer);
          
            Destroy(Lazer);
            if (ground != null)
            {
                _gayzerCount--;
                StartCoroutine(Gayzer(_gayzerSize, pointCheck));
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else if(hit != null && hit.gameObject.CompareTag("hit"))
        {
            HitTarget(hit);
        }

    }
    private IEnumerator Gayzer(Vector2 gayzerSize, Vector2 spawnPoint)
    {
        
        Vector2 gayzerCarentSize = gayzerSize;
        gayzerCarentSize.y = 0;
        GameObject downfoum = Instantiate(_downFoum, spawnPoint, Quaternion.identity);
        downfoum.SetActive(true);
        downfoum.transform.localScale = new Vector2(gayzerCarentSize.x, gayzerCarentSize.x);
        GameObject gazer = Instantiate(_gazer, spawnPoint, Quaternion.identity);
        gazer.SetActive(true);
        gazer.transform.localScale = new Vector2(gayzerCarentSize.x,0);
        GameObject upfoum = Instantiate(_upFoum, spawnPoint, Quaternion.identity);
        upfoum.SetActive(true);
        upfoum.transform.localScale = new Vector2(gayzerCarentSize.x, gayzerCarentSize.x);

        yield return new WaitForSeconds(_gayzerDelay);
        while (gayzerCarentSize.y < gayzerSize.y)
        {
           
            Vector2 boxCenter = spawnPoint + Vector2.up * (gayzerCarentSize.y / 2f);

            RaycastHit2D hit = Physics2D.BoxCast( boxCenter, gayzerCarentSize, 0f, Vector2.up,0f, hitlayer);
            if (hit) HitTarget(hit.collider);
            drawSize = gayzerCarentSize;
            drawPoint = boxCenter;
            gazer.transform.localScale = new Vector2(gayzerCarentSize.x, gayzerCarentSize.y / 3.9f);
            gazer.transform.position = new Vector2(spawnPoint.x, spawnPoint.y+gayzerCarentSize.y/2);
            upfoum.transform.position = new Vector2(spawnPoint.x, spawnPoint.y + gayzerCarentSize.y);
            gayzerCarentSize.y += _gayzerSpeed * Time.deltaTime;
            
            yield return null;
        }
        Destroy(downfoum);
        Destroy(upfoum);
        while (gayzerCarentSize.y >0)
        {
            Vector2 boxCenter = spawnPoint + Vector2.up * (gayzerCarentSize.y / 2f);

            RaycastHit2D hit = Physics2D.BoxCast(boxCenter, gayzerCarentSize, 0f, Vector2.up, 0f, hitlayer);
            if (hit) HitTarget(hit.collider);
            drawSize = gayzerCarentSize;
            drawPoint = boxCenter;
            gazer.transform.localScale = new Vector2(gayzerCarentSize.x, gayzerCarentSize.y / 4);
            gazer.transform.position = new Vector2(spawnPoint.x, spawnPoint.y + gayzerCarentSize.y / 2);
            //upfoum.transform.position = new Vector2(spawnPoint.x, spawnPoint.y + gayzerCarentSize.y);
            gayzerCarentSize.y -= _gayzerRetractionSpeed * Time.deltaTime;
            yield return null;
        }
        
        Destroy(gazer);
        if (_gayzerCount > 0)
        {
            
            
            _gayzerCount--;
            gayzerCarentSize.y = gayzerSize.y - _gayzerDeminish;
            spawnPoint.x += _gayzerSpred;
            

            Collider2D ground = Physics2D.OverlapPoint(spawnPoint, groundLayer);
            if (ground)
            {
                StartCoroutine(Gayzer(_gayzerSize, spawnPoint));
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else 
        {
            Destroy(gameObject);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = _color;
        Gizmos.DrawCube(drawPoint, drawSize);
    }
    private void HitDetected()
    {
        GetComponentInParent<FighterDamage>().TakeDamage(_damage, _moveID, _coolDown);

        
    }
    private void GroundHit(Collider2D collision)
    {
        if (_hitFlag && _pattack != null)
        {
            _pattack.AttackHitDetected(collision, _moveID);
        }
    }
    private void HitTarget(Collider2D collision)
    {
        if (collision.tag != playerTag && collision.gameObject.layer == LayerMask.NameToLayer("hit") && collision.GetComponentInParent<FighterDamage>() != null)
        {
            collision.GetComponentInParent<FighterDamage>().TakeDamage(_damage, _moveID, _coolDown);
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
        hit.GetComponentInParent<FighterDamage>().addKnockback(knockBackForce);
    }
}
