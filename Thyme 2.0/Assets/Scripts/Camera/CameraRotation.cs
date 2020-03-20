using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [Header("Camera Settings")]
    public float mouseRotateSpeed = 90;
    public float controllerRotateSpeed = 180;

    [Header("Clamp Settings")]
    public float topClamp = 50;
    public float botClamp = -50;

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
    public Vector3 camX, camY;
    private float cHor, cVer;
    float xAxisClamp;
    float minimumTrigger = 0.1f;
    //float joystickDelay = 0.2f;
    //List<string> connectedJoystick;
    //private void Awake()
    //{
    //    StartCoroutine(CheckConnectedJoystick(joystickDelay));
    //}

    //public IEnumerator CheckConnectedJoystick(float delay)
    //{
    //    while (true)
    //    {
    //        connectedJoystick = new List<string>(Input.GetJoystickNames());
    //        for (int i = 0; i < connectedJoystick.Count; i++)
    //        {
    //            if (connectedJoystick[i] == "")
    //            {
    //                print("Found Empty");
    //                connectedJoystick.Remove(connectedJoystick[i]);
    //                break;
    //            }
    //        }
    //        yield return new WaitForSeconds(delay);
    //    }
    //}

    public bool CheckInput()
    {
        if (Input.GetAxis("RotateHor") < -minimumTrigger || Input.GetAxis("RotateHor") > minimumTrigger || Input.GetAxis("RotateVer") < -minimumTrigger || Input.GetAxis("RotateVer") > minimumTrigger)
        {
            GameManager.instance.controlMode = GameMode.controller;
        }
        if (Input.GetAxis("Mouse X") < -minimumTrigger || Input.GetAxis("Mouse X") > minimumTrigger || Input.GetAxis("Mouse Y") < -minimumTrigger || Input.GetAxis("Mouse Y") > minimumTrigger)
        {
            GameManager.instance.controlMode = GameMode.pc;
        }
        if (GameManager.instance.controlMode == GameMode.controller)
        {
            cHor = Input.GetAxis("RotateHor") * controllerXSensitivity * Time.deltaTime * controllerRotateSpeed;
            cVer = Input.GetAxis("RotateVer") * controllerYSensitivity * Time.deltaTime * controllerRotateSpeed;
            if (controllerXInvert)
            {
                cHor = -cHor;
            }
            if (controllerYInvert)
            {
                cVer = -cVer;
            }
            return true;
        }
        else
        {
            cHor = Input.GetAxis("Mouse X") * mouseXSensitivity * Time.deltaTime * mouseRotateSpeed;
            cVer = Input.GetAxis("Mouse Y") * mouseYSensitivity * Time.deltaTime * mouseRotateSpeed; 
            if (mouseXInvert)
            {
                cHor = -cHor;
            }
            if (mouseYInvert)
            {
                cVer = -cVer;
            }
            return false;
        }
    }

    public void CameraMovement()
    {
        xAxisClamp += cVer;

        camHolder.transform.Rotate(Vector3.up * cHor, Space.World);

        CameraClamp(cVer);

        camHolder.transform.Rotate(Vector3.left * cVer);
    }

    public void CameraClamp(float ver)
    {
        if (xAxisClamp > -botClamp)
        {
            xAxisClamp = -botClamp;
            cVer = 0f;
            ClampXAxisRotationToValue(botClamp);
        }
        else if (xAxisClamp < -topClamp)
        {
            xAxisClamp = -topClamp;
            cVer = 0f;
            ClampXAxisRotationToValue(topClamp);
        }
    }

    private void ClampXAxisRotationToValue(float value)
    {
        Vector3 eulerRotation = camHolder.transform.localEulerAngles;
        eulerRotation.x = value;
        camHolder.transform.localEulerAngles = eulerRotation;
    }
}
