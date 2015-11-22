using UnityEngine;
using System.Collections;

public class KillPlaneHandler : MonoBehaviour {

	void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")){
            Debug.Log("HIT");
            ThirdPersonCharacter player = other.gameObject.GetComponent<ThirdPersonCharacter>();
            player.KillImmediately();
        }
    }
}
