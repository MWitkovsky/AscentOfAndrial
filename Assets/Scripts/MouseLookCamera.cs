using UnityEngine;
using System.Collections;

public class MouseLookCamera : MonoBehaviour
{
    public Transform target;
    public float xSpeed;
    public float ySpeed;
    public float yMinLimit;
    public float yMaxLimit;
    public float minDistance;
    public float maxDistance;

    private Vector3 distanceVector;
    private float x, y;
    private float distance;


    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        distance = (minDistance + maxDistance) / 2.0f;

        distanceVector.y = -0.5f;

        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().freezeRotation = true;
        }
    }

    void LateUpdate()
    {
        if (target)
        {
            //Zoom
            if (Input.GetAxis("Mouse ScrollWheel") > 0) //forwards
            {
                if(distance > -3.5f)
                {
                    distance -= 0.5f;
                }
                
            }
            else if(Input.GetAxis("Mouse ScrollWheel") < 0) // backwards
            {
                if(distance < maxDistance)
                {
                    distance += 0.5f;
                }
            }

            //Rotation
            x += Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime;
            y -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            distanceVector.z = -distance;

            Quaternion rotation = Quaternion.Euler(y, x, 0);
            Vector3 position = rotation * distanceVector + target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
    }

    private float ClampAngle (float angle, float min, float max)
    {
        while (angle < -360)
        {
            angle += 360;
        }
        while (angle > 360)
        {
            angle -= 360;
        }

        return Mathf.Clamp(angle, min, max);
    }
}