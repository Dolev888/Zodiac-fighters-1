using UnityEngine;

public class platform : MonoBehaviour
{
    [SerializeField] private playermain.TYPE type;
    public playermain.TYPE Type => type;
    [Header("player chack")]
    [SerializeField] private Vector2 _size;
    [SerializeField] private Vector3 _offSet;
    [SerializeField] private Color _color;
    [SerializeField] private float _duration;
    
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll( transform.position + _offSet, _size,0f);

        foreach (Collider2D ob in hits)
        {
            playermain player = ob.GetComponentInParent<playermain>();

            if (player != null && player.Type == type)
            {
                player.platformEnhanceSet(_duration);
            }
        }
    }
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Collider2D hit = Physics2D.OverlapBox(transform.position + _offSet, _size, 0f);
    //    if (hit )
    //    {
    //        playermain player = collision.gameObject.GetComponentInParent<playermain>();
    //        if(player != null && player.Type == type)
    //        {
    //            player.platformEnhanceSet(_duration);
    //        }
    //    }
    //}
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _color;
        Gizmos.DrawWireCube(transform.position + _offSet, _size);
    }
}
