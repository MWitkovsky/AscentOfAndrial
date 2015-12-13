using UnityEngine;
using System.Collections;

public class AnchorObject : MonoBehaviour {

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
