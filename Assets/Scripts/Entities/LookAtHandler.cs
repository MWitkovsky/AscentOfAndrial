using UnityEngine;
using System.Collections;

/*
*   This class makes it so if the player enters within the sphere's radius, the enemy will turn to stare at them.
*/

[RequireComponent(typeof(SphereCollider))]
public class LookAtHandler : MonoBehaviour {

    public Transform parent;

	void Start () {
        //lock this to true in case some jerk forgets to set this as a trigger
        GetComponent<SphereCollider>().isTrigger = true;
	}
	
	void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            parent.LookAt(new Vector3(other.transform.position.x, parent.transform.position.y, other.transform.position.z));
        }
    }
}
