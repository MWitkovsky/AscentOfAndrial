using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

//lol spectral HANDler get it?
public class SpectralHandler : MonoBehaviour {

    public ThirdPersonCharacter player;
    public Transform characterModel;
    public float speed;
    public float range;

    private Vector3 direction;
    private float distanceTravelled;
    private bool hooked;

    void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            if (hooked)
            {
                player.SetHoming(false, true);
                player.deleteHand(this);
                Destroy(gameObject);
            }
        }
    }

    void FixedUpdate() {
        if (!hooked)
        {
            if (distanceTravelled < range)
            {
                transform.Translate(direction * speed * Time.fixedDeltaTime, Space.World);
                distanceTravelled += speed * Time.fixedDeltaTime;
            }
            else
            {
                player.deleteHand(this);
                Destroy(gameObject);
            }
        }
        else
        {
            if (player.IsHoming()) //hand grabbed a hook
            {
                if (Vector3.Distance(player.transform.position, transform.position) <= 3.0f)
                {
                    player.SetHoming(false, true);
                    characterModel.LookAt(new Vector3(transform.position.x + direction.x, characterModel.position.y, transform.position.z + direction.z));
                    player.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(player.gameObject.GetComponent<Rigidbody>().velocity.x, player.groundJumpPower, player.gameObject.GetComponent<Rigidbody>().velocity.z);

                    player.deleteHand(this);
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
            player.destroyOtherHand(this);

            direction = (transform.position - player.transform.position).normalized;
            direction *= player.moveSpeedMultiplier * 2.0f;

            player.SetHoming(true, true);
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
        transform.LookAt(transform.position + direction);
        direction = transform.forward;
    }
}
