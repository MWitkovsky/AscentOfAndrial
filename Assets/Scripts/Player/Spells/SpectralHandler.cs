using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

//lol spectral HANDler get it?
public class SpectralHandler : MonoBehaviour {

    public AudioClip shootSound, grabSound;
    public ThirdPersonCharacter player;
    public Transform characterModel;
    public float speed;
    public float range;

    private AudioSource source;
    private Vector3 direction;
    private float distanceTravelled;
    private bool hooked;

    void Start()
    {
        source = GetComponent<AudioSource>();
        source.loop = false;
        PlayShootSound();
    }

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
                if (Vector3.Distance(player.transform.position, transform.position) <= 6.0f)
                {
                    player.SetHoming(false, true);
                    characterModel.LookAt(new Vector3(transform.position.x + direction.x, characterModel.position.y, transform.position.z + direction.z));
                    player.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, player.groundJumpPower, 0.0f);

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
        if (other.gameObject.CompareTag("Hook") && !hooked)
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

            PlayGrabSound();
        }
    }

    private void PlayShootSound()
    {
        source.clip = shootSound;
        source.Play();
    }

    private void PlayGrabSound()
    {
        source.clip = grabSound;
        source.Play();
    }

    public void setDirection(Vector3 direction)
    {
        this.direction = direction;
        transform.LookAt(transform.position + direction);
        direction = transform.forward;
    }
}
