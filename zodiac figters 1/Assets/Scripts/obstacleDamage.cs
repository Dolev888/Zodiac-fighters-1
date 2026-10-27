using UnityEngine;

public class obstacleDamage : MonoBehaviour
{
    [SerializeField] private GameObject ob;
    [SerializeField] private float _health;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (ob == null) ob= GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void takeDamage(float damage)
    {
        _health-=damage;
    }
}
