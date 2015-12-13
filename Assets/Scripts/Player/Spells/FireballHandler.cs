using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
public class FireballHandler : MonoBehaviour {

    [Range(1f, 4f)]public float gravityMultiplier = 1.0f;
    public float launchForce;

    private Rigidbody rb;
    private SphereCollider sphereCollider;
    private Vector3 force;
    private int frameCounter;
    private bool explode;
    private bool launch;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
        frameCounter = 3;
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

            launch = false;
        }

        if (explode)
        {
            frameCounter--;
            if(frameCounter < 0)
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
    }
}
