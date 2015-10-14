using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class ThirdPersonCharacter : MonoBehaviour {

    public float movingTurnSpeed = 360.0f;
    public float stationaryTurnSpeed = 180.0f;
    public float groundJumpPower = 12.0f;
    public float airJumpPower = 8.0f;
    [Range(1f, 4f)]public float gravityMultiplier = 1.0f;
    public float moveSpeedMultiplier = 1.0f;
    public float dashSpeedMultiplier = 2.0f;
    public float dashTime = 0.2f;
    public float groundCheckDistance = 1.0f;
    public int maxNumberOfAirJumps = 3;

    private Rigidbody rb;
    private Vector3 groundNormal;
    private Vector3 prevVelocity;
    private float origGroundCheckDistance;
    private float dashTimer;
    private int numberOfAirJumps;
    private bool isGrounded;
    private bool isDodging;
    private bool isDashing;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        origGroundCheckDistance = groundCheckDistance;
    }
	
	public void Move(Vector3 move, bool jump)
    {
        CheckGroundStatus();
        if (!isDodging && !isDashing)
        {
            if (move.magnitude > 1.0f)
            {
                move.Normalize();
            }

            move = transform.InverseTransformDirection(move);
            move = Vector3.ProjectOnPlane(move, groundNormal);

            if (isGrounded)
            {
                HandleGroundedMovement(jump);
            }
            else
            {
                HandleAirborneMovement(jump);
            }

            move *= moveSpeedMultiplier;

            rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
        }
        if (jump && isDodging)
        {
            isDodging = false;
            HandleAirborneMovement(jump);
        }
    }

    //Called from User Control, calls Dash if requested while airborne
    public void Dodge(Vector3 move)
    {
        if (!isGrounded)
        {
            if(numberOfAirJumps < maxNumberOfAirJumps && !isDashing)
            {
                Dash(move);
            }
        }
        if (!isDodging && isGrounded)
        {
            isDodging = true;

            if (move.magnitude > 1.0f)
            {
                move.Normalize();
            }

            move = transform.InverseTransformDirection(move);
            move = Vector3.ProjectOnPlane(move, groundNormal);
            move *= moveSpeedMultiplier;

            rb.velocity = new Vector3(move.x*2.0f, airJumpPower/3.0f, move.z*2.0f);
        }
    }

    //used in the air, consumes a jump
    private void Dash(Vector3 move)
    {
        isDashing = true;
        dashTimer = 0.0f;

        if (move.magnitude > 1.0f)
        {
            move.Normalize();
        }

        move = transform.InverseTransformDirection(move);
        move = Vector3.ProjectOnPlane(move, groundNormal);
        move *= moveSpeedMultiplier;

        rb.velocity = new Vector3(move.x * dashSpeedMultiplier, airJumpPower / 3.0f, move.z * dashSpeedMultiplier);
        rb.useGravity = false;

        numberOfAirJumps++;
    }

    private void HandleGroundedMovement(bool jump)
    {
        // check whether conditions are right to allow a jump:
        if (jump && isGrounded)
        {
            // jump!
            rb.velocity = new Vector3(rb.velocity.x, groundJumpPower, rb.velocity.z);
            isGrounded = false;
            groundCheckDistance = 0.5f;
        }
    }

    private void HandleAirborneMovement(bool jump)
    {
        if(gravityMultiplier > 1.0f)
        {
            Vector3 extraGravityForce = (Physics.gravity * gravityMultiplier) - Physics.gravity;
            rb.AddForce(extraGravityForce);
        }

        if (jump && numberOfAirJumps < maxNumberOfAirJumps)
        {
            numberOfAirJumps++;
            rb.velocity = new Vector3(rb.velocity.x, airJumpPower, rb.velocity.z);
        }

        groundCheckDistance = rb.velocity.y < 0 ? origGroundCheckDistance : 0.01f;
    }

    private void CheckGroundStatus()
    {
        RaycastHit hitInfo;
#if UNITY_EDITOR
        Debug.DrawLine(transform.position, transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance));
#endif

        if(Physics.Raycast(transform.position, Vector3.down, out hitInfo, groundCheckDistance))
        {
            groundNormal = hitInfo.normal;
            isGrounded = true;
            isDodging = false;
            numberOfAirJumps = 0;
        }
        else
        {
            isGrounded = false;
            groundNormal = Vector3.up;
        }
    }

    private void Update()
    {
        dashTimer += Time.deltaTime;
        if(dashTimer > dashTime)
        {
            isDashing = false;
            rb.useGravity = true;
        }
    }
}
