using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement")]
    public float moveSpeed = 10;
    private float ver, hor;
    private bool moveRequest;
    private Vector3 movePlayer;

    [Header("Player Rotation")]
    public float rotationSpeed = 90;

    [Header("Player Dash")]
    public float dashSpeed = 50;
    private float startDashTime = 0.1f;
    private float dashTime;
    private Vector3 walkDirection = new Vector3();
    private bool dashRequest;

    [Header("Camera Settings")]
    public float mouseXSensitivity=1;
    public float mouseYSensitivity=1;
    public float controllerXSensitivity=1;
    public float controllerYSensitivity=1;
    public float rotateSpeed=90;
    public float xMinClamp=-30, xMaxClamp=65;
    public float minZoom, maxZoom;

    [Header("Camera")]
    public Transform cam;
    //public GameObject actualCam;
    
    [SerializeField]Vector3 camOffset;
    [SerializeField]float camSpeed;
    private Vector3 camX,camY;
    private float cHor, cVer;
    private bool camRequest;

    [Header("Misc")]
    public LayerMask groundMask;
    public Transform offset;
    public GameObject actualPlayer;
    private RaycastHit hit;


    private void Awake()
    {
        dashTime = startDashTime;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        GroundMovement();
        CheckDirection();
        CameraMovement();
        CameraClamp();
        SetCamPos();
        if (Input.GetButtonDown("Dash"))
        {
            StartCoroutine(DashForward());
        }
    }

    private void FixedUpdate()
    {
        if (camRequest)
        {
            cam.transform.Rotate(camX * Time.deltaTime * rotateSpeed, Space.Self);
            cam.transform.Rotate(camY * Time.deltaTime * rotateSpeed, Space.World);
            camRequest = false;
        }
        if (moveRequest)
        {
            SetCharacterRotation();
            transform.Translate(movePlayer * moveSpeed * Time.fixedDeltaTime);
            moveRequest = false;
        }
        if (dashRequest)
        {
            GetComponent<Rigidbody>().AddForce(actualPlayer.transform.forward * dashSpeed,ForceMode.Impulse);
            StartCoroutine(cam.gameObject.GetComponent<CamShake>().HighScreenShake());
            dashRequest = false;
        }
    }

    public void SetCamPos()
    {
        cam.position = Vector3.Lerp(cam.position, transform.position + camOffset, Time.deltaTime * camSpeed);
    }

    public void SetCharacterRotation()
    {
        Quaternion dirWeWant = Quaternion.LookRotation(walkDirection);
        actualPlayer.transform.rotation = Quaternion.RotateTowards(actualPlayer.transform.rotation, dirWeWant, rotationSpeed);
    }

    public void CheckDirection()
    {
        if(hor != 0 || ver != 0)
        {
            if (hor != 0)
            {
                if (hor > 0)
                {
                    walkDirection += cam.transform.right * hor;
                }
                else
                {
                    walkDirection -= cam.transform.right * -hor;
                }
            }
            if (ver != 0)
            {
                if (ver > 0)
                {
                    walkDirection += cam.transform.forward * ver;
                }
                else
                {
                    walkDirection -= cam.transform.forward * -ver;
                }
            }
        }
        walkDirection.y = 0;
        movePlayer = walkDirection;
    }

    public void GroundMovement()
    {
        ver = Input.GetAxis("Vertical");
        hor = Input.GetAxis("Horizontal");
        movePlayer.x = hor;
        movePlayer.z = ver;
        walkDirection = Vector3.zero;
        moveRequest = true;
    }

    public IEnumerator DashForward()
    {
        while (true)
        {
            dashRequest = true;
            yield return new WaitForSeconds(startDashTime);
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            yield break;
        }
    }

    public void CameraMovement()
    {
        if(Input.GetJoystickNames().Length > 0)
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
        Vector3 newEuler = cam.transform.localEulerAngles;
        float tempClamp = newEuler.x;
        if(newEuler.x > 180)
        {
            tempClamp -= 360;
        }
        tempClamp = Mathf.Clamp(tempClamp, xMinClamp, xMaxClamp);
        newEuler.x = tempClamp;
        cam.transform.eulerAngles = newEuler;
        camRequest = true;
    }
}