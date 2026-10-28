using UnityEngine;

public class taurus_groundwave : projectileParent
{
    [SerializeField] private float _existensTime;
    [SerializeField] private float _existensSpeed;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (_pmain.isleft)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            rb.linearVelocityX = _existensSpeed * (-1);
        }
        else rb.linearVelocityX = _existensSpeed;
    }


    void Update()
    {
        if (_existensTime <= 0)
        {
            Destroy(gameObject);
        }
        _existensTime -= Time.deltaTime;
        hitcheck();
    }
    private void OnDrawGizmos()
    {
        DrawHitBox();

    }
}
