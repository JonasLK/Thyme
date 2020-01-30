using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{ 
    [Header("PlayerJump")]
    public float jumpPower=5;
    public int maxAmountJumps;
    int curAmountJumps;
    public bool inAir; 
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    bool jumpRequest;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider c)
    {
        if (c.transform.tag == "Ground" || c.transform.tag == "Ledge")
        {
            inAir = false;
            curAmountJumps = maxAmountJumps;
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
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up * jumpPower,ForceMode.Impulse);
            jumpRequest = false;
        }
    }

    public void CheckInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            print("Jump");
            if (curAmountJumps > 0)
            {
                Jump();
                curAmountJumps--;
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
