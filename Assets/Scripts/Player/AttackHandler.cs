using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class AttackHandler : MonoBehaviour {

    public ThirdPersonCharacter player;
    public Transform characterModel;

    private EnemyHandler enemy;
    private SphereCollider attackRadius;
    private Vector3 direction;
    private Vector3 destination;
    private int frameCounter;

	// Use this for initialization
	void Start () {
        attackRadius = GetComponent<SphereCollider>();
        frameCounter = 0;
	}
	
	void FixedUpdate () {
        if (attackRadius.enabled)
        {
            frameCounter++;
            if (frameCounter > 3)
            {
                attackRadius.enabled = false;
                frameCounter = 0;
            }
        }

        if (player.IsHoming())
        {
            if(Vector3.Distance(player.transform.position, destination) <= 1.0f)
            {
                player.SetHoming(false);
                characterModel.LookAt(new Vector3(destination.x + direction.x, characterModel.position.y, destination.z + direction.z));
                player.GetComponent<Rigidbody>().velocity = new Vector3(player.GetComponent<Rigidbody>().velocity.x, player.groundJumpPower * 1.1f, player.GetComponent<Rigidbody>().velocity.z);
                //player.getAnimator().SetTrigger("AirAttackSwipe");

                enemy.GetComponent<Rigidbody>().velocity = new Vector3(player.GetComponent<Rigidbody>().velocity.x, -player.groundJumpPower * 4.0f, player.GetComponent<Rigidbody>().velocity.z);

                if (enemy)
                {
                    enemy.Kill();
                    enemy = null;
                }
            }
            else
            {
                player.GetComponent<Rigidbody>().velocity = direction;
            }
        }
	}

    void OnTriggerEnter (Collider other)
    {
        if (!enemy)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                //Non-obvious check: isTrigger is false when enemy is dead
                if (other.GetComponent<Collider>().isTrigger)
                {
                    if (Vector3.Dot(other.transform.forward.normalized, characterModel.forward.normalized) <= 0.0f)
                    {
                        enemy = other.GetComponent<EnemyHandler>();
                        destination = other.transform.position;
                        direction = (other.transform.position - player.transform.position).normalized;
                        direction *= player.moveSpeedMultiplier * 2.0f;

                        player.SetHoming(true);
                        characterModel.transform.LookAt(other.transform.position);

                        //player.GetComponent<Rigidbody>().useGravity = false;

                        attackRadius.enabled = false;
                        frameCounter = 0;
                        return; //just want the first valid enemy we get
                    }
                    else
                    {
                        enemy = null;
                    }
                }
            }
        }
    }
}
