using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Collider normalizeCollider;

    public float playerSpeed;
    private float playerAcceleration;
    public float rotationSpeed;
    private Vector3 counterMovement;
    private Vector3 movementDirection;
    public float counterMovementForce;

    private bool onGround;
    public float groundCheckDistance;

    public float jumpForce;
    public float laungeForce;
    public bool isJumping;

    private float horizontalInput;
    private float verticalInput;

    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        GetInput();
        CheckPlayerOnGround();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (onGround)
            {
                LaungePlayer();
            }
        }
    }

    private void FixedUpdate()
    {
        if (onGround)
        {
            MovePlayer();
            RotatePlayer();
        }
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        movementDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        counterMovement = new Vector3(-playerRb.velocity.x * counterMovementForce, 0, -playerRb.velocity.z * counterMovementForce);
    }


    private void LaungePlayer()
    {
        playerRb.AddForce((Vector3.up + movementDirection) * jumpForce, ForceMode.Impulse);
    }

    private void CheckPlayerOnGround()
    {
        // Cast a ray downwards from the player's position
        RaycastHit hit;
        if (Physics.Raycast(groundCheckPoint.position,-groundCheckPoint.up , out hit, groundCheckDistance, groundLayer))
        {
            // Ray hit a ground or floor object
            onGround = true;
        }
        else
        {
            // Ray did not hit anything or hit something other than ground
            onGround = false;
        }
        Debug.DrawRay(groundCheckPoint.position, Vector3.down * groundCheckDistance, Color.red);
    }

    private void RotatePlayer()
    {
        // Rotate the player towards the movement direction
        // Rotate the player towards the movement direction
        if (movementDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            playerRb.MoveRotation(Quaternion.RotateTowards(playerRb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime * 100f));
        }

    }

    private void MovePlayer()
    {
        Debug.Log(movementDirection);

        if (movementDirection.magnitude > 0.1f)
        {
            // Apply the movement force to the player
            playerRb.AddForce(movementDirection * playerSpeed + counterMovement);
        }
    }
}
