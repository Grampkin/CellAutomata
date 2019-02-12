using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kij : MonoBehaviour
{
    public Transform sphere;
    public Transform player;
    public float sphereVel = 15;
    public Spells spell;
    public float castingTime = 500;

    float nextCast;

    public void Cast()
    {
        if(Time.time > nextCast)
        {
            nextCast = Time.time + castingTime / 1000;
            if (spell!=null)
            {
                Spells newSpell = Instantiate(spell, sphere.position, player.rotation) as Spells;
                newSpell.SetSpeed(sphereVel);
            }
           
        }
        
    }
    
}
