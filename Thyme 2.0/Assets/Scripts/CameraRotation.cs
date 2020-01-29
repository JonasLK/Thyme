using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [Header("Camera Settings")]
    public float rotateSpeed = 90;
    public float xMinClamp = -30, xMaxClamp = 65;
    //public float minZoom, maxZoom;

    [Header("Camera Sensitivity")]
    public float mouseXSensitivity = 1;
    public float mouseYSensitivity = 1;
    public float controllerXSensitivity = 1;
    public float controllerYSensitivity = 1;

    [Header("Misc")]
    public float range;
    public LayerMask interactable;
    public GameObject camHolder;
    private Vector3 camX, camY;
    private float cHor, cVer;
    private bool camRequest;

    public void Update()
    {
        CameraMovement();
        CameraClamp();
    }
    private void FixedUpdate()
    {
        if (camRequest)
        {
            camHolder.transform.Rotate(camX * Time.deltaTime * rotateSpeed, Space.Self);
            camHolder.transform.Rotate(camY * Time.deltaTime * rotateSpeed, Space.World);
            camRequest = false;
        }
    }

    public void CameraMovement()
    {
        if (Input.GetJoystickNames().Length > 0)
        {
            cHor = Input.GetAxis("RotateHor") * controllerXSensitivity;
            cVer = Input.GetAxis("RotateVer") * controllerYSensitivity;
        }
        else
        {
            cHor = Input.GetAxis("Mouse X") * mouseXSensitivity;
            cVer = Input.GetAxis("Mouse Y") * mouseYSensitivity;
        }
        camY.y = -cHor;
        camX.x = cVer;
    }

    public void CameraClamp()
    {
        Vector3 newEuler = camHolder.transform.localEulerAngles;
        float tempClamp = newEuler.x;
        if (newEuler.x > 180)
        {
            tempClamp -= 360;
        }
        tempClamp = Mathf.Clamp(tempClamp, xMinClamp, xMaxClamp);
        newEuler.x = tempClamp;
        camHolder.transform.eulerAngles = newEuler;
        camRequest = true;
    }
}
