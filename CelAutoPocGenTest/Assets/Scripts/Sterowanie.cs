using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Sterowanie : MonoBehaviour
{
    Vector3 predkosc;
    Rigidbody graczRigidbody;


    void Start()
    {
        graczRigidbody = GetComponent<Rigidbody>();
    }

    public void Poruszaj(Vector3 _predkosc)
    {
        predkosc = _predkosc;
    }

    public void Patrz(Vector3 miejsce)
    {
        transform.LookAt(miejsce);
    }

    public void FixedUpdate()
    {
        graczRigidbody.MovePosition(graczRigidbody.position + predkosc * Time.fixedDeltaTime);
    }
}
