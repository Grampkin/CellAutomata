using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopViewCamera : MonoBehaviour
{
    public GameObject Gracz;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - Gracz.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Gracz.transform.position + offset;
    }
}
