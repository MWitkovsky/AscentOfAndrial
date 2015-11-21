using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class EnemyHandler : MonoBehaviour {

    public SphereCollider lookAtBoundary;
    public float wobbleIntensity, wobbleSpeed;

    new private Collider collider;
    private Rigidbody rb;
    private Vector3 originalPosition;
    private bool wobbleUp;

	void Start () {
        originalPosition = transform.position;
        transform.position = new Vector3(transform.position.x, Random.Range(transform.position.y - wobbleIntensity, transform.position.y + wobbleIntensity), transform.position.z);
        collider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate () {
        //Wobbling up and down
	    if(transform.position.y >= originalPosition.y + wobbleIntensity)
        {
            wobbleUp = false;
        }
        else if (transform.position.y <= originalPosition.y - wobbleIntensity)
        {
            wobbleUp = true;
        }

        if (wobbleUp)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + (wobbleSpeed * Time.fixedDeltaTime), transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - (wobbleSpeed * Time.fixedDeltaTime), transform.position.z);
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Spell"))
        {
            Kill();
        }
        if (other.gameObject.CompareTag("Player"))
        {
            ThirdPersonCharacter player = other.gameObject.GetComponent<ThirdPersonCharacter>();
            if (!player.IsHoming() && player.isVulnerable())
            {
                player.healthBar.applyDamage(20.0f);
                player.Hit(-other.gameObject.GetComponent<ThirdPersonUserControl>().characterModel.transform.forward); //lol
            }
        }
    }

    //Kills the enemy, causing physics to start
    public void Kill()
    {
        lookAtBoundary.enabled = false;
        collider.isTrigger = false;
        rb.useGravity = true;
        rb.isKinematic = false;
        gameObject.layer = LayerMask.NameToLayer("Corpses");
    }
}
