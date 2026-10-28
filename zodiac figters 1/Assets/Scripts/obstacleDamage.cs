using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacleDamage : MonoBehaviour
{
    [SerializeField] private GameObject ob;
    [SerializeField] public float _health;
    private HashSet<int> takenId = new HashSet<int>();
    [SerializeField] private bool _overideDeath;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (ob == null) ob= GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!_overideDeath && _health <= 0)
        {
            Destroy(ob);
        }
    }
    public void takeDamage(float damage,int id, float cooldown)
    {
        if (CheckId(id)) return;
        if (cooldown != 0)
        {
            StartCoroutine(countdown(cooldown, id));
        }
        if (damage <= 0f) return;
        _health -=damage;
    }
    private bool CheckId(int id)
    {
        if (takenId.Contains(id)) return true;
        return false;

    }
    private IEnumerator countdown(float time, int id)
    {
        takenId.Add(id);
        yield return new WaitForSeconds(time);
        takenId.Remove(id);
    }
}
