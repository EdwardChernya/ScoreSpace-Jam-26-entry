using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestSlap : MonoBehaviour
{

    private bool isLeftMousePressed;
    public float slapSpeedForward = 50f;
    public float slapSpeedUp = 0.2f;
    private Rigidbody colRidgid;
    private List<Collider> coliders = new List<Collider>();


    private void OnTriggerEnter(Collider other)
    {
        if (!coliders.Contains(other) && other.GetComponent<Rigidbody>() != null) {coliders.Add(other);Debug.Log(other);}
        
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (isLeftMousePressed)
        {
            foreach (Collider col in coliders)
            {

                colRidgid = col.GetComponent<Rigidbody>();
                
                colRidgid.velocity = (transform.forward + (transform.up * slapSpeedUp)) * slapSpeedForward;

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        coliders.Remove(other);
    }


    private void Update()
    {
        isLeftMousePressed = Input.GetMouseButtonDown(0);
    }
}
