using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class ThirdPersonCharacter : MonoBehaviour {

    public SphereCollider attackRadius;
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
    private Animator anim;
    private AnimatorStateInfo animInfo; //For later use, used for polling which anim state we're in to restrict controls
    private Vector3 groundNormal;
    private Vector3 prevVelocity;
    private float origGroundCheckDistance;
	//private float origGravityMultiplier;
    private float dashTimer;
    private float jumpTimer;
    private float landAnimDelay; //jumping off ground is broken because of multiple triggers on same frame, this fixes that
    private int numberOfAirJumps;
    private bool isGrounded;
    private bool isDodging;
    private bool isDashing;
    private bool isGrinding;
    private bool isHoming;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        origGroundCheckDistance = groundCheckDistance;
		//origGravityMultiplier = gravityMultiplier;

        landAnimDelay = 0.05f;

        anim = GetComponent<Animator>();

        attackRadius.enabled = false;
    }
	
	public void Move(Vector3 move, bool jump)
    {
        if (isGrinding)
        {
            HandleGrindingMovement(jump);
            return;
        }

        if (isHoming)
        {
            return;
        }

        if (!jump)
        {
            CheckGroundStatus(jump);
        }
        
        if (!isDodging && !isDashing)
        {
            if (move.magnitude > 1.0f)
            {
                move.Normalize();
            }

            anim.SetFloat("moveSpeed", move.magnitude);
				
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
				isDodging = false;
                Dash(move);
            }
        }
        if (!isDodging && isGrounded)
        {
            isDodging = true;
            dashTimer = 0.0f;

            if (move.magnitude > 1.0f)
            {
                move.Normalize();

            }

            move = transform.InverseTransformDirection(move);
            move = Vector3.ProjectOnPlane(move, groundNormal);
            move *= moveSpeedMultiplier;
            jumpTimer = landAnimDelay;

            rb.velocity = new Vector3(move.x*2.0f, airJumpPower/3.0f, move.z*2.0f);
        }
    }

    //used when dodging in the air, consumes a jump
    private void Dash(Vector3 move)
    {
		anim.SetTrigger("Dash");
        isDashing = true;
        dashTimer = 0.0f;

        if (move.magnitude > 1.0f)
        {
            move.Normalize();
        }


        move = transform.InverseTransformDirection(move);
        move = Vector3.ProjectOnPlane(move, groundNormal);
        move *= moveSpeedMultiplier;

		float dashDir = 0.0f;
		if(Camera.main.transform.forward.z < 0.0f)
		dashDir = Camera.main.transform.forward.z - move.z;

		if(Camera.main.transform.forward.z >= 0.0f)
		dashDir = -1.0f * (Camera.main.transform.forward.z - move.z);
		
		rb.velocity = new Vector3(move.x * dashSpeedMultiplier, 1.0f, move.z * dashSpeedMultiplier);

		anim.SetFloat("moveZ", dashDir);
		Debug.Log(dashDir);
		rb.useGravity = false;
        jumpTimer = landAnimDelay;

        numberOfAirJumps++;
    }

    public void AirAttack()
    {
        attackRadius.enabled = true;
        //anim.SetTrigger("AirAttack");
    }

    private void HandleGroundedMovement(bool jump)
    {
        // check whether conditions are right to allow a jump:
        if (jump && isGrounded)
        {
            // jump!
            anim.SetTrigger("Jump");
            jumpTimer =landAnimDelay;

            rb.velocity = new Vector3(rb.velocity.x, groundJumpPower, rb.velocity.z);
            isGrounded = false;
            groundCheckDistance = rb.velocity.y < 0 ? origGroundCheckDistance : 0.01f;
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
            anim.SetTrigger("Jump");
            jumpTimer = landAnimDelay;

            numberOfAirJumps++;
            rb.velocity = new Vector3(rb.velocity.x, airJumpPower, rb.velocity.z);
        }

        groundCheckDistance = rb.velocity.y < 0 ? origGroundCheckDistance : 0.01f;
    }

    private void HandleGrindingMovement(bool jump)
    {
        HandleGroundedMovement(jump);
    }

    private void CheckGroundStatus(bool jump)
    {
        //Raycast version adapted from standard assets, doesn't work half the time...
        RaycastHit hitInfo;
#if UNITY_EDITOR
        Debug.DrawLine(transform.position, transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance));
#endif

        if(Physics.Raycast(transform.position, Vector3.down, out hitInfo, groundCheckDistance))
        {
            if (!jump && groundCheckDistance == origGroundCheckDistance && hitInfo.transform.gameObject.CompareTag("Ground"))
            {
                groundNormal = hitInfo.normal;
                isGrounded = true;
                isDodging = false;
                numberOfAirJumps = 0;
            }
        }
        else
        {
            isGrounded = false;
            groundNormal = Vector3.up;
        }

        anim.SetBool("isGrounded", isGrounded);
    }

    private void Update()
    {
        dashTimer += Time.deltaTime;
        if(dashTimer > dashTime)
        {
            isDodging = false;
            isDashing = false;
            if (!isGrinding)
            {
                rb.useGravity = true;
            }
        }

        jumpTimer -= Time.deltaTime;
        anim.SetFloat("jumpTimer", jumpTimer);
    }

    public bool IsGrinding()
    {
        return isGrinding;
    }

    public void SetGrinding(bool isGrinding)
    {
        this.isGrinding = isGrinding;
		anim.SetBool("isGrinding", isGrinding);
        if (isGrinding)
        {
            isGrounded = true;
            isDodging = false;
            numberOfAirJumps = 0;
        }
    }

    public void setHoming(bool isHoming)
    {
        this.isHoming = isHoming;
    }

    public bool IsHoming()
    {
        return isHoming;
    }

    public Animator getAnimator()
    {
        return anim;
    }

    public float GetJumpTimer()
    {
        return jumpTimer;
    }
}
