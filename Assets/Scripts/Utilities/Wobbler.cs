using UnityEngine;
using System.Collections;

public class Wobbler : MonoBehaviour {

    public float wobbleIntensity, wobbleSpeed;

    private Vector3 originalPosition;
    private bool wobbleUp;

    void Start () {
        originalPosition = transform.position;
        transform.position = new Vector3(transform.position.x, Random.Range(transform.position.y - wobbleIntensity, transform.position.y + wobbleIntensity), transform.position.z);
    }
	
	void FixedUpdate () {
        if (transform.position.y >= originalPosition.y + wobbleIntensity)
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
}
