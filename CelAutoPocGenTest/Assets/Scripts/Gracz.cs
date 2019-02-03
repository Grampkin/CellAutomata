using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Sterowanie))]
[RequireComponent(typeof(KijSterownik))]
public class Gracz : MonoBehaviour
{
    public float predkosc = 5;

    Camera myCamera;
    Sterowanie sterownik;
    KijSterownik spellCaster;

    void Start ()
    {
        sterownik = GetComponent<Sterowanie>();
        spellCaster = GetComponent<KijSterownik>();
        myCamera = Camera.main;
    }
	

	void Update ()
	{
	    Vector3 poruszanie = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"));
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

        if (Input.GetMouseButton(0))
        {
            spellCaster.Cast();
        }
	}

    
}
