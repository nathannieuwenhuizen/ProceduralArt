using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    private float maxUpAngle = 70;
    private float maxDownAngle = 70;

    [SerializeField]
    private float speed = 5f;

    private float minZoom = 10;
    private float maxZoom = 40;

    [SerializeField]
    private float currentZoom;
    [SerializeField]
    private float destZoom;
    [SerializeField]
    private float zoomSpeed = 5f;

    private Vector2 angleDelta;

    [SerializeField]
    private Transform cameraTransform;

    void Start()
    {
        destZoom = currentZoom = minZoom + (maxZoom - minZoom) * 0.5f;
    }

    void Update()
    {
        Debug.Log("scroll " + Input.GetAxis("Mouse ScrollWheel"));
        destZoom += Input.GetAxis("Mouse ScrollWheel") * -20f;
        destZoom = Globals.Cap(destZoom, minZoom, maxZoom);
        currentZoom = Mathf.Lerp(currentZoom, destZoom, Time.deltaTime * zoomSpeed);

        Vector3 delta = cameraTransform.position - transform.position;
        delta.Normalize();
        delta *= currentZoom;
        cameraTransform.position = transform.position + delta;


        angleDelta.y = -Input.GetAxis("Horizontal");
        angleDelta.x = Input.GetAxis("Vertical");

        transform.Rotate(angleDelta * speed);
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
