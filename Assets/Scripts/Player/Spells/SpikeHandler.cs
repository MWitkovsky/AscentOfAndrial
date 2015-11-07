using UnityEngine;
using System.Collections;

public class SpikeHandler : MonoBehaviour {

    public float riseSpeed;
    public float stayTime;

    private Vector3 translation;
    private float distanceRisen;
    private float distanceToRise;

    void Start()
    {
        translation = new Vector3(0.0f, riseSpeed, 0.0f);
        distanceToRise = transform.localScale.y * 2.0f;
    }
	
	void FixedUpdate () {
        if(stayTime > 0.0f)
        {
            if (distanceRisen < distanceToRise)
            {
                transform.Translate(translation * Time.fixedDeltaTime);
                distanceRisen += riseSpeed * Time.fixedDeltaTime;
            }
            else
            {
                stayTime -= Time.fixedDeltaTime;
            }
        }
        else
        {
            if (distanceRisen > 0.0f)
            {
                transform.Translate(-(translation * Time.fixedDeltaTime));
                distanceRisen -= riseSpeed * Time.fixedDeltaTime;
            }
            else
            {
                //spike is destroyed when fully underground again
                Destroy(gameObject);
            }
        }
	}

    void OnCollisionEnter(Collision col)
    {
        /*if (col.collider.gameObject.CompareTag("Enemy"))
        {
            col.collider.gameObject.GetComponent<EnemyHandler>().Kill();
        }
        */
    }
}
