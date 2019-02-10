using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RozmiarZiemi : MonoBehaviour
{

    public GameObject generator;
    public GameObject ziemia;
    
    // Start is called before the first frame update
    void Start()
    {
        var genMap = generator.GetComponent<GenerujMape>();
        ziemia.transform.localScale = new Vector3((float)genMap.szerokosc / 10, 1f, (float)genMap.wysokosc / 10);
        GameObject.Instantiate(ziemia, new Vector3(0, -5, 0), new Quaternion(0, 0, 0, 0));
       
    }

    
}
