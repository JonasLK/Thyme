﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] float moveSpeed = 10;
    [SerializeField] float hangSpeed = 5;
    private float ver, hor;
    private bool moveRequest;
    private Vector3 movePlayer;

    [Header("Player Rotation")]
    [SerializeField] float rotationSpeed = 90;

    [Header("Player Dash")]
    [SerializeField] float dashSpeed = 50;
    [SerializeField] float dashCooldownTime = 2.0f;
    private float dashcurdownTime;
    private float startDashTime = 0.1f;
    private float dashTime;
    private Vector3 walkDirection = new Vector3();
    private bool dashRequest;

    [Header("Camera")]
    [SerializeField] Transform cam;
    
    [Header("Misc")]
    [SerializeField] public LayerMask wallMask;
    [SerializeField] public GameObject actualPlayer;
    [SerializeField] Animator playerAnime;


    private void Awake()
    {
        if (actualPlayer)
        {
            playerAnime = actualPlayer.GetComponent<Animator>();
        }
        dashTime = startDashTime;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Horizontal") > 0.1f || Input.GetAxis("Vertical") > 0.1f || Input.GetAxis("Horizontal") < -0.1f || Input.GetAxis("Vertical") < -0.1f)
        {
            GroundMovement();
            CheckDirection();
        }
        else
        {
            ResetAnime();
            playerAnime.SetTrigger("isIdle");
        }
        if(dashcurdownTime < 0)
        {
            if (Input.GetButtonDown("Dash"))
            {
                StartCoroutine(DashForward());
            }
        }
        else
        {
            dashcurdownTime -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (moveRequest)
        { 
            SetCharacterRotation();
            transform.Translate(movePlayer * moveSpeed * Time.fixedDeltaTime);
            moveRequest = false;
        }
        if (dashRequest)
        {
            GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            Debug.Log(GetComponent<Rigidbody>().collisionDetectionMode);
            GetComponent<Rigidbody>().AddForce(actualPlayer.transform.forward * dashSpeed,ForceMode.Impulse);
            StartCoroutine(cam.gameObject.GetComponent<CamShake>().LowScreenShake());
            dashRequest = false;
        }
    }
    public void SetCharacterRotation()
    {
        Quaternion dirWeWant = Quaternion.LookRotation(walkDirection);
        actualPlayer.transform.rotation = Quaternion.Lerp(actualPlayer.transform.rotation, dirWeWant, rotationSpeed*Time.fixedDeltaTime);
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
        ResetAnime();
        playerAnime.SetTrigger("isRunning");
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
            dashcurdownTime = dashCooldownTime;
            GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Discrete;
            Debug.Log(GetComponent<Rigidbody>().collisionDetectionMode);
            yield break;
        }
    }
    public void ResetAnime()
    {
        playerAnime.ResetTrigger("isRunning");
        playerAnime.ResetTrigger("isIdle");
    }
}