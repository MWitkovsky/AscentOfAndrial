using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class ThirdPersonCharacter : MonoBehaviour {

    public GameObject fireballPrefab, groundSpikePrefab, spectralHandPrefab;
    public SphereCollider attackRadius;
    public float maxSpellRange = 30.0f;
    public float spellCooldown = 2.0f;
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

    private LayerMask groundSpikeLayerMask;
    private Animator anim;
    private Rigidbody rb;
    private SpectralHandler spectralHand1, spectralHand2;
    private Vector3 groundNormal;
    private Vector3 prevVelocity;
    private float origGroundCheckDistance;
	//private float origGravityMultiplier;
    private float dashTimer;
    private float jumpTimer;
    private float castTimer;
    private float landAnimDelay; //jumping off ground is broken because of multiple triggers on same frame, this fixes that
    private int numberOfAirJumps;
    private bool isGrounded;
    private bool isDodging;
    private bool isDashing;
    private bool isGrinding;
    private bool isHoming;
    private ThirdPersonUserControl.Spell currentSpell;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        origGroundCheckDistance = groundCheckDistance;
		//origGravityMultiplier = gravityMultiplier;

        landAnimDelay = 0.05f;

        anim = GetComponent<Animator>();

        attackRadius.enabled = false;

        groundSpikeLayerMask = (1 << LayerMask.NameToLayer("Ground"));
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

            move *= moveSpeedMultiplier;

            if (isGrounded)
            {
                HandleGroundedMovement(jump);
                rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
            }
            else
            {
                HandleAirborneMovement(jump);
                rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
            }
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
        anim.SetBool("isDashing", isDashing);
		rb.useGravity = false;
        jumpTimer = landAnimDelay;

        numberOfAirJumps++;
    }

    public void AirAttack()
    {
        attackRadius.enabled = true;
    }

    public void CastSpell(Vector3 position, Vector3 direction, Transform characterModel)
    {
        if(castTimer < 0.0f)
        {
            if (currentSpell == ThirdPersonUserControl.Spell.Fireball)
            {
                GameObject fireball = (GameObject)Instantiate(fireballPrefab, transform.position + direction*3.0f, transform.rotation);

                fireball.GetComponent<FireballHandler>().Launch(direction);
            }
            else if (currentSpell == ThirdPersonUserControl.Spell.GroundSpike)
            {
                RaycastHit hitInfo;

                if (Physics.Raycast(position, direction, out hitInfo, maxSpellRange, groundSpikeLayerMask))
                {
                    Vector3 spawnPosition = new Vector3(hitInfo.point.x, hitInfo.point.y - groundSpikePrefab.transform.localScale.y - 0.5f, hitInfo.point.z);
                    Instantiate(groundSpikePrefab, spawnPosition, transform.rotation);
                }
            }
            else
            {
                if (spectralHand1)
                {
                    spectralHand2 = spectralHand1;
                }

                GameObject hand = (GameObject)Instantiate(spectralHandPrefab, transform.position, transform.rotation);
                
                spectralHand1 = hand.GetComponent<SpectralHandler>();

                spectralHand1.setDirection(direction);
                spectralHand1.player = this;
                spectralHand1.characterModel = characterModel;
            }

            castTimer = spellCooldown;
        }
       
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
        RaycastHit hitInfo;

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
        //I accidentally did this one with addition instead of subtraction...
        //same result but inconsistent with the negative timers. Oh well.
        dashTimer += Time.deltaTime;
        if(dashTimer > dashTime)
        {
            isDodging = false;
            isDashing = false;
            if (!isGrinding)
            {
                rb.useGravity = true;
            }

            if (!isHoming)
            {
                anim.SetBool("isDashing", isDashing);
            }
        }

        jumpTimer -= Time.deltaTime;
        castTimer -= Time.deltaTime;

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

    public void SetHoming(bool isHoming)
    {
        this.isHoming = isHoming;
        anim.SetBool("isDashing", isHoming);
        if (isHoming)
        {
            anim.SetFloat("moveZ", 1.0f);
            anim.SetTrigger("Dash");
        }
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

    public void ChangeSpell(ThirdPersonUserControl.Spell spell)
    {
        currentSpell = spell;
    }

    public ThirdPersonUserControl.Spell GetSpell()
    {
        return currentSpell;
    }

    public void resetAirJumps()
    {
        numberOfAirJumps = 0;
    }

    public void resetCastTimer()
    {
        castTimer = 0.0f;
    }

    public void deleteHand(SpectralHandler handToDelete)
    {
        if(handToDelete == spectralHand1)
        {
            spectralHand1 = null;
        }
        else
        {
            spectralHand2 = null;
        }
    }

    public void destroyOtherHand(SpectralHandler callingHand)
    {
        if (spectralHand2 == callingHand)
        {
            if (spectralHand1)
            {
                Destroy(spectralHand1.gameObject);
                spectralHand1 = null;
            }
        }
        else
        {
            if (spectralHand2)
            {
                Destroy(spectralHand2.gameObject);
                spectralHand2 = null;
            }
        }
    }
}
