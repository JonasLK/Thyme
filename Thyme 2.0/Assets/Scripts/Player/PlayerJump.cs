﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [Header("PlayerJump")]
    public float jumpPower = 7f;
    public int maxAmountJumps = 3;
    int curAmountJump;
    public bool inAir; 
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public bool jumpRequest;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider c)
    {
        if (c.transform.tag == "Ground" || c.transform.tag == "Ledge")
        {
            if (GameManager.instance.soundMan.IsPlaying("Jump"))
            {
                GameManager.instance.soundMan.Stop("Jump");
            }
            GetComponent<PlayerMovement>().PlayAnime("Landing");
            inAir = false;
            curAmountJump = 0;
        }
    }

    private void OnTriggerStay(Collider c)
    {
        if (c.transform.tag == "Ground" || c.transform.tag == "Ledge")
        {
            inAir = false;
        }
    }

    private void OnTriggerExit(Collider c)
    {
        if (c.transform.tag == "Ground" || c.transform.tag == "Ledge")
        {
            inAir = true;
        }
    }

    private void Update()
    {
        if (rb.useGravity)
        {
            CheckInput();
        }
    }
    private void FixedUpdate()
    {
        if (rb.useGravity)
        {
            CheckGravity();
        }
        if (jumpRequest)
        {
            if (!GameManager.instance.soundMan.IsPlaying("Jump"))
            {
                GameManager.instance.soundMan.Play("Jump");
            }
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up * jumpPower,ForceMode.Impulse);
            jumpRequest = false;
        }
    }

    public void CheckInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (curAmountJump < maxAmountJumps)
            {
                GetComponent<PlayerMovement>().PlayAnime("Jump " + curAmountJump.ToString());
                Jump();
                curAmountJump++;
                return;
            }
        }
    }

    public void CheckGravity()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    public void Jump()
    {
        jumpRequest = true;
    }
}
