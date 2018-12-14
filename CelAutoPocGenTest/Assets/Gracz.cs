using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gracz : MonoBehaviour
{
     Rigidbody rb;
     Vector3 predkosc;
     const int WSPOLCZYNNIK_PREDKOSCI = 7;

	void Start ()
	{
	    rb = GetComponent<Rigidbody> ();
        
	}
	

	void Update () {
	    predkosc = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * WSPOLCZYNNIK_PREDKOSCI;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + predkosc * Time.fixedDeltaTime);
    }
}
