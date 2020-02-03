﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] float moveSpeed = 10;
    private float ver, hor;
    private bool moveRequest;
    private Vector3 movePlayer;

    [Header("Player Rotation")]
    [SerializeField] float walkingRotationSpeed = 5;
    [SerializeField] float dashingRotationSpeed = 360;

    [Header("Player Dash")]
    [SerializeField] float dashSpeed = 50;
    [SerializeField] float dashCooldownTime = 0.5f;
    private float dashcurdownTime;
    private float startDashTime = 0.1f;
    private Vector3 walkDirection = new Vector3();
    private bool dashRequest;

    [Header("Camera")]
    [SerializeField] public Transform actualCam;
    [SerializeField] public GameObject cam;

    [Header("RayCast")]
    public float dis = 5f;
    public LayerMask interactable;
    public RaycastHit hit;
    bool onGround;

    [Header("Misc")]
    [SerializeField] public CollisionDetectionMode collisionSet = CollisionDetectionMode.Continuous;
    [SerializeField] public LayerMask wallMask;
    [SerializeField] public GameObject actualPlayer;
    [SerializeField] public Animator playerAnime;


    private void Awake()
    {
        if (actualPlayer)
        {
            playerAnime = actualPlayer.GetComponent<Animator>();
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(actualPlayer.transform.position,-actualPlayer.transform.up,out hit, dis, interactable, QueryTriggerInteraction.Ignore))
        {
            if(hit.transform.tag == "Ground")
            {
                onGround = true;
            }
            else
            {
                onGround = false;
            }
        }
        if (Input.GetAxis("Horizontal") > 0.1f || Input.GetAxis("Vertical") > 0.1f || Input.GetAxis("Horizontal") < -0.1f || Input.GetAxis("Vertical") < -0.1f)
        {
            if (!GetComponent<PlayerClimb>().hang)
            {
                if (!GetComponent<PlayerJump>().jumpRequest)
                {
                    GroundMovement();
                }
            }
            else
            {
                //Set HangingAnimation
                ResetAnime();
                playerAnime.SetTrigger("isIdle");
            }
        }
        else
        {
            if (!GetComponent<PlayerJump>().jumpRequest)
            {
                ResetAnime();
                playerAnime.SetTrigger("isIdle");
            }
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
        if (moveRequest && !dashRequest)
        {
            SetCharacterWalkingRotation();
            transform.Translate(movePlayer * moveSpeed * Time.fixedDeltaTime);
            moveRequest = false;
        }
        if (dashRequest)
        {
            SetCharacterRotation();
            playerAnime.Play("DodgeFront");
            GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            GetComponent<Rigidbody>().AddForce(actualPlayer.transform.forward * dashSpeed, ForceMode.Impulse);
            StartCoroutine(cam.gameObject.GetComponent<CamShake>().LowScreenShake());
            dashRequest = false;
        }
    }

    public void SetCharacterWalkingRotation()
    {
        if (walkDirection != Vector3.zero)
        {
            Quaternion dirWeWant = Quaternion.LookRotation(walkDirection);
            if (onGround)
            {
                actualPlayer.transform.rotation = Quaternion.FromToRotation(actualPlayer.transform.up,hit.normal);
            }
            actualPlayer.transform.rotation = Quaternion.Lerp(actualPlayer.transform.rotation, dirWeWant, walkingRotationSpeed * Time.fixedDeltaTime);
        }
    }
    public void SetCharacterRotation()
    {
        if(walkDirection != Vector3.zero)
        {
            Quaternion dirWeWant = Quaternion.LookRotation(walkDirection);
            actualPlayer.transform.rotation = Quaternion.RotateTowards(actualPlayer.transform.rotation, dirWeWant, dashingRotationSpeed);
        }
    }

    public void CheckDirection()
    {
        if(hor != 0 || ver != 0)
        {
            if (hor != 0)
            {
                if (hor > 0)
                {
                    walkDirection += actualCam.transform.right * hor;
                }
                else
                {
                    walkDirection -= actualCam.transform.right * -hor;
                }
            }
            if (ver != 0)
            {
                if (ver > 0)
                {
                    walkDirection += actualCam.transform.forward * ver;
                }
                else
                {
                    walkDirection -= actualCam.transform.forward * -ver;
                }
                
            }
        }
        walkDirection.y = 0;
        walkDirection.Normalize();
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
        CheckDirection();
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
            GetComponent<Rigidbody>().collisionDetectionMode = collisionSet;
            yield break;
        }
    }
    public void ResetAnime()
    {
        playerAnime.ResetTrigger("isIdle");
        playerAnime.ResetTrigger("isRunning");
        playerAnime.ResetTrigger("isHanging");
    }
    public void PlayAnime(string animeName)
    {
        playerAnime.Play(animeName);
    }
}