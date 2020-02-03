using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [Header("Camera Settings")]
    public float rotateSpeed = 90;
    public float xMinClamp = -50, xMaxClamp = 50;
    //public float minZoom, maxZoom;

    [Header("Mouse Sensitivity Settings")]
    public float mouseXSensitivity = 1;
    public float mouseYSensitivity = 1;
    public bool mouseXInvert;
    public bool mouseYInvert;

    [Header("Controller Sensitivity Settings")]
    public float controllerXSensitivity = 1;
    public float controllerYSensitivity = 1;
    public bool controllerXInvert = true;
    public bool controllerYInvert;

    [Header("Misc")]
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
            if (controllerXInvert)
            {
                cHor = -cHor;
            }
            if (controllerYInvert)
            {
                cVer = -cVer;
            }
        }
        else
        {
            cHor = Input.GetAxis("Mouse X") * mouseXSensitivity;
            cVer = Input.GetAxis("Mouse Y") * mouseYSensitivity;
            if (mouseXInvert)
            {
                cHor = -cHor;
            }
            if (mouseYInvert)
            {
                cVer = -cVer;
            }
        }
        camY.y = cHor;
        camX.x = cVer;
        camY.z = 0;
        camX.z = 0;
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
