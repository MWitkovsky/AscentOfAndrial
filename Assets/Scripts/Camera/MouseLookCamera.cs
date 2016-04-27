using UnityEngine;
using System.Collections;
using UnityStandardAssets.Cameras;

public class MouseLookCamera : MonoBehaviour
{
    public ThirdPersonCharacter player;
    public Transform target;
    public Transform aimTarget;
    public float xSpeed;
    public float ySpeed;
    public float yMinLimit;
    public float yMaxLimit;
    public float minDistance;
    public float maxDistance;

    private ProtectCameraFromWallClip wallCheck;
    private Rigidbody playerRb;
    private Vector3 distanceVector;
    private float x, y;
    private float distance, aimDistance, velModifier, finalDistance, finalAimDistance, blockedDistance;
    private bool aiming;


    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        distance = (minDistance + maxDistance) / 2.0f;
        finalDistance = distance;

        aimDistance = 0.0f;

        distanceVector.y = -0.5f;

        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().freezeRotation = true;
        }

        wallCheck = GetComponent<ProtectCameraFromWallClip>();
        playerRb = player.GetComponent<Rigidbody>();
    }

    void Update()
    {
        aiming = Input.GetButton("Fire2");
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

            //Aim mode when right click held
            if (aiming)
            {
                finalDistance = Mathf.Lerp(finalDistance, aimDistance, 2.0f * Time.deltaTime);
                Debug.Log(finalDistance);
                distanceVector.z = -(finalDistance);
                Vector3 position = rotation * distanceVector + aimTarget.position;
                transform.position = position;
            }
            else
            {
                //Zoom
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

                velModifier = playerRb.velocity.magnitude * 0.333f;

                if (!wallCheck.HitSomething())
                {
                    if (distance + velModifier < finalDistance)
                    {
                        finalDistance = Mathf.Lerp(finalDistance, distance + velModifier, 2.0f * Time.deltaTime);
                    }
                    else
                    {
                        finalDistance = Mathf.Lerp(finalDistance, distance + velModifier, 0.5f * Time.deltaTime);
                    }

                    distanceVector.z = -(finalDistance);
                    Vector3 position = rotation * distanceVector + target.position;
                    transform.position = position;
                }
                /*else
                {
                    if (distance + velModifier < finalDistance)
                    {
                        finalDistance = Mathf.Lerp(finalDistance, distance + velModifier, 2.0f * Time.deltaTime);
                    }
                    else
                    {
                        finalDistance = Mathf.Lerp(finalDistance, distance + velModifier, 0.5f * Time.deltaTime);
                    }

                    distanceVector.z = -(finalDistance);
                    Vector3 position = rotation * distanceVector + target.position;
                    transform.position = position;
                }*/
            }
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