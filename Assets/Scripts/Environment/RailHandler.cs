﻿using UnityEngine;
using System.Collections;

public class RailHandler : MonoBehaviour {

    public Transform origin, destination;
    private Vector3 lookAt;

    private ThirdPersonCharacter player;
    private Rigidbody playerRB;
    private Vector3 grindVelocity, originalGrindVelocity;
    private bool wasReverse;

	private Vector3 beginTemp;
	private Vector3 endTemp;
	private Vector3 mid;

    void Start()
    {
        grindVelocity = (destination.position - origin.position).normalized;
        originalGrindVelocity = grindVelocity;
    }

	void FixedUpdate () {
        if (playerRB)
        {
            if(player.GetJumpTimer() < 0.0f)
            {
                playerRB.velocity = grindVelocity;

                /*if (grindVelocity.y > 0.0f)
                {
                    if (wasReverse)
                    {
                        grindVelocity += originalVelocity * originalNormalized.y;
                    }
                    else
                    {
                        grindVelocity -= originalVelocity * originalNormalized.y;
                    }
                }
                else
                {
                    if (wasReverse)
                    {
                        grindVelocity -= originalVelocity * originalNormalized.y; 
                    }
                    else
                    {
                        grindVelocity -= originalVelocity * originalNormalized.y;
                    }
                }*/
            }            
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")){
            other.GetComponent<ThirdPersonCharacter>().SetGrinding(true);
			other.GetComponentInChildren<ParticleSystem>().Play();
            player = other.GetComponent<ThirdPersonCharacter>();
            playerRB = other.GetComponent<Rigidbody>();

            Transform characterModel = other.GetComponent<ThirdPersonUserControl>().characterModel;

            //elegant method.
            BinarySearch(playerRB.transform.position);

            if (Vector3.Dot(characterModel.forward.normalized, grindVelocity) >= 0)
            {
                grindVelocity *= player.moveSpeedMultiplier;
            }
            else
            {
                grindVelocity *= -player.moveSpeedMultiplier;
                wasReverse = true;
            }
        
            if(Mathf.Abs(playerRB.velocity.y / 2.0f) > 1.0f)
            {
                if((Mathf.Abs(playerRB.velocity.y / 2.0f)) > 3.5f)
                {
                    grindVelocity *= 3.5f;
                }
                else
                {
                    grindVelocity *= Mathf.Abs(playerRB.velocity.y / 2.0f);
                }
            }

            if ((playerRB.velocity.magnitude - 44.9f) > 0.0f)
            {
                grindVelocity *= 2.0f;
            }

            characterModel.LookAt(characterModel.position + grindVelocity);

            playerRB.useGravity = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<ThirdPersonCharacter>().SetGrinding(false);
			other.GetComponentInChildren<ParticleSystem>().Stop();
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

        wasReverse = false;
        grindVelocity = originalGrindVelocity;
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
