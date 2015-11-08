using UnityEngine;
using System.Collections;

//lol spectral HANDler get it?
[RequireComponent(typeof(SphereCollider))]
public class SpectralHandler : MonoBehaviour {

    public ThirdPersonCharacter player;
    public Transform characterModel;
    public float speed;
    public float range;

    private Vector3 direction;
    private float distanceTravelled;
    private bool hooked;

    void FixedUpdate() {
        if (!hooked)
        {
            if (distanceTravelled < range)
            {
                transform.Translate(direction * speed * Time.fixedDeltaTime);
                distanceTravelled += speed * Time.fixedDeltaTime;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (player.IsHoming()) //hand grabbed a hook
            {
                if (Vector3.Distance(player.transform.position, transform.position) <= 3.0f)
                {
                    player.SetHoming(false);
                    characterModel.LookAt(new Vector3(transform.position.x + direction.x, characterModel.position.y, transform.position.z + direction.z));

                    Destroy(gameObject);
                }
                else
                {
                    player.GetComponent<Rigidbody>().velocity = direction;
                }
            }
        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hook"))
        {
            transform.position = other.transform.position;

            hooked = true;

            direction = (transform.position - player.transform.position).normalized;
            direction *= player.moveSpeedMultiplier * 2.0f;

            player.SetHoming(true);
            characterModel.transform.LookAt(transform.position);

            player.GetComponent<Rigidbody>().useGravity = false;

            player.resetAirJumps();
            player.resetCastTimer();

            //Change hand graphic to grabbing
        }
    }

    public void setDirection(Vector3 direction)
    {
        this.direction = direction;
    }
}
