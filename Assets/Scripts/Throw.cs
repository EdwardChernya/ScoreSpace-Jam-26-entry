using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Throw : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] public Transform holdPoint;
    [SerializeField] public Material highlightMaterial;
    [SerializeField] public Material whiteMaterial;


    public float sphereCastRadius;
    public float sphereCastDistance;

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
            throwable.GetComponent<Rigidbody>().AddForce(player.transform.forward * 550f);
            throwable.GetComponent<Rigidbody>().AddForce(Vector3.up * 70f);
        }
    }

    private void PickUpObject()
    {
        if (objectIsInRange)
        {
            objectInRange.GetComponent<Rigidbody>().isKinematic = true;
            currentHeldCube = objectInRange;
            currentHeldCube.GetComponent<Transform>().position = holdPoint.position;

            currentHeldCube.transform.SetParent(holdPoint.transform);
            currentHeldCube.gameObject.tag = "Throw";

            objectInRange = null;
            objectIsInRange = false;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Pickup")
        {
            Renderer renderer = other.GetComponent<Renderer>();
            renderer.sharedMaterial = highlightMaterial;
            Debug.Log("PICKUP");

            this.objectInRange = other.gameObject;
            this.objectIsInRange = true;
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Pickup")
        {
            Renderer renderer = other.GetComponent<Renderer>();
            renderer.sharedMaterial = whiteMaterial;
            objectIsInRange = false;
            objectInRange = null;
        }
    }

}
