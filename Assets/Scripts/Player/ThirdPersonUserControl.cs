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
    private Direction dodgeDirection;

    enum Direction {Forward, Back, Left, Right, FR, FL, BR, BL};

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
	
	private void Update () {
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
                    }
                }
                else if (Input.GetKey(KeyCode.A)) //Left
                {
                    dodge = true;
                    dodgeDirection = Direction.Left;                    
                }
                else if (Input.GetKey(KeyCode.D)) //Right
                {
                    dodge = true;
                    dodgeDirection = Direction.Right;
                    
                }
            }
        }
    }

    private void FixedUpdate()
    {
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

        //Set player's rotation if moving
        if (h != 0.0f || v != 0.0f)
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
