using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

    public Vector3 rotationVector;

    void Start()
    {
        transform.Rotate(rotationVector * Random.Range(0.0f, 120.0f));
    }

	void FixedUpdate () 
	{
		transform.Rotate (rotationVector * Time.fixedDeltaTime);
	}
}