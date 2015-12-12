using UnityEngine;
using System.Collections;

public class AnchorObject : MonoBehaviour {

	private Vector3 currentPosition;
	private Vector3 lastPosition;

	void FixedUpdate(){
		lastPosition = currentPosition;
		currentPosition = transform.position;
	}

	void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Player"))
        {
            other.transform.parent = gameObject.transform;
        }
	}

	void OnTriggerExit(Collider other){
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.parent = null;
        }
	}
}
