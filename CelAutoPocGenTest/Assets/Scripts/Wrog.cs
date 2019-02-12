using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Wrog : LivingEntity
{
    private NavMeshAgent wyszukiwaczDrogy;
    private Transform cel;
    public float agroRadius = 10f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        wyszukiwaczDrogy = GetComponent<NavMeshAgent>();
        cel = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(UpdateDroge());
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, agroRadius);
    }

    IEnumerator UpdateDroge()
    {
        
        float odswiezanie = 0.3f;
        while (cel!=null)
        {
            Vector3 celWspolrzedne = new Vector3(cel.position.x,0,cel.position.z);
            float distance = Vector3.Distance(celWspolrzedne, transform.position);
            if (!dead&&distance<=agroRadius)
            {
                wyszukiwaczDrogy.SetDestination(celWspolrzedne);
            }
            yield return new WaitForSeconds(odswiezanie);
        }
    }
}
