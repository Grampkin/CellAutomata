using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraczMinimap : MonoBehaviour
{
     Rigidbody2D rb;
     Vector2 predkosc;
     const int WSPOLCZYNNIK_PREDKOSCI = 7;

	void Start ()
	{
	    rb = GetComponent<Rigidbody2D> ();
        
	}
	

	void Update () {
	    predkosc = new Vector2(Input.GetAxisRaw("Horizontal"),  Input.GetAxisRaw("Vertical")).normalized * WSPOLCZYNNIK_PREDKOSCI;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + predkosc * Time.fixedDeltaTime);
    }
}
