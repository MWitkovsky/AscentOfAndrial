using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {

	void OnCollisionEnter(Collision col){
	    if (col.collider.CompareTag ("Player")) {

		}
	}
}
