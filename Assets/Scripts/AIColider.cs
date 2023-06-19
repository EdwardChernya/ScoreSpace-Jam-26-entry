using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIColider : MonoBehaviour
{
    public bool spotted;
    public float passiveRadius = 20f;
    public float activeRadius = 40f;
    private SphereCollider colSphere;

    private void Start()
    {
        colSphere = gameObject.GetComponent<SphereCollider>();
        colSphere.radius = passiveRadius;
    }   

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            spotted = true;
            colSphere.radius = activeRadius;
            Debug.Log("Zelence");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            spotted = false;
            colSphere.radius = passiveRadius;
        }
    }
}
