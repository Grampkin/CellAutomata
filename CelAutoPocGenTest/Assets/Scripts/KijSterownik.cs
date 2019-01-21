using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KijSterownik : MonoBehaviour
{
    public Transform staffHolder;
    public Kij defaultStaff;

    Kij equippedStaff;

    void Start()
    {
        if (defaultStaff != null)
        {
            EquipStaff(defaultStaff);
        }
    }

    public void EquipStaff(Kij staffToEquip)
    {
        if(equippedStaff != null)
        {
            Destroy(equippedStaff.gameObject);
        }
        equippedStaff = Instantiate(staffToEquip, staffHolder.position, staffHolder.rotation) as Kij;
        equippedStaff.transform.parent = staffHolder;
    }

    public void Cast()
    {
        if(equippedStaff != null)
        {
            equippedStaff.Cast(); 
        }
    }
    
}
