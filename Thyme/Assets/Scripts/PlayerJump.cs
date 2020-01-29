using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{ 
    [Header("PlayerJump")]
    public float jumpPower=5;
    public bool doubleJump, inAir; 
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    bool jumpRequest, groundJump;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag == "Ground")
        {
            inAir = false;
            doubleJump = true;
            groundJump = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Ground")
        {
            inAir = true;
            groundJump = false;
        }
    }

    private void Update()
    {
        CheckInput();
    }
    private void FixedUpdate()
    {
        CheckGravity();
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
            if (!inAir && groundJump)
            {
                Jump();
                groundJump = false;
                return;
            }
            if (inAir && doubleJump)
            {
                Jump();
                doubleJump = false;
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
