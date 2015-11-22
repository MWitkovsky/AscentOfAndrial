using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

[RequireComponent(typeof(ThirdPersonCharacter))]
public class ThirdPersonUserControl : MonoBehaviour {

    public Transform characterModel;
    private ThirdPersonCharacter character;
    private Transform cam;
    private Vector3 camForward;
    private Vector3 move;
    private Quaternion characterRotation;
    private bool jump;
    private bool dodge;
    private bool attack;
    private Direction dodgeDirection;

    public enum Direction {Forward, Back, Left, Right, FR, FL, BR, BL};
    public enum Spell {Fireball, GroundSpike, SpectralHand};

    // Use this for initialization
    void Start () {
        // get the transform of the main camera
        if (Camera.main != null)
        {
            cam = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning(
                "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
            // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
        }

        // get the third person character ( this should never be null due to require component )
        character = GetComponent<ThirdPersonCharacter>();
    }

    private void Update() {
        if (character.isDead())
        {
            return;
        }

        if (!jump)
        {
            jump = CrossPlatformInputManager.GetButtonDown("Jump");
        }

        if (!dodge)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (Input.GetKey(KeyCode.W)) //Forward
                {
                    dodge = true;
                    if (Input.GetKey(KeyCode.D))
                    {
                        dodgeDirection = Direction.FR;
                    }
                    else if (Input.GetKey(KeyCode.A))
                    {
                        dodgeDirection = Direction.FL;
                    }
                    else
                    {
                        dodgeDirection = Direction.Forward;
						character.ChangeDashDir(dodgeDirection);
                    }
                }
                else if (Input.GetKey(KeyCode.S)) //Backward
                {
                    dodge = true;
                    if (Input.GetKey(KeyCode.D))
                    {
                        dodgeDirection = Direction.BR;
                    }
                    else if (Input.GetKey(KeyCode.A))
                    {
                        dodgeDirection = Direction.BL;
                    }
                    else
                    {
                        dodgeDirection = Direction.Back;
						character.ChangeDashDir(dodgeDirection);
                    }
                }
                else if (Input.GetKey(KeyCode.A)) //Left
                {
                    dodge = true;
                    dodgeDirection = Direction.Left;
					character.ChangeDashDir(dodgeDirection);
                }
                else if (Input.GetKey(KeyCode.D)) //Right
                {
                    dodge = true;
                    dodgeDirection = Direction.Right;
					character.ChangeDashDir(dodgeDirection);
                }
            }
        }

        if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            character.AirAttack();
        }

        if (CrossPlatformInputManager.GetButtonDown("Fire2"))
        {
            character.CastSpell(cam.position, cam.forward, characterModel);
        }

        //Selecting which spell is being readied
        //Alpha refers to the top row of numbers on the keyboard vs the numpad
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            character.ChangeSpell(Spell.Fireball);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            character.ChangeSpell(Spell.GroundSpike);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            character.ChangeSpell(Spell.SpectralHand);
        }
    }

    private void FixedUpdate()
    {
        if (character.isDead())
        {
            character.Move(Vector3.zero, false);
            return;
        }

        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");

        if(cam != null)
        {
            camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
            move = v * camForward + h * cam.right;
        }
        else
        {
            move = v * Vector3.forward + h * Vector3.right;
        }

        //Set player's rotation if moving and not on some sort of on-rails thing
        if ((h != 0.0f || v != 0.0f) && !character.IsGrinding() && !character.IsHoming())
        {
            characterRotation = cam.transform.rotation;
            characterRotation.x = 0;
            characterRotation.z = 0;
            characterModel.rotation = characterRotation;
        }

        if (dodge)
        {
            if (dodgeDirection == Direction.Forward)
            {
                move = camForward;
            }
            else if (dodgeDirection == Direction.Back)
            {
                move = -camForward;
            }
            else if (dodgeDirection == Direction.Left)
            {
                move = -cam.right;
            }
            else if (dodgeDirection == Direction.Right)
            {
                move = cam.right;
            }
            else if (dodgeDirection == Direction.FR)
            {
                move = camForward + cam.right;
            }
            else if (dodgeDirection == Direction.FL)
            {
                move = camForward - cam.right;
            }
            else if (dodgeDirection == Direction.BR)
            {
                move = -camForward + cam.right;
            }
            else
            {
                move = -camForward - cam.right;
            }
            character.Dodge(move);
        }

        if (!dodge)
        {
            character.Move(move, jump);
        }

        jump = false;
        dodge = false;
    }
}
