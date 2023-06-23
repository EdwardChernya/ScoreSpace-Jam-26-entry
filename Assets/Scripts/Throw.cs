using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;

public class Throw : MonoBehaviour
{
    [SerializeField] private Transform orientation;
    [SerializeField] public Transform holdPoint;
    [SerializeField] public Material highlightMaterial;
    [SerializeField] public Animator animator;
    [SerializeField] public GameObject player;

    private PlayerMovement controller;

    public bool hasJustThrown;

    public float sphereCastRadius;
    public float sphereCastDistance;
    public float throwSpeedForward = 500f;
    public float throwSpeedUp = 75f;

    private GameObject currentHeldObject;
    private bool objectIsInRange = false;
    private bool currentlyHoldingObject = false;


    public List<GameObject> objectsInRange = new List<GameObject>();
    public GameObject closestObjectInRange;



    void Start()
    {
        this.controller = player.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (objectsInRange.Count > 0)
        {
            this.objectIsInRange = true;
        }

        if (Input.GetKeyDown(KeyCode.E) && objectsInRange.Count > 0 && closestObjectInRange && !currentlyHoldingObject)
        {
            PickUpObject();
        }

        if (Input.GetMouseButton(0) && currentlyHoldingObject && controller.playerGetUp)
        {
            ThrowObject();
        }

        if (!currentlyHoldingObject)
        {
            if (objectsInRange.Count != 0)
            {
                HighlightClosestInRange();
            }
        }
    }

    private IEnumerator CooldownPickUpSameObject(GameObject gameObject)
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(0.5f);

        if (gameObject)
        {
            gameObject.tag = "Pickup";
        }
    }

    private IEnumerator CooldownAfterThrow()
    {
        hasJustThrown = true;
        yield return new WaitForSeconds(1f);
        hasJustThrown = false;
    }


    private void ThrowObject()
    {
        if (currentlyHoldingObject && controller.playerGetUp && !hasJustThrown)
        {
            currentHeldObject.transform.SetParent(null);
            currentHeldObject.GetComponent<Rigidbody>().isKinematic = false;

            for (int i = 0; i < currentHeldObject.transform.GetComponentsInChildren<Collider>().Length; i++)
            {
                currentHeldObject.transform.GetComponentsInChildren<Collider>()[i].isTrigger = false;
            }

            currentHeldObject.GetComponent<Rigidbody>().AddForce(orientation.transform.forward * throwSpeedForward);
            currentHeldObject.GetComponent<Rigidbody>().AddForce(orientation.up * throwSpeedUp);
            currentHeldObject.tag = "PickupCd";

            StartCoroutine(CooldownPickUpSameObject(currentHeldObject));
            animator.Play("throw2");
            StartCoroutine(CooldownAfterThrow());


            currentlyHoldingObject = false;
            currentHeldObject = null;
        }
    }

    private void PickUpObject()
    {
        if (objectIsInRange && !currentlyHoldingObject && !hasJustThrown)
        {
            currentlyHoldingObject = true;

            closestObjectInRange.GetComponent<Rigidbody>().isKinematic = true;
            currentHeldObject = closestObjectInRange;

            objectsInRange.Remove(closestObjectInRange);



            MeshRenderer objRenderer = currentHeldObject.gameObject.GetComponent<MeshRenderer>();
            Material[] objMaterials = objRenderer.materials;
            Array.Resize(ref objMaterials, objMaterials.Length - 1);
            objRenderer.materials = objMaterials;

            closestObjectInRange = null;

            currentHeldObject.GetComponent<Transform>().position = holdPoint.position;
            currentHeldObject.GetComponent<Rigidbody>().isKinematic = true;

            for (int i = 0; i < currentHeldObject.transform.GetComponentsInChildren<Collider>().Length; i++)
            {
                currentHeldObject.transform.GetComponentsInChildren<Collider>()[i].isTrigger = true;
            }

            currentHeldObject.transform.rotation = Quaternion.identity;
            currentHeldObject.transform.Rotate(xAngle: -90, 0, 0);
            currentHeldObject.transform.SetParent(holdPoint.transform);
        }
    }

    void HighlightClosestInRange()
    {
        Vector3 playerPosition = transform.position;
        MeshRenderer objRenderer;
        Material[] objMaterials;

        // Sort the objects by which one is closest to the player
        objectsInRange.Sort((a, b) =>
            Vector3.Distance(playerPosition, a.transform.position).CompareTo(Vector3.Distance(playerPosition, b.transform.position))
        );

        if (closestObjectInRange != null)
        {
            if (closestObjectInRange != objectsInRange[0])
            {
                objRenderer = closestObjectInRange.gameObject.GetComponent<MeshRenderer>();
                objMaterials = objRenderer.materials;

                if (objMaterials.Length > 1)
                {
                    Array.Resize(ref objMaterials, objMaterials.Length - 1);
                    objRenderer.materials = objMaterials;
                }
            }
            else if (closestObjectInRange == objectsInRange[0])
            {
                return;
            }
        }


        closestObjectInRange = objectsInRange[0];

        objRenderer = closestObjectInRange.gameObject.GetComponent<MeshRenderer>();
        objMaterials = objRenderer.materials;
        Array.Resize(ref objMaterials, objMaterials.Length + 1);
        objMaterials[1] = highlightMaterial;
        objRenderer.materials = objMaterials;
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Pickup")
        {
            if (other.gameObject != currentHeldObject)
            {

                if (objectsInRange.Contains(other.gameObject))
                {
                    return;
                }

                objectsInRange.Add(other.gameObject);
            }
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Pickup")
        {
            objectsInRange.Remove(other.gameObject);

            if (other.gameObject == closestObjectInRange)
            {
                MeshRenderer objRenderer = other.gameObject.GetComponent<MeshRenderer>();
                Material[] objMaterials = objRenderer.materials;

                if (objMaterials.Length > 1)
                {
                    Array.Resize(ref objMaterials, objMaterials.Length - 1);
                    objRenderer.materials = objMaterials;
                }

            }

            if (objectsInRange.Count == 0)
            {
                closestObjectInRange = null;
            }



        }
    }

}