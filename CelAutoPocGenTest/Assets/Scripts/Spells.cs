using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spells : MonoBehaviour
{
    public LayerMask collisionMask;
    float speed = 10;
    private float damage = 1;

    public void SetSpeed(float spd)
    {
        speed = spd;
    }

    // Update is called once per frame
    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
    }

    void CheckCollisions(float moveDist)
    {
        Ray ray = new Ray(transform.position,transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDist, collisionMask,QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);

        }

    }

    void OnHitObject(RaycastHit hit)
    {
        IDamageable damageableObj = hit.collider.GetComponent<IDamageable>();
        if (damageableObj!=null)
        {
            damageableObj.TakeHit(damage, hit);
        }
        GameObject.Destroy(gameObject);
    }
}
