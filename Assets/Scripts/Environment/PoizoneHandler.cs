using UnityEngine;
using System.Collections;

public class PoizoneHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerStay(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
        {
			ThirdPersonCharacter player = other.gameObject.GetComponent<ThirdPersonCharacter>();
            if (player.isVulnerable())
            {
                player.healthBar.applyDamage(1);
               // player.Hit(-other.gameObject.GetComponent<ThirdPersonUserControl>().characterModel.transform.forward); //lol
            }
        }
	}
}
