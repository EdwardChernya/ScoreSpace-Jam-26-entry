using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform orientation;
    public Transform aimLookAt;
    public Transform player;
    public Transform playerObj;
    public Transform penguinBodyObject;
    public Rigidbody rb;

    [Header("Trolley")]
    public bool onTrolley = false;
    public Transform trolley;
    public Transform trolleyOrientation;
    public Transform trolleyObject;


    public float rotationSpeed;
    public float rotationSpeedWhileSliding;


    private PlayerMovement controller;
    private Throw throwController;
    public Vector3 aimViewDir;

    public void Start()
    {
        Cursor.lockState= CursorLockMode.Locked;
        Cursor.visible = false;
        controller = player.GetComponent<PlayerMovement>();
        throwController = penguinBodyObject.GetComponent<Throw>();
    }
        
    private void Update()
    {

        if (controller.playerGetUp)
        {
            if (!throwController.playerCurrentlyAiming)
            {
                Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
                orientation.forward = viewDir.normalized;
                float horizontalInput = Input.GetAxis("Horizontal");
                float verticalInput = Input.GetAxis("Vertical");
                Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

                if (inputDir != Vector3.zero)
                {
                    playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
                }
            }
            else
            {

                aimViewDir = aimLookAt.position - new Vector3(transform.position.x, aimLookAt.position.y, transform.position.z);
                orientation.forward = aimViewDir.normalized;
                playerObj.forward = aimViewDir.normalized;
            }



        }
        else if (!controller.playerGetUp)
        {
            Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);

            orientation.forward = viewDir.normalized;

            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (inputDir != Vector3.zero)
            {
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeedWhileSliding);
            }
        }
        


    }
}