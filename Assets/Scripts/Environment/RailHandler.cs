using UnityEngine;
using System.Collections;

public class RailHandler : MonoBehaviour {

    public Transform origin, destination;
    public Vector3 lookAt;

    private ThirdPersonCharacter player;
    private Rigidbody playerRB;
    private Vector3 grindVelocity;
    private bool wasReverse;

	private Vector3 beginTemp;
	private Vector3 endTemp;

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


			//Semi-elegant method.
			//BinarySearch(playerRB.transform.position);

			/* The brute-force method.
			float rotationalOffset = this.transform.rotation.y - playerRB.transform.rotation.y;


			Debug.Log("Global: " + this.GetComponent<BoxCollider>().transform.position.x);
			Debug.Log("Local: " + this.GetComponent<BoxCollider>().transform.localPosition.x);

			Debug.Log("Global Player: " + playerRB.transform.position.x);
			Debug.Log("Local Player: " + playerRB.transform.localPosition.x);

			if(rotationalOffset != 0)
				playerRB.transform.rotation.Set(playerRB.transform.rotation.x, this.transform.rotation.y, playerRB.transform.rotation.z, playerRB.transform.rotation.w); 
	

				if(positionOffset != 0)
				{
					float correctedX = positionOffset > 0 ? this.GetComponent<BoxCollider>().transform.position.x - 0.5f : this.GetComponent<BoxCollider>().transform.position.x + 0.5f;
					playerRB.transform.position.Set(correctedX, playerRB.transform.position.y, playerRB.transform.position.z);
				}
*/
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

	void BinarySearch(Vector3 position)
	{
		bool match = false;

		beginTemp = origin.position;
		endTemp = destination.position;
		float positionOffset = 1.0f;
		if(!match)
		{
			Vector3 mid = new Vector3((beginTemp.x + endTemp.x)/2 ,0.0f, (beginTemp.z + endTemp.z)/2);
			if((position.x >= mid.x - positionOffset && position.x <=  mid.x + positionOffset) 
			   && (position.z >= mid.z - positionOffset && position.x <=  mid.z + positionOffset))
			{
				match = true; //In the words of a Witkovsky, "Snap!!"
				Debug.Log(match);
			}

			else
			{
				if((position.x >= mid.x - positionOffset && position.x <=  endTemp.x + positionOffset) 
					&& (position.z >= mid.z - positionOffset && position.x <=  endTemp.z + positionOffset))
				{
					beginTemp = mid;
				}

				else
					endTemp = mid;
			}


		}
				



	}
}
