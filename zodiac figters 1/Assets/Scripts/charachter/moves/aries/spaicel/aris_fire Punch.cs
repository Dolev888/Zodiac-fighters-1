
using UnityEngine;

using GameAudio;
public class aris_firePunch : projectileParent
{
    [SerializeField]  private float _existensTime;
    [SerializeField] private float _existensSpeed;

    [Header("regular info")]
    //[SerializeField] private float _damage;
    
   
    private Rigidbody2D rb;

    //public SoundEvent firePunch;


    private float _time;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float rotation = transform.eulerAngles.z;

        if (_pattack._upgradeBoolList[3])
        {
            _existensSpeed *= 1.5f;
        }

        rb = GetComponent<Rigidbody2D>();
        bool isLeft = _pmain.isleft;

        float adj = Mathf.Cos(rotation * Mathf.Deg2Rad) * _existensSpeed;
        float ops = Mathf.Sin(rotation * Mathf.Deg2Rad) * _existensSpeed;

        if (isLeft)
        {
            adj = -adj;
        }

        rb.linearVelocity = new Vector2(adj, ops);

        transform.rotation = Quaternion.Euler(0, isLeft ? 180 : 0, rotation);
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
        if (collision.gameObject.layer == LayerMask.NameToLayer("obstecal"))
        {
            if (collision.transform.parent != null)
                collision.transform.parent.gameObject.SendMessage("OnRaycastHit", gameObject, SendMessageOptions.DontRequireReceiver);
            else
            {
                collision.gameObject.SendMessage("OnRaycastHit", gameObject, SendMessageOptions.DontRequireReceiver);
            }
            obstacleDamage ObD = collision.gameObject.GetComponentInParent<obstacleDamage>();
            if (ObD != null)
            {
                ObD.takeDamage(_damage, _moveID, _coolDown);
            }
            else
            {
                ObD = collision.gameObject.GetComponent<obstacleDamage>();
                if (ObD != null)
                {
                    ObD.takeDamage(_damage, _moveID, _coolDown);
                }
            }
            Destroy(gameObject);
        }
        if (collision.tag !=playerTag && collision.gameObject.layer == LayerMask.NameToLayer("hit") && collision.GetComponentInParent<FighterDamage>() !=null)
        {
            collision.GetComponentInParent<FighterDamage>().TakeDamage(_damage, _moveID, _coolDown);
            Destroy(gameObject);

        }
    }


}
