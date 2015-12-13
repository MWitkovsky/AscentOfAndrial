using UnityEngine;
using System.Collections;

public class KillPlaneHandler : MonoBehaviour {

	void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")){
            ThirdPersonCharacter player = other.gameObject.GetComponent<ThirdPersonCharacter>();
            player.KillImmediately();
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Spell"))
        {
            Destroy(other.gameObject);
        }
    }
}
