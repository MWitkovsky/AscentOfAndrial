using UnityEngine;
using System.Collections;
using UnityStandardAssets.Cameras;

public class ThirdPersonMouseCamera : MonoBehaviour
{
    public Transform target;
    public float xSpeed = 250.0f, ySpeed = 120.0f;
    public float yMinLimit = -50.0f, yMaxLimit = 50.0f;
    public float minDistance = -3.5f, maxDistance = 10.0f;

    private Vector3 distanceVector;
    private float x, y;
    private float distance, finalDistance;


    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        distance = (minDistance + maxDistance) / 2.0f;
        finalDistance = distance;

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
            //Rotation
            x += Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime;
            y -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);
            transform.rotation = rotation;

            if (Input.GetAxis("Mouse ScrollWheel") > 0) //forwards
            {
                if (distance > minDistance)
                {
                    distance -= 0.5f;
                }
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0) // backwards
            {
                if (distance < maxDistance)
                {
                    distance += 0.5f;
                }
            }

            finalDistance = Mathf.Lerp(finalDistance, distance, 2.0f * Time.deltaTime);

            distanceVector.z = -(finalDistance);
            Vector3 position = rotation * distanceVector + target.position;
            transform.position = position;
        }
    }

    private float ClampAngle(float angle, float min, float max)
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
