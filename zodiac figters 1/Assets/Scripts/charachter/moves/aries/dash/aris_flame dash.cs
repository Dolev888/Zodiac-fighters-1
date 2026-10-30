
using UnityEngine;

public class aris_flamedash : projectileParent
{
    
    [SerializeField] private float _existancTime;
    //[SerializeField] private float _damage;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(_pmain != null && _pmain.isleft)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        if (_pmain.pattack._upgradeBoolList[5])
        {
            _existancTime = _existancTime * 2;
            _damage = _damage * 1.5f;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_existancTime <= 0)
        {
            Destroy(gameObject);
        }
        _existancTime -= Time.deltaTime;

        for (int i = 0; i < _hitBoxOb.Length; i++)
        {
            if (_hitBoxOb[i].GetComponent<BoxCollider2D>() != null)
            {
                Vector2 point = new Vector2(_hitBoxOb[i].transform.position.x, _hitBoxOb[i].transform.position.y);

                BoxCollider2D box = _hitBoxOb[i].GetComponent<BoxCollider2D>();
                Vector2 size = box.bounds.size;
                Collider2D[] hits = Physics2D.OverlapBoxAll(point, size, 0f, hitlayer);

                foreach (Collider2D hit in hits)
                {
                    Debug.Log(hit.gameObject.name);
                    if (hit.GetComponentInParent<FighterDamage>() != null)
                    {
                        if (hit.gameObject.tag == _pmain.gameObject.tag || hit.GetComponentInParent<FighterDamage>() == null)
                        {
                            return;
                        }

                        hit.GetComponentInParent<FighterDamage>().TakeDamage(_damage, _moveID, _coolDown);
                    }

                }
            }
        }
    }

}
