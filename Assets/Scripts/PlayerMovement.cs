using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    public float acceleration;
    public float deceleration;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;
    [SerializeField] private float counterMovementForce;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform player;
    public float groundCheckDistance;
    public float countdownGetUp;
    public bool playerGetUp = true;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    public float laungeForce;
    public bool isJumping;

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (grounded && playerGetUp)
            {
                LaungePlayer();
            }
        }

        if (playerGetUp)
        {
            this.rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }

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
        if (grounded && playerGetUp)
        {
            MovePlayer();
        }
    }

    private IEnumerator StartCountdown()
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(countdownGetUp);

        // Countdown completed, perform any desired actions
        Debug.Log("Countdown finished!");

        this.playerGetUp = true;
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

    private void LaungePlayer()
    {
        this.rb.constraints = RigidbodyConstraints.None;

        rb.AddForce(orientation.up * jumpForce, ForceMode.Impulse);
        rb.AddForce(moveDirection * laungeForce, ForceMode.Impulse);

        this.playerGetUp = false;
        StartCoroutine(StartCountdown());
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        // Calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        counterMovement = new Vector3(-rb.velocity.x * counterMovementForce, 0, -rb.velocity.z * counterMovementForce);

        // On ground
        if (grounded)
        {
            // Apply acceleration
            float currentSpeed = rb.velocity.magnitude;
            float maxSpeed = moveSpeed; // Adjust the value to set the maximum speed

            // Determine the target speed based on the input and maximum speed
            float targetSpeed = Mathf.Clamp01(Mathf.Max(Mathf.Abs(horizontalInput), Mathf.Abs(verticalInput)));
            targetSpeed *= maxSpeed;

            // Calculate the speed difference and acceleration
            float speedDiff = targetSpeed - currentSpeed;
            float accelerationRate = speedDiff * acceleration;

            // Apply acceleration and clamp to the maximum speed
            velocity += moveDirection.normalized * accelerationRate * Time.fixedDeltaTime;
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

            // Apply movement force
            rb.AddForce(velocity, ForceMode.Acceleration);

            // Deceleration when no input is detected
            if (horizontalInput == 0 && verticalInput == 0)
            {
                velocity -= velocity * deceleration * Time.fixedDeltaTime;
            }
        }
        // In air
        else
        {
            // Apply air control
            float airAcceleration = moveSpeed / 0.2f; // Adjust the division factor to control the air acceleration rate
            velocity += moveDirection.normalized * airAcceleration * airMultiplier * Time.fixedDeltaTime;

            // Limit air velocity to maximum speed
            float maxAirSpeed = moveSpeed * airMultiplier; // Adjust the value to set the maximum air speed
            velocity = Vector3.ClampMagnitude(velocity, maxAirSpeed);

            // Apply movement force
            rb.AddForce(velocity, ForceMode.Acceleration);
        }
    }
}