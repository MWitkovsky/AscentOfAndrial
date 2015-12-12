using UnityEngine;
using System.Collections;

public class MovingPlatformHandler : MonoBehaviour {
	
	public Transform platform;
	public Transform position1;
	public Transform position2;
	public float speed;
	public float resetTime;
    public bool acceleratedMovement;

	private Vector3 targetPosition, initialPosition;
	private string currentState;
    private float timer;
    private float lerpCounter;

    private enum MovingTo {toPosition1, toPosition2}
    private MovingTo target;

	void Start() {
        target = MovingTo.toPosition1;
        platform.position = position1.position;
        initialPosition = position1.position;
        timer = resetTime;

        ChangeTarget();
	}

	void FixedUpdate() {
        if (acceleratedMovement)
        {
            platform.position = Vector3.Lerp(platform.position, targetPosition, speed * Time.fixedDeltaTime);
        }
        else
        {
            lerpCounter += speed * Time.fixedDeltaTime;
            if(lerpCounter >= 1.0f)
            {
                lerpCounter = 1.0f;
            }
            platform.position = Vector3.Lerp(initialPosition, targetPosition, lerpCounter);
        }
		
        if(Vector3.Distance(platform.position, targetPosition) < 0.2f)
        {
            timer -= Time.fixedDeltaTime;
            if (timer < 0.0f)
            {
                ChangeTarget();
                timer = resetTime;
            }
        }
	}

	void ChangeTarget(){
		if (target == MovingTo.toPosition1) {
			target = MovingTo.toPosition2;
            initialPosition = position1.position;
			targetPosition = position2.position;
		}
        else {
            target = MovingTo.toPosition1;
            initialPosition = position2.position;
            targetPosition = position1.position;
		}

        lerpCounter = 0.0f;
	}

	//draws debug stuff on the scene
	void OnDrawGizmos(){
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube (position1.position, platform.transform.localScale);

		Gizmos.color = Color.red;
		Gizmos.DrawWireCube (position2.position, platform.transform.localScale);
	}
}
