using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;

    [Header("Trolley")]
    public bool onTrolley = false;
    public Transform trolley;
    public Transform trolleyOrientation;
    public Transform trolleyObject;


    public float rotationSpeed;
    public float rotationSpeedWhileSliding;


    private PlayerMovement controller;

    public void Start()
    {
        Cursor.lockState= CursorLockMode.Locked;
        Cursor.visible = false;
        controller = player.GetComponent<PlayerMovement>();
    }
        
    private void Update()
    {

        if (controller.playerGetUp)
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