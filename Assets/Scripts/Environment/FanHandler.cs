/*
*   Adds constant force to player as they stay within trigger. Could be cool for various obstacles.
*   Can add things like getting hurt if landing on fan (because spinning blades) and such.
*
*   Version 1.0: Fans work, distance modifier only works for upright fans.
*/

using UnityEngine;
using System.Collections;

public class FanHandler : MonoBehaviour {

    public float fanPower = 2500.0f; //~2500.0f is roughly equilibrium hovering around the middle of the trigger for "isVertical", use 1250.0f for similar result with non vertical.
    public bool isVertical; //ONLY SET THIS TO ON IF FACING UPWARDS PERFECTLY OR VERY CLOSE TO SO

    private BoxCollider col;
    private Vector3 fanPosition;
    private Vector3 fanDirection;

	void Start () {
        col = GetComponent<BoxCollider>();
        fanPosition = transform.parent.gameObject.transform.position;
        fanDirection = (transform.position - fanPosition).normalized;
	}
	
	void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy"))
        {  
            if (isVertical)
            {
                //Direction * Power * % modifier based on distance from fan versus edge of trigger (100% is standing on fan, 0% is extreme top edge of trigger)
                //ONLY WORKS FOR COMPLETELY VERTICAL FANS
                other.gameObject.GetComponent<Rigidbody>().AddForce(fanDirection * fanPower * ((transform.localScale.y - other.transform.position.y - fanPosition.y) / transform.localScale.y) * Time.fixedDeltaTime);
            }
            else
            {
                //Boring distance ignoring version because I'm currently too tired at 4:47AM on 12/19/2015 to figure out the 3d math to calculate distance in arbitrary directions
                other.gameObject.GetComponent<Rigidbody>().AddForce(fanDirection * fanPower * Time.fixedDeltaTime);
            }
        }
    }
}
