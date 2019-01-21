using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GrowndSize : MonoBehaviour
{

    GenerujMape mapa;
    
    // Start is called before the first frame update
    void Start()
    {
        mapa = GetComponent<GenerujMape>();
        transform.localScale = new Vector3((float)mapa.poziom.GetValue(0)/10, 1f, (float)mapa.poziom.GetValue(1)/10);
    }

    
}
