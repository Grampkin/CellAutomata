using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHP;
    protected float hp;
    protected bool dead;

    protected virtual void Start()
    {
        hp = startingHP;
    }

    public void TakeHit(float damage, RaycastHit hit)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
    }

    protected void Die()
    {
        dead = true;
        GameObject.Destroy(gameObject);
    }

}
