using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RolleycartController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    public float accelerationRate;
    public float decelerationRate;
    public float airMultiplier;
    public float counterMovementForce;

    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform player;
    public float groundCheckDistance;

    [Header("Ground Check")]
    public float playerHeight;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;



    private Vector3 moveDirection;
    private Vector3 counterMovement;
    private Vector3 velocity;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Ground check
        CheckPlayerOnGround();

        MyInput();


        if (!grounded)
        {
            this.rb.drag = 0;
        }
        else
        {
            this.rb.drag = groundDrag;
        }
    }

    private void FixedUpdate()
    {
        MovePlayerOnCart();
    }

    private void CheckPlayerOnGround()
    {
        // Cast a ray downwards from the player's position
        RaycastHit hit;
        if (Physics.Raycast(groundCheckPoint.position, -groundCheckPoint.up, out hit, groundCheckDistance, groundLayer))
        {
            // Ray hit a ground or floor object
            grounded = true;
        }
        else
        {
            // Ray did not hit anything or hit something other than ground
            grounded = false;
        }
        Debug.DrawRay(groundCheckPoint.position, Vector3.down * groundCheckDistance, Color.red);
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }


    private void MovePlayerOnCart()
    {

        // Calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        counterMovement = new Vector3(-rb.velocity.x * counterMovementForce, 0, -rb.velocity.z * counterMovementForce);

        // Apply acceleration
        velocity += moveDirection.normalized * accelerationRate * Time.fixedDeltaTime;

        // Limit velocity to maximum speed
        float maxSpeed = moveSpeed; // Adjust the value to set the maximum speed
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        // Apply movement force
        rb.AddForce(velocity, ForceMode.Acceleration);

        // Deceleration when no input is detected
        if (horizontalInput == 0 && verticalInput == 0)
        {
            velocity -= velocity * decelerationRate * Time.fixedDeltaTime;
        }
    }
}