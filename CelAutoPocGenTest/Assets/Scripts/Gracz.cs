using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Sterowanie))]
public class Gracz : MonoBehaviour
{
    public float predkosc = 5;

    Camera myCamera;
    Sterowanie sterownik;

    void Start ()
    {
        sterownik = GetComponent<Sterowanie>();
        myCamera = Camera.main;
    }
	

	void Update ()
	{
	    Vector3 poruszanie = new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical"));
	    Vector3 predkoscPorusznia = poruszanie.normalized * predkosc;
	    sterownik.Poruszaj(predkoscPorusznia);

        Ray promien = myCamera.ScreenPointToRay(Input.mousePosition);
        Plane ziemia = new Plane(Vector3.up, transform.position);
	    float odleglosc;

	    if (ziemia.Raycast(promien, out odleglosc))
	    {
	        Vector3 point = promien.GetPoint(odleglosc);
	        sterownik.Patrz(point);
	    }
	}

    
}
