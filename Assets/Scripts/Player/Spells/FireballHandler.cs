using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
public class FireballHandler : MonoBehaviour {

    public AudioClip shootSound, explosionSound;
    [Range(1f, 4f)]public float gravityMultiplier = 1.0f;
    public float launchForce;

    private AudioSource source;
    private Rigidbody rb;
    private SphereCollider sphereCollider;
    private Vector3 force;
    private int frameCounter;
    private bool explode;
    private bool launch;

    void Start()
    {
        source = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();

        source.loop = false;
    }

    void FixedUpdate () {
        if (gravityMultiplier > 1.0f)
        {
            Vector3 extraGravityForce = (Physics.gravity * gravityMultiplier) - Physics.gravity;
            rb.AddForce(extraGravityForce);
        }

        if (launch)
        {
            rb.velocity = force;
            PlayShootSound();

            launch = false;
        }

        if (explode)
        {
            if(!source.isPlaying)
            {
                sphereCollider.enabled = false;
                //This will actually be deleted when the fire particles die, but for now...
                Destroy(gameObject);
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        Explode();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Explode();

            other.GetComponent<EnemyHandler>().Kill();
            other.GetComponent<Rigidbody>().AddExplosionForce(1400.0f, transform.position, sphereCollider.radius);
        }
    }

    public void Launch(Vector3 direction)
    {
        launch = true;
        force = direction * launchForce;
    }

    private void Explode()
    {
        explode = true;

        rb.isKinematic = true;
        rb.useGravity = false;

        sphereCollider.isTrigger = true;
        sphereCollider.radius = 7.0f;

        PlayExplosionSound();
    }

    private void PlayShootSound()
    {
        source.clip = shootSound;
        source.Play();
    }

    private void PlayExplosionSound()
    {
        source.clip = explosionSound;
        source.Play();
    }
}
