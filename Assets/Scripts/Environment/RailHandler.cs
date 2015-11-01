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
	private Vector3 mid;

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
			BinarySearch(playerRB.transform.position);

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
		int count = 0;
		while(!match)
		{
			//safeguard so game doesn't crash in some strange fringe case
			if(count > 50){
				break;
			}
			Vector3 mid = new Vector3((beginTemp.x + endTemp.x)/2.0f , (beginTemp.y + endTemp.y)/2.0f, (beginTemp.z + endTemp.z)/2.0f);

			if(isBetween(position.x, mid.x - positionOffset, mid.x + positionOffset)
			   && isBetween(position.z, mid.z - positionOffset, mid.z + positionOffset))
			{
				match = true; //In the words of a Witkovsky, "Snap!!"
				//1.6f is half andrial's height
				playerRB.gameObject.transform.position = new Vector3(mid.x, mid.y+1.6f, mid.z);
			}
			else
			{
				if(isBetween(position.x, mid.x, endTemp.x)
				   && isBetween(position.z, mid.z, endTemp.z))
				{
					beginTemp = mid;
				}
				else
				{
					endTemp = mid;
				}
			}
			count++;
		}
	}

	//Checks if a is between b and c
	bool isBetween(float a, float b, float c)
	{
		if (b < c) {
			if (a >= b && a <= c) 
			{
				return true;
			}
		}
		else
		{
			if (a >= c && a <= b)
			{
				return true;
			}
		}

		//strange bugs happened because of floating point errors, this is a catch
		//this also fixes the logical bug of a rail perfectly on the X or Z axis
		if (b >= c-0.1f && b <= c+0.1f) 
		{
			return true;
		}

		return false;
	}
}
