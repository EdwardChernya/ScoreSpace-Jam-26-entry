using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Throw : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] public Transform holdPoint;
    [SerializeField] public Material highlightMaterial;

    public float sphereCastRadius;
    public float sphereCastDistance;
    public float throwSpeedForward = 500f;
    public float throwSpeedUp = 75f;

    private GameObject currentHeldCube;
    private GameObject objectInRange;
    private bool objectIsInRange = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            PickUpObject();
        }

        if (Input.GetMouseButton(0))
        {
            ThrowObject();
        }
    }

    private void ThrowObject()
    {
        if (currentHeldCube != null)
        {
            GameObject throwable = new GameObject();
            throwable = currentHeldCube;

            currentHeldCube = null;

            throwable.transform.SetParent(null);
            throwable.GetComponent<Rigidbody>().isKinematic = false;
            throwable.GetComponent<Rigidbody>().AddForce(player.transform.forward * throwSpeedForward);
            throwable.GetComponent<Rigidbody>().AddForce(Vector3.up * throwSpeedUp);
        }
    }

    private void PickUpObject()
    {
        if (objectIsInRange)
        {
            objectInRange.GetComponent<Rigidbody>().isKinematic = true;
            currentHeldCube = objectInRange;
            currentHeldCube.GetComponent<Transform>().position = holdPoint.position;

            currentHeldCube.transform.rotation = Quaternion.identity;
            currentHeldCube.transform.Rotate(xAngle:-90,0,0);

            currentHeldCube.transform.SetParent(holdPoint.transform);

            objectInRange = null;
            objectIsInRange = false;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Pickup")
        {
            MeshRenderer objRenderer = other.GetComponent<MeshRenderer>();
            //other.GameObject().GetComponent<MeshRenderer>().materials[1] = highlightMaterial;

            Material[] materials = objRenderer.materials;
            Array.Resize(ref materials,materials.Length + 1);
            materials[1] = highlightMaterial;
            objRenderer.materials = materials;
            
            this.objectInRange = other.gameObject;
            this.objectIsInRange = true;
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Pickup")
        {
            objectIsInRange = false;
            objectInRange = null;
            MeshRenderer objRenderer = other.GetComponent<MeshRenderer>();
            Material[] materials = objRenderer.materials;
            Array.Resize(ref materials,materials.Length - 1);
            objRenderer.materials = materials;
        }
    }

}
