using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    private float maxUpAngle = 70;
    private float maxDownAngle = 70;

    private float speed = 5f;

    private float maxZoom = 2;
    private float minZoom = 10;

    private float currentZoom;
    private float destZoom;

    private Vector2 angleDelta;

    [SerializeField]
    private Transform cameraTransform;

    void Start()
    {
        currentZoom = minZoom;
    }

    void Update()
    {
        angleDelta.y = -Input.GetAxis("Horizontal");
        angleDelta.x = Input.GetAxis("Vertical");

        transform.Rotate(angleDelta);
        Quaternion q = transform.rotation;
        Vector3 cappedRot = new Vector3(q.eulerAngles.x, q.eulerAngles.y, 0);

        if (cappedRot.x > maxUpAngle && cappedRot.x <= 180)
        {
            cappedRot.x = maxUpAngle;
        } else if (cappedRot.x < 360 - maxDownAngle && cappedRot.x >= 180) {
            cappedRot.x = 360 - maxDownAngle;
        }
        q.eulerAngles = cappedRot;
        transform.rotation = q;

    }
}
