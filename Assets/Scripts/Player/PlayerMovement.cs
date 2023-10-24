using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    int playerNumber;
    [Header("Distances")]
    [SerializeField] float distToGround;
    [SerializeField] float distToWallRun;
    //Ground
    [Header("Ground")]
    [SerializeField] float groundForwardForce;
    [SerializeField] float maxGroundForwardSpeed;
    [SerializeField] float groundStrafeForce;
    [SerializeField] float maxGroundStrafeSpeed;

    //Air
    [Header("Air")]
    [SerializeField] float airForwardForce;
    [SerializeField] float maxAirForwardSpeed;
    [SerializeField] float airStrafeForce;
    [SerializeField] float maxAirStrafeSpeed;
    [Header("DownForce")]
    [SerializeField] float apexYVelocity;
    [SerializeField] float downForceBeforeApex;
    [SerializeField] float downForceAfterApex;
    [SerializeField] float downForceWhenRotating;
    [SerializeField] float maxFallSpeed;
    [SerializeField] float airMultiplier;


    //Jump
    [Header("Jump")]
    [SerializeField] float jumpForce = 30f;
    [SerializeField] float jumpCooldown = 0.5f;
    [SerializeField] float coyoteTimeDuration = 0.2f;
    [SerializeField] float jumpBufferDuration = 0.2f;
    float coyoteTimer;
    float jumpBufferTimer;
    [SerializeField] Text forwardspeedText; 
    [SerializeField] Text strafeText; 
    bool readyToJump = true;

    [Header("WallRunning")]
    [SerializeField] float wallRunForwardForce;
    [SerializeField] float wallRunDownForceSmall;
    [SerializeField] float wallRunDownForceBig;

    bool isRotating;

    //
    PlayerInput playerInput;
    PlayerShooting playerShooting;
    PlayerDuplicateManager duplicateManager;
    LevelRepeater levelRepeater;    
    Rigidbody rb;
    public enum MoveState
    {
        Grounded,
        Flying,
        WallRunning
    }
    public MoveState CurrentMoveState;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerShooting = GetComponent<PlayerShooting>();
        duplicateManager = GetComponent<PlayerDuplicateManager>();
        playerInput = GetComponent<PlayerInput>();
        levelRepeater = FindFirstObjectByType<LevelRepeater>();
    }
    private void Start()
    {
        playerNumber = GetComponent<PlayerLocalManager>().PlayerNumber;
    }
    MoveState previousMoveState;
    private void GetMovementState()
    {
        // If on ground
        if (IsGrounded())
        {
            CurrentMoveState = MoveState.Grounded;
            return; // return so can't wallrun when grounded
        }
        // Not Grounded
        // If Wallruning
        if (IsWallRunningLeft() || IsWallRunningRight())
        {
            //CurrentMoveState = MoveState.WallRunning;
            //return;
        }
        // Not Wallrunning
        CurrentMoveState = MoveState.Flying;
    }
    void FixedUpdate()
    {
        GetMovementState();

        switch (CurrentMoveState)
        {
            case MoveState.Grounded:
                GroundedMovement();
                LimitGroundSpeed();
                break;
            case MoveState.WallRunning:
                WallRunningMovement();
                break;
            case MoveState.Flying:
                FlyingMovement();
                LimitAirSpeed();
                break;
        }
        previousMoveState = CurrentMoveState;

        if(isRotating)
        {
            LerpToNewRotation();
        }

        WrapCheck();
    }


    void Update()
    {
        //forwardspeedText.text = transform.InverseTransformDirection(rb.velocity).z.ToString("F2");
        //strafeText.text = transform.InverseTransformDirection(rb.velocity).x.ToString("F2");
        //strafeText.text = (rb.velocity).x.ToString("F2");

        JumpCheck();
        


    }
    
    void JumpCheck()
    {
        // Coyote time
        if (IsGrounded() || IsWallRunning())
        {
            coyoteTimer = 0;
        }
        else
        {
            coyoteTimer += Time.deltaTime;
        }
        // Jump buffer
        if (playerInput.actions["Jump"].WasPressedThisFrame())
        {
            jumpBufferTimer = 0;
        }
        else
        {
            jumpBufferTimer += Time.deltaTime;
        }

        if (jumpBufferTimer < jumpBufferDuration && coyoteTimer < coyoteTimeDuration)
        {
            Jump();
        }
    }
    
    private void ResetRelativeYVelocity()
    {
        SetRelativeVelocity(new Vector3(GetRelativeVelocity().x, 0, GetRelativeVelocity().z));
    }
    [SerializeField] float groundDrag;
    void GroundedMovement()
    {
        rb.drag = groundDrag;
        Vector3 groundRight = transform.right * (InputDirection().x * groundStrafeForce);
        Vector3 groundForward;
        // Input forwards
        if (InputDirection().z > 0)
        {
            groundForward = transform.forward * (InputDirection().z * groundForwardForce);
        }
        // Input backwards
        else
        {
            groundForward = transform.forward * (InputDirection().z * groundForwardForce * 0.5f);
        }
        Vector3 groundMoveDirection = groundRight + groundForward;

        rb.AddForce(groundMoveDirection * 10, ForceMode.Force);

        //ResetRelativeYVelocity();
    }
    void FlyingMovement()
    {
        rb.drag = 0;

        Vector3 inputDirection = InputDirection();

        // Calculate the forces
        Vector3 airRight = transform.right * (inputDirection.x * airStrafeForce);
        Vector3 airUp = transform.up * GetDownForce();
        Vector3 airForward = transform.forward * (inputDirection.z * airForwardForce);
        Vector3 airborneMoveDirection = airRight + airUp + airForward;
        // Apply the forces
        rb.AddForce(airborneMoveDirection, ForceMode.Force);
    }
    float GetDownForce()
    {
        if (isRotating)
        {
            return 0;
        }
        if(GetRelativeVelocity().y > apexYVelocity)
        {
            return -downForceBeforeApex;
        }
        else
        {
            return -downForceAfterApex;
        }
    }
    void WallRunningMovement()
    {
        if(GetRelativeVelocity().y > 0)
        {
            //SetRelativeVelocity(new Vector3(GetRelativeVelocity().x, GetRelativeVelocity().y * 0.5f, GetRelativeVelocity().z));
        }
        else
        {
            //SetRelativeVelocity(new Vector3(GetRelativeVelocity().x, wallRunDownForceBig, GetRelativeVelocity().z));
        }
        rb.drag = 0;
        Vector3 airborneMoveDirection = transform.forward * (InputDirection().z * wallRunForwardForce);
        rb.AddForce(airborneMoveDirection, ForceMode.Force);
    }
    void Jump()
    {
        //Grounded or just left ground
        if (readyToJump) { //can jump set in cooroutine     
            readyToJump = false;
            Invoke(nameof(ResetJump), jumpCooldown);

            ResetRelativeYVelocity();
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
        if (CurrentMoveState == MoveState.WallRunning && readyToJump)
        { 
            //can jump set in cooroutine
            readyToJump = false;
            Invoke(nameof(ResetJump), jumpCooldown);

            ResetRelativeYVelocity();
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }
    
    void ResetJump()
    {
        readyToJump = true;
    }
    Vector3 GetRelativeVelocity()
    {
        return transform.InverseTransformDirection(rb.velocity);
    }
    private void SetRelativeVelocity(Vector3 newVelocity)
    {
        rb.velocity = transform.TransformDirection(newVelocity);
    }
    void LimitAirSpeed()
    {
        // X
        if (GetRelativeVelocity().x > maxAirStrafeSpeed)
        {
            SetRelativeVelocity(new Vector3(maxAirStrafeSpeed, GetRelativeVelocity().y, GetRelativeVelocity().z));
        }
        if (GetRelativeVelocity().x < -maxAirStrafeSpeed)
        {
            SetRelativeVelocity(new Vector3(-maxAirStrafeSpeed, GetRelativeVelocity().y, GetRelativeVelocity().z));
        }
        // Y
        if (GetRelativeVelocity().y < maxFallSpeed)
        {
            SetRelativeVelocity(new Vector3(GetRelativeVelocity().x, maxFallSpeed, GetRelativeVelocity().z));
        }
        // Z
        if (GetRelativeVelocity().z > maxAirForwardSpeed)
        {
            SetRelativeVelocity(new Vector3(GetRelativeVelocity().x, GetRelativeVelocity().y, maxAirForwardSpeed));
        }
        if (GetRelativeVelocity().z < -maxAirForwardSpeed)
        {
            SetRelativeVelocity(new Vector3(GetRelativeVelocity().x, GetRelativeVelocity().y, -maxAirForwardSpeed));
        }
        
    }
    void LimitGroundSpeed()
    {
        // X
        if (GetRelativeVelocity().x > maxGroundStrafeSpeed)
        {
            SetRelativeVelocity(new Vector3(maxGroundStrafeSpeed, GetRelativeVelocity().y, GetRelativeVelocity().z));
        }
        if (GetRelativeVelocity().x < -maxGroundStrafeSpeed)
        {
            SetRelativeVelocity(new Vector3(-maxGroundStrafeSpeed, GetRelativeVelocity().y, GetRelativeVelocity().z));
        }
        // Z
        if (GetRelativeVelocity().z > maxGroundForwardSpeed)
        {
            SetRelativeVelocity(new Vector3(GetRelativeVelocity().x, GetRelativeVelocity().y, maxGroundForwardSpeed));
        }
        if (GetRelativeVelocity().z < -maxGroundForwardSpeed)
        {
            SetRelativeVelocity(new Vector3(GetRelativeVelocity().x, GetRelativeVelocity().y, -maxGroundForwardSpeed));
        }
       
    }
    
    private Vector3 InputDirection()
    {
        return new Vector3(playerInput.actions["Move"].ReadValue<Vector2>().x, 0, playerInput.actions["Move"].ReadValue<Vector2>().y).normalized;
    }
    private Vector3 newSurfaceNormal;
    float timeRotating = 0.0f; // Initialize the interpolation factor
    [SerializeField] float rotateSpeed;
    public void ChangeRotation(Vector3 surfaceNormal, Vector3 hitPos)
    {
        if (isRotating)
            return;
        // Interpolation is complete.
        newSurfaceNormal = surfaceNormal;
        isRotating = true;

    }
    void LerpToNewRotation()
    {
        // Use the Lerp function with the calculated t value.
        timeRotating += rotateSpeed * Time.deltaTime; // Increment t based on your desired rotation speed.

        // Use the Lerp function with the calculated t value.
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up, newSurfaceNormal) * transform.rotation, timeRotating);

        if (GetRelativeVelocity().y < apexYVelocity)
        {
            rb.AddForce(-newSurfaceNormal * downForceBeforeApex, ForceMode.Force);

        }
        else
        {
            rb.AddForce(-newSurfaceNormal * downForceWhenRotating, ForceMode.Force);
        }

        if (timeRotating >= 1.0f)
        {
            // Interpolation is complete.
            isRotating = false;
            timeRotating = 0f;
        }
    }
    
    void WrapCheck()
    {
        float boundsMax = levelRepeater.RepeatSpacing / 2f;
        if (transform.position.x > boundsMax)
        {
            WrapTo(new Vector3(-boundsMax, transform.position.y, transform.position.z));
        }
        if (transform.position.x < -boundsMax)
        {
            WrapTo(new Vector3(boundsMax, transform.position.y, transform.position.z));
        }

        if (transform.position.z > boundsMax)
        {
            WrapTo(new Vector3(transform.position.x, transform.position.y, -boundsMax));
        }
        if (transform.position.z < -boundsMax)
        {
            WrapTo(new Vector3(transform.position.x, transform.position.y, boundsMax));
        }

        if (transform.position.y > boundsMax)
        {
            WrapTo(new Vector3(transform.position.x, -boundsMax, transform.position.z));
        }
        if (transform.position.y < -boundsMax)
        {
            WrapTo(new Vector3(transform.position.x, boundsMax, transform.position.z));
        }

    }
    // move dups first test
    
    void WrapTo(Vector3 newPos)
    {
        duplicateManager.WrapTo(newPos);
        // move dups first
        transform.position = newPos;
        // move player

        Vector3 diff = newPos - transform.position;

       

        // move players grappling hook
        PlayerShooting playerShooting = GetComponentInChildren<PlayerShooting>();

        if (playerShooting.IsGrappling())
            playerShooting.SetGrapplingPoint(playerShooting.GetGrapplePoint() + diff);
    }
    bool IsGrounded()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position, -transform.up * distToGround);
        if (Physics.Raycast(transform.position, -transform.up, out hit, distToGround))
        {
            if (hit.collider.isTrigger)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
            return false;
    }
    bool IsWallRunning()
    {
        if (IsWallRunningLeft() || IsWallRunningRight())
        {
            return true;
        }
        return false;
    }
    bool IsWallRunningLeft()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.right, out hit, distToWallRun))
        {
            if (hit.collider.isTrigger)
            {
                return false;
            }
            else
            {
                return true;
            }
        }            
        else
            return false;
    }
    bool IsWallRunningRight()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.right, out hit, distToWallRun))
        {
            if (hit.collider.isTrigger)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
            return false;
    }
}
//#region Collisions

//#region MathGenious
//Vector2 ClampedAdditionVector(Vector2 a, Vector2 b)
//{
//    float k, x, y;
//    k = Mathf.Sqrt(Mathf.Pow(a.x, 2) + Mathf.Pow(a.y, 2)) / Mathf.Sqrt(Mathf.Pow(a.x + b.x, 2) + Mathf.Pow(a.y + b.y, 2));
//    x = k * (a.x + b.x) - a.x;
//    y = k * (a.y + b.y) - a.y;
//    return new Vector2(x, y);
//}

//Vector3 RotateToPlane(Vector3 vect, Vector3 normal)
//{
//    Vector3 rotDir = Vector3.ProjectOnPlane(normal, Vector3.up);
//    Quaternion rotation = Quaternion.AngleAxis(-90f, Vector3.up);
//    rotDir = rotation * rotDir;
//    float angle = -Vector3.Angle(Vector3.up, normal);
//    rotation = Quaternion.AngleAxis(angle, rotDir);
//    vect = rotation * vect;
//    return vect;
//}

//float WallrunCameraAngle()
//{
//    Vector3 rotDir = Vector3.ProjectOnPlane(groundNormal, Vector3.up);
//    Quaternion rotation = Quaternion.AngleAxis(-90f, Vector3.up);
//    rotDir = rotation * rotDir;
//    float angle = Vector3.SignedAngle(Vector3.up, groundNormal, Quaternion.AngleAxis(90f, rotDir) * groundNormal);
//    angle -= 90;
//    angle /= 180;
//    Vector3 playerDir = transform.forward;
//    Vector3 normal = new Vector3(groundNormal.x, 0, groundNormal.z);

//    return Vector3.Cross(playerDir, normal).y * angle;
//}