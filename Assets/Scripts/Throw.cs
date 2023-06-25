using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour
{
    [SerializeField] private Transform orientation;
    [SerializeField] public Transform holdPoint;
    [SerializeField] public Material highlightMaterial;
    [SerializeField] public Animator animator;
    [SerializeField] public GameObject player;
    [SerializeField] public GameObject regularCamera;
    [SerializeField] public GameObject aimCamera;

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
    public bool playerCurrentlyAiming;

    private bool ignoreCollisionWithPlayer;

    private int currentObjectCount = 0;

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

        if (currentlyHoldingObject && controller.playerGetUp)
        {
            ThrowObject();
        }

        if (!currentlyHoldingObject)
        {
            if (objectsInRange.Count > 0)
            {
                if (currentObjectCount != objectsInRange.Count)
                {
                    HighlightClosestInRange();
                    currentObjectCount = objectsInRange.Count;
                }
            }
        }

        if (objectsInRange.Count == 0 && currentObjectCount != 0)
        {
            currentObjectCount = 0;
        }

        ChangeHightlight();
        Debug.Log(currentObjectCount);

    }

    private IEnumerator CooldownAfterThrow()
    {
        hasJustThrown = true;
        yield return new WaitForSeconds(1f);
        hasJustThrown = false;
    }


    private void ThrowObject()
    {
        if (controller.playerGetUp && !hasJustThrown)
        {
            if (Input.GetMouseButtonDown(0))
            {
                playerCurrentlyAiming = true;

                regularCamera.SetActive(false);
                aimCamera.SetActive(true);


                aimCamera.GetComponent<CinemachineFreeLook>().m_XAxis = regularCamera.GetComponent<CinemachineFreeLook>().m_XAxis;
                aimCamera.GetComponent<CinemachineFreeLook>().m_YAxis = regularCamera.GetComponent<CinemachineFreeLook>().m_YAxis;


                controller.moveSpeed = 30f;
                controller.groundDrag = 22.5f;
            }
            if (Input.GetMouseButtonUp(0) && playerCurrentlyAiming)
            {
                currentHeldObject.transform.SetParent(null);
                currentHeldObject.GetComponent<Rigidbody>().isKinematic = false;

                for (int i = 0; i < currentHeldObject.transform.GetComponentsInChildren<Collider>().Length; i++)
                {
                    currentHeldObject.transform.GetComponentsInChildren<Collider>()[i].isTrigger = false;
                }

                currentHeldObject.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * throwSpeedForward, ForceMode.Impulse);
                currentHeldObject.GetComponent<Rigidbody>().AddForce(orientation.up * throwSpeedUp, ForceMode.Impulse);

                Vector3 randomTorque = new Vector3(UnityEngine.Random.Range(0, 30f), UnityEngine.Random.Range(0, 30f), UnityEngine.Random.Range(0, 30f));
                currentHeldObject.GetComponent<Rigidbody>().AddTorque(randomTorque);

                currentHeldObject.tag = "PickupCd";

                currentHeldObject.GetComponent<IgnoreCollisionWithPlayer>().ignoreCollisionWithPlayer = true;
                currentHeldObject = null;

                animator.Play("throw2");
                StartCoroutine(CooldownAfterThrow());

                // release here
                playerCurrentlyAiming = false;
                currentlyHoldingObject = false;
                
                controller.moveSpeed = 60f;
                controller.groundDrag = 17.5f;

                aimCamera.SetActive(false);

                regularCamera.GetComponent<CinemachineFreeLook>().m_XAxis = aimCamera.GetComponent<CinemachineFreeLook>().m_XAxis;
                regularCamera.GetComponent<CinemachineFreeLook>().m_YAxis = aimCamera.GetComponent<CinemachineFreeLook>().m_YAxis;

                regularCamera.SetActive(true);

                if (objectsInRange.Count > 0)
                {
                    HighlightClosestInRange();
                }
            }


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

            currentObjectCount--;
        }
    }

    private void DropObject()
    {

    }

    private void ChangeHightlight()
    {
        MeshRenderer objRenderer;
        Material[] objMaterials;

        if (objectsInRange.Count > 0 && Input.GetKeyDown(KeyCode.X))
        {
            // Handle the case where the object is already the closest in range
            if (objectsInRange.Count == 1 && closestObjectInRange == null)
            {
                Debug.Log("last object is the closest in range");
                HighlightClosestInRange();
                return;
            }

            for (int i = 0; i < objectsInRange.Count; i++)
            {
                // lets say we have the objects [A, B, C, D]
                // B is set as the current closest object
                // we start at A and check whether A == B (which it is not)
                // Change closest object = A, to prevent from just switching back and forth between A and B, put B as the last object?

                if (objectsInRange[i] != closestObjectInRange)
                {
                    objRenderer = closestObjectInRange.GetComponent<MeshRenderer>();
                    objMaterials = objRenderer.materials;

                    // Remove the highlight material from the old closest object
                    if (objMaterials.Length > 1)
                    {
                        Array.Resize(ref objMaterials, objMaterials.Length - 1);
                        objRenderer.materials = objMaterials;
                    }

                    closestObjectInRange = objectsInRange[i];
                    GameObject temp = objectsInRange[objectsInRange.Count - 1];
                    objectsInRange[objectsInRange.Count - 1] = objectsInRange[i];
                    objectsInRange[i] = temp;


                    // Highlight the new object
                    objRenderer = closestObjectInRange.gameObject.GetComponent<MeshRenderer>();
                    objMaterials = objRenderer.materials;

                    Array.Resize(ref objMaterials, objMaterials.Length + 1);
                    objMaterials[1] = highlightMaterial;
                    objRenderer.materials = objMaterials;


                }
            }

        }
    }

    private void HighlightClosestInRange()
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

        Debug.Log("huh");

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
            Debug.Log(other.gameObject + " is other");
            Debug.Log(closestObjectInRange + " is closest object in range");

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