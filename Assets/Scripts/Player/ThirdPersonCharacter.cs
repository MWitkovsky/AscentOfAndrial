using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class ThirdPersonCharacter : MonoBehaviour {

    public HealthBar healthBar;
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
    private Vector3 normalizedVelocity;
    private float origGroundCheckDistance;
	//private float origGravityMultiplier;
    private float dashTimer;
    private float jumpTimer;
    private float castTimer;
    private float landAnimDelay; //jumping off ground is broken because of multiple triggers on same frame, this fixes that
    private int numberOfAirJumps;
    private int textboxReadDelay;
    private int groundCheckDelay;
    private bool isFrozen;
    private bool isTextboxOpen;
    private bool isGrounded;
    private bool isDodging;
    private bool isDashing;
    private bool isGrinding;
    private bool isHoming;
    private bool isHit;
    private bool shouldDie;
    private bool dead;
    private ThirdPersonUserControl.Spell currentSpell;
	private ThirdPersonUserControl.Direction dashDirection; 

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
        if (isHoming || dead)
        {
            return;
        }

        if (isGrinding)
        {
            HandleGrindingMovement(jump);
            return;
        }

        if (isHit)
        {
            if (!shouldDie)
            {
                HandleAirborneMovement(jump);
            }
            CheckGroundStatus(jump);
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

            normalizedVelocity = move.normalized;

            if (isGrounded)
            {
                HandleGroundedMovement(jump);
            }
            else
            {
                HandleAirborneMovement(jump);
            }

            Physics.IgnoreLayerCollision(2, 8, true);
            RaycastHit wallCheck;

            if (rb.SweepTest(normalizedVelocity, out wallCheck, move.magnitude * Time.fixedDeltaTime) && !isGrounded)
            {
                rb.velocity = new Vector3(0.0f, rb.velocity.y, 0.0f);
                //rb.velocity = new Vector3(-move.x, rb.velocity.y, -move.z);
                rb.AddForce(-move*10);
            }
            else
            {
                rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
            }

            Physics.IgnoreLayerCollision(2, 8, false);

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
            groundCheckDelay = 10;
            dashTimer = 0.0f;

            if (move.magnitude > 1.0f)
            {
                move.Normalize();
            }

            move = transform.InverseTransformDirection(move);
            move = Vector3.ProjectOnPlane(move, groundNormal);
            move *= moveSpeedMultiplier;
            jumpTimer = landAnimDelay;

            if (dashDirection == ThirdPersonUserControl.Direction.Forward)
            {
                anim.SetFloat("moveZ", 1.0f);
                anim.SetFloat("moveX", 0.0f);
            }

            if (dashDirection == ThirdPersonUserControl.Direction.Back)
            {
                anim.SetFloat("moveZ", -1.0f);
                anim.SetFloat("moveX", 0.0f);
            }

            if (dashDirection == ThirdPersonUserControl.Direction.Left)
            {
                anim.SetFloat("moveZ", 0.0f);
                anim.SetFloat("moveX", -1.0f);
            }

            if (dashDirection == ThirdPersonUserControl.Direction.Right)
            {
                anim.SetFloat("moveZ", 0.0f);
                anim.SetFloat("moveX", 1.0f);
            }

            anim.SetBool("isDashing", isDodging);
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

        /*
		float dashDirZ = 0.0f;
		float dashDirX = 0.0f;
		if(Camera.main.transform.forward.z < 0.0f)
			dashDirZ = Camera.main.transform.forward.z - move.z;

		if(Camera.main.transform.forward.z >= 0.0f)
			dashDirZ = -1.0f * (Camera.main.transform.forward.z - move.z);

		if(Camera.main.transform.right.x < 0.0f)
			dashDirX = Camera.main.transform.right.x - move.x;

		if(Camera.main.transform.right.x > 0.0f)
			dashDirX = -1.0f * (Camera.main.transform.right.x - move.x);
        */


		rb.velocity = new Vector3(move.x * dashSpeedMultiplier, 1.0f, move.z * dashSpeedMultiplier);

		if(dashDirection == ThirdPersonUserControl.Direction.Forward)
		{
			anim.SetFloat("moveZ", 1.0f);
			anim.SetFloat("moveX", 0.0f);
		}

		if(dashDirection == ThirdPersonUserControl.Direction.Back)
		{
			anim.SetFloat("moveZ", -1.0f);
			anim.SetFloat("moveX", 0.0f);
		}

		if(dashDirection == ThirdPersonUserControl.Direction.Left)
		{
			anim.SetFloat("moveZ", 0.0f);
			anim.SetFloat("moveX", -1.0f);
		}
		
		if(dashDirection == ThirdPersonUserControl.Direction.Right)
		{
			anim.SetFloat("moveZ", 0.0f);
			anim.SetFloat("moveX", 1.0f);
		}

		/*
        Debug.Log("MoveZ: " + dashDirZ);
		Debug.Log("MoveX: " + dashDirX);
        */
        anim.SetBool("isDashing", isDashing);
		rb.useGravity = false;
        jumpTimer = landAnimDelay;

        numberOfAirJumps++;
    }

    public void AirAttack()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Slash") && !isHit && !isGrounded)
        {
            anim.SetTrigger("AirAttackSwipe");
            attackRadius.enabled = true;
        }
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
            //groundCheckDistance = rb.velocity.y < 0 ? origGroundCheckDistance : 0.01f;
            groundCheckDistance = origGroundCheckDistance;
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

            isHit = false;
            numberOfAirJumps++;
            rb.velocity = new Vector3(rb.velocity.x, airJumpPower, rb.velocity.z);
        }

        //groundCheckDistance = rb.velocity.y < 0 ? origGroundCheckDistance : 0.01f;
        groundCheckDistance = origGroundCheckDistance;
    }

    private void HandleGrindingMovement(bool jump)
    {
        HandleGroundedMovement(jump);
    }

    private void CheckGroundStatus(bool jump)
    {
        if(groundCheckDelay > 0)
        {
            return;
        }

        RaycastHit hitInfo;

        if(Physics.Raycast(transform.position, Vector3.down, out hitInfo, groundCheckDistance))
        {
            if (!jump && groundCheckDistance == origGroundCheckDistance && hitInfo.transform.gameObject.CompareTag("Ground"))
            {
                groundNormal = hitInfo.normal;
                isGrounded = true;
                isDodging = false;
                isHit = false;
                numberOfAirJumps = 0;

                if (shouldDie)
                {
                    dead = true;
                    anim.SetTrigger("Die");
                    rb.velocity = Vector3.zero;
                    GetComponent<CapsuleCollider>().height = 0.0f;
                    GetComponent<CapsuleCollider>().radius = 0.4f;
                }
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
                anim.SetBool("isHoming", isHoming);
            }
        }

        jumpTimer -= Time.deltaTime;
        castTimer -= Time.deltaTime;
        groundCheckDelay--;
        if (!isTextboxOpen)
        {
            textboxReadDelay--;
        }

        //had to do this here because it broke if called too early
        if (isFrozen)
        {
            if (rb)
            {
                rb.isKinematic = true;
                rb.useGravity = false;

                anim.speed = 0.0f;
            }
        }

        anim.SetFloat("jumpTimer", jumpTimer);
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

    public void SetHoming(bool isHoming, bool isSpectralHand)
    {
        this.isHoming = isHoming;
        isGrounded = !isHoming;
        anim.SetBool("isDashing", isHoming);
        anim.SetBool("isHoming", isHoming);
        if (isHoming)
        {
            anim.SetFloat("moveZ", 1.0f);
            if (isSpectralHand)
            {
                anim.SetTrigger("Dash");
            }
        }
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

    //For spectral hand deletions
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

	public void ChangeDashDir(ThirdPersonUserControl.Direction dir)
	{
		dashDirection = dir;
	}

    //Took damage
    public void Hit(Vector3 direction)
    {
        isHit = true;
        anim.SetTrigger("Hit");
        rb.velocity = (direction * moveSpeedMultiplier) + (Vector3.up * moveSpeedMultiplier / 2.0f);
    }

    public void Kill()
    {
        shouldDie = true;
        groundCheckDistance = origGroundCheckDistance;
    }

    public void KillImmediately()
    {
        healthBar.applyDamage(100);
        shouldDie = true;
        dead = true;
        anim.SetTrigger("Die");
        rb.velocity = rb.velocity - new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);
        GetComponent<CapsuleCollider>().height = 0.0f;
        GetComponent<CapsuleCollider>().radius = 0.4f;
    }

    public void OpenTextbox()
    {
        isTextboxOpen = true;
        //pause physics and animations
        if (rb)
        {
            rb.isKinematic = true;
            rb.useGravity = false;

            anim.speed = 0.0f;
        }

        textboxReadDelay = 10;
    }

    public void CloseTextbox()
    {
        isTextboxOpen = false;
        //restart physics and animations
        rb.isKinematic = false;
        rb.useGravity = true;
        anim.speed = 1.0f;

        textboxReadDelay = 10;
    }

    public void SplashScreenFreeze()
    {
        isFrozen = true;
        textboxReadDelay = 10;
    }

    public void EndSplashScreen()
    {
        isFrozen = false;

        //restart physics and animations
        rb.isKinematic = false;
        rb.useGravity = true;
        anim.speed = 1.0f;

        textboxReadDelay = 10;
    }

    public bool IsFrozen()
    {
        return isFrozen;
    }

    public bool IsGrinding()
    {
        return isGrinding;
    }

    public bool IsHoming()
    {
        return isHoming;
    }

    public bool isVulnerable()
    {
        return !isHit;
    }

    public bool IsDead()
    {
        return dead || shouldDie;
    }

    public bool IsTextboxOpen()
    {
        return isTextboxOpen;
    }

    public bool CanOpenTextbox()
    {
        return textboxReadDelay <= 0;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public bool IsDashing()
    {
        return isDashing;
    }
}
