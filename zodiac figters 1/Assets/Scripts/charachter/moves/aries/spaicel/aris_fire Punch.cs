
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
        rb = GetComponent<Rigidbody2D>();
        float direction = _pmain.gameObject.transform.rotation.y;
        if (_pmain.isleft)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            rb.linearVelocityX = _existensSpeed *(-1);
        }
        else  rb.linearVelocityX = _existensSpeed;
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
