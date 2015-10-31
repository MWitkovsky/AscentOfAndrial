using UnityEngine;
using System.Collections;

public class RailHandler : MonoBehaviour {

    public Transform origin, destination;
    public Vector3 lookAt;

    private ThirdPersonCharacter player;
    private Rigidbody playerRB;
    private Vector3 grindVelocity;
    private bool wasReverse;

    void Start()
    {
        grindVelocity = (destination.position - origin.position).normalized;
    }

	void FixedUpdate () {
        if (playerRB)
        {
            if(player.getJumpTimer() < 0.0f)
            {
                playerRB.velocity = grindVelocity;  
            }            
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")){
            other.GetComponent<ThirdPersonCharacter>().SetGrinding(true);
            player = other.GetComponent<ThirdPersonCharacter>();
            playerRB = other.GetComponent<Rigidbody>();

			float positionOffset = this.transform.position.x - playerRB.transform.position.x;
			float rotationalOffset = this.transform.rotation.y - playerRB.transform.rotation.y;

			Debug.Log(positionOffset);

			
			if(rotationalOffset > 0.2f)
				playerRB.transform.Translate(0.0f, -rotationalOffset, 0.0f);
			
			else if(rotationalOffset < 0.2f)
				playerRB.transform.Translate(0.0f, rotationalOffset, 0.0f);

			Debug.Log(rotationalOffset);
			Debug.Log(playerRB.transform.rotation.y);
			if(Mathf.Abs(rotationalOffset) < 0.01)
			{
				Debug.Log("Center");
				if(positionOffset > 0)
					playerRB.transform.Translate(-positionOffset, 0.0f, 0.0f);

				else if(positionOffset < 0)
					playerRB.transform.Translate(positionOffset, 0.0f, 0.0f);
			}
            if (Vector3.Dot(other.GetComponent<ThirdPersonUserControl>().characterModel.forward.normalized, grindVelocity) >= 0)
            {
                grindVelocity *= player.moveSpeedMultiplier;
                
            }
            else
            {
                grindVelocity *= -player.moveSpeedMultiplier;
                wasReverse = true;
            }

            
            playerRB.useGravity = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<ThirdPersonCharacter>().SetGrinding(false);
            playerRB.useGravity = true;

            if (wasReverse)
            {
                grindVelocity /= -player.moveSpeedMultiplier;
            }
            else
            {
                grindVelocity /= player.moveSpeedMultiplier;
            }
            
            player = null;
            playerRB = null;
        }
    }
}
