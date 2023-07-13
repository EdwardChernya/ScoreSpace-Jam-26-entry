using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour
{
    private Transform orientation;
    private Transform holdPoint;
    public Material highlightMaterial;
    private Animator animator;
    private GameObject player;
    private GameObject regularCamera;
    private GameObject aimCamera;
    private GameObject penguinObject;

    private Transform cameras;

    private PlayerMovement controller;

    public bool hasJustThrown;

    private float sphereCastRadius = 2f;
    private float sphereCastDistance = 3f;
    private float throwSpeedForward = 15f;
    private float throwSpeedUp = 1.5f;

    public GameObject currentHeldObject;
    private bool objectIsInRange = false;
    public bool currentlyHoldingObject = false;


    public List<GameObject> objectsInRange = new List<GameObject>();
    public GameObject closestObjectInRange;
    public bool playerCurrentlyAiming;

    private bool ignoreCollisionWithPlayer;

    private bool cameraHasToSwitch = false;
    private int currentObjectCount = 0;
    private bool isCurrentlyThrowing;
    private float cooldownThrowObjectAfterAnimation = 0.35f;

    private void Awake()
    {
        this.player = GameObject.Find("Player");
        this.penguinObject = this.player.transform.Find("Penguin").gameObject;
        this.orientation = penguinObject.transform;
        this.holdPoint = penguinObject.transform.Find("mixamorig:Hips").transform.Find("mixamorig:Spine")
            .GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0);

        Debug.Log(holdPoint);

        this.animator = penguinObject.transform.GetComponent<Animator>();
        this.controller = player.GetComponent<PlayerMovement>();
    }

    void Start()
    {
        this.cameras = GameObject.Find("Cameras").transform;
        Debug.Log(cameras);
        this.regularCamera = cameras.transform.Find("PlayerCam").gameObject;
        this.aimCamera = cameras.transform.Find("AimCamera").gameObject;
    }

    void Update()
    {
        if (objectsInRange.Count > 0)
        {
            this.objectIsInRange = true;
        }

        if (!controller.playerPlayingMinigame)
        {
            if (Input.GetKeyDown(KeyCode.E) && objectsInRange.Count > 0 && closestObjectInRange && !currentlyHoldingObject)
            {
                PickUpObject();
            }

            if (currentlyHoldingObject && controller.playerGetUp)
            {
                ThrowObject();
                DropObject();
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
        }

        if (objectsInRange.Count == 0 && currentObjectCount != 0)
        {
            currentObjectCount = 0;
        }

        ChangeHightlight();

    }

    private IEnumerator CooldownAfterThrow()
    {
        hasJustThrown = true;
        yield return new WaitForSeconds(1f);
        hasJustThrown = false;
    }

    private IEnumerator CameraHasToSwitchCd()
    {
        yield return new WaitForSeconds(0.25f);
        
        if (Input.GetMouseButton(0))
        {
            cameraHasToSwitch = true;
        }

    }
    private IEnumerator ThrowObjectAfterAnimationCd()
    {
        yield return new WaitForSeconds(cooldownThrowObjectAfterAnimation);

        if (currentHeldObject != null)
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



            StartCoroutine(CooldownAfterThrow());

            // release here

            currentlyHoldingObject = false;
            isCurrentlyThrowing = false;

            controller.moveSpeed = 60f;
            controller.groundDrag = 17.5f;



            if (aimCamera.activeSelf)
            {
                aimCamera.SetActive(false);
                regularCamera.GetComponent<CinemachineFreeLook>().m_XAxis = aimCamera.GetComponent<CinemachineFreeLook>().m_XAxis;
                regularCamera.GetComponent<CinemachineFreeLook>().m_YAxis = aimCamera.GetComponent<CinemachineFreeLook>().m_YAxis;
                regularCamera.GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = 4f;
                regularCamera.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = 0.04f;
                regularCamera.SetActive(true);
            }

            if (objectsInRange.Count > 0)
            {
                HighlightClosestInRange();
            }

            playerCurrentlyAiming = false;

        }

    }


    private void ThrowObject()
    {
        if (controller.playerGetUp && !hasJustThrown)
        {
            if (playerCurrentlyAiming && cameraHasToSwitch)
            {
                regularCamera.SetActive(false);
                aimCamera.SetActive(true);

                aimCamera.GetComponent<CinemachineFreeLook>().m_XAxis = regularCamera.GetComponent<CinemachineFreeLook>().m_XAxis;
                aimCamera.GetComponent<CinemachineFreeLook>().m_YAxis = regularCamera.GetComponent<CinemachineFreeLook>().m_YAxis;
                aimCamera.GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = 2f;
                aimCamera.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = 0.02f;

                controller.moveSpeed = 30f;
                controller.groundDrag = 22.5f;

                cameraHasToSwitch = false;
            }
            if (Input.GetMouseButtonDown(0))
            {
                playerCurrentlyAiming = true;
                StartCoroutine(CameraHasToSwitchCd());
            }
            if (Input.GetMouseButtonUp(0) && playerCurrentlyAiming)
            {
                isCurrentlyThrowing = true;
                animator.Play("throw2");
                
                StartCoroutine(ThrowObjectAfterAnimationCd());

            }


        }
    }

    private void PickUpObject()
    {
        if (objectIsInRange && !currentlyHoldingObject && !hasJustThrown && !playerCurrentlyAiming && !isCurrentlyThrowing)
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
        if (currentlyHoldingObject && !playerCurrentlyAiming && Input.GetKeyDown(KeyCode.Q) && !hasJustThrown && !isCurrentlyThrowing)
        {
            currentlyHoldingObject = false;
            for (int i = 0; i < currentHeldObject.transform.GetComponentsInChildren<Collider>().Length; i++)
            {
                currentHeldObject.transform.GetComponentsInChildren<Collider>()[i].isTrigger = false;
            }

            Vector3 randomTorque = new Vector3(UnityEngine.Random.Range(0, 30f), UnityEngine.Random.Range(0, 30f), UnityEngine.Random.Range(0, 30f));
            currentHeldObject.GetComponent<Rigidbody>().AddTorque(randomTorque);

            currentHeldObject.transform.SetParent(null);
            currentHeldObject.GetComponent<Rigidbody>().isKinematic = false;
            currentHeldObject.GetComponent<Rigidbody>().AddForce(-orientation.forward * throwSpeedForward * 0.2f, ForceMode.Impulse);
            currentHeldObject.GetComponent<Rigidbody>().AddForce(orientation.up * throwSpeedUp * 0.5f, ForceMode.Impulse);

            currentHeldObject.GetComponent<IgnoreCollisionWithPlayer>().ignoreCollisionWithPlayer = true;
            currentHeldObject.tag = "PickupCd";
            currentHeldObject = null;

        }
    }

    private void ChangeHightlight()
    {
        MeshRenderer objRenderer;
        Material[] objMaterials;

        if (objectsInRange.Count > 0 && Input.GetKeyDown(KeyCode.X) && !playerCurrentlyAiming && !currentlyHoldingObject)
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

        if (closestObjectInRange != null) // if there is no closest object
        {
            if (closestObjectInRange != objectsInRange[0]) // if the previous closest object != the new closest object
            {
                objRenderer = closestObjectInRange.gameObject.GetComponent<MeshRenderer>();
                objMaterials = objRenderer.materials;

                if (objMaterials.Length > 1)
                {
                    Array.Resize(ref objMaterials, objMaterials.Length - 1);
                    objRenderer.materials = objMaterials;
                    return;
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