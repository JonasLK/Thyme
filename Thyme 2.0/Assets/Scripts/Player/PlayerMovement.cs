using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum PlayerState
    {
        Normal,
        Dash,
        Jumping,
        Death,
        Ability,
        Attack,
        Interacting
    }

    [Header("PlayerState")]
    public PlayerState curState = PlayerState.Normal;

    [Header("Player Movement")]
    [SerializeField] float moveSpeed = 10;
    private float ver, hor;
    private bool moveRequest;
    private Vector3 movePlayer;

    [Header("Player Rotation")]
    [SerializeField] float walkingRotationSpeed = 5;
    [SerializeField] float dashingRotationSpeed = 360;

    [Header("PlayerJump")]
    public float jumpPower = 7f;
    public int maxAmountJumps = 3;
    int curAmountJump;
    public bool inAir;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public bool jumpRequest;
    Rigidbody rb;

    [Header("Player Dash")]
    [SerializeField] float dashSpeed = 50;
    [SerializeField] float dashCooldownTime = 0.5f;
    private float dashcurdownTime;
    private float startDashTime = 0.1f;
    private Vector3 walkDirection = new Vector3();
    private bool dashRequest;

    [Header("Player Interaction")]
    [SerializeField] public float rad;
    public LayerMask interactable;

    [Header("Camera")]
    [SerializeField] public Transform actualCam;
    [SerializeField] public GameObject cam;

    [Header("RayCast")]
    public float dis = 5f;
    public RaycastHit hit;

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
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (curState)
        {
            case PlayerState.Normal:
                GroundMovement();
                SetJump();

                CheckInteraction();
                CheckDash();
                break;
            case PlayerState.Dash:
                StartCoroutine(DashForward());
                ReturnState();
                break;
            case PlayerState.Jumping:
                CheckJumpInput();
                ReturnState();
                break;
            case PlayerState.Ability:
                AimRotation();
                break;
            case PlayerState.Interacting:
                break;
        }
    }

    private void CheckInteraction()
    {
        Collider[] c = Physics.OverlapSphere(GetComponent<AbilityBase>().handModel.transform.position + GetComponent<AbilityBase>().handModel.transform.forward, rad,interactable);
        for (int i = 0; i < c.Length; i++)
        {
            //TODO cogs script check if you have enough cogs to close gate
            if(c[i].tag == "Portal")
            {
                GetComponent<ComboHolder>().ableToAttack = false;
                if (Input.GetButtonDown("Fire1"))
                {
                    c[i].GetComponent<Portal>().Close();
                    Debug.Log("Closing");
                }
                break;
            }
            else
            {
                GetComponent<ComboHolder>().ableToAttack = true;
            }
        }
        if(c.Length == 0)
        {
            GetComponent<ComboHolder>().ableToAttack = true;
        }
    }

    private void FixedUpdate()
    {
        if (rb.useGravity)
        {
            CheckGravity();
        }
        if (moveRequest && !dashRequest)
        {
            SetCharacterWalkingRotation();
            transform.Translate(movePlayer * moveSpeed * Time.fixedDeltaTime);
            moveRequest = false;
        }
        if (jumpRequest)
        {
            if (!GameManager.instance.soundMan.IsPlaying("Jump"))
            {
                GameManager.instance.soundMan.Play("Jump");
            }
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            jumpRequest = false;
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

    public void SetAbility()
    {
        ResetAnime();
        playerAnime.SetTrigger("isIdle");
        curState = PlayerState.Ability;
    }

    public void ReturnState()
    {
        curState = PlayerState.Normal;
    }

    public void AimRotation()
    {
        Vector3 aimDirection = new Vector3(actualCam.forward.x, 0, actualCam.forward.z);
        Quaternion dirWeWant = Quaternion.LookRotation(aimDirection);
        actualPlayer.transform.rotation = Quaternion.Lerp(actualPlayer.transform.rotation, dirWeWant, walkingRotationSpeed * Time.fixedDeltaTime);
    }

    public void SetCharacterWalkingRotation()
    {
        if (walkDirection != Vector3.zero)
        {
            Quaternion dirWeWant = Quaternion.LookRotation(walkDirection);
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
        ver = Input.GetAxis("Vertical");
        hor = Input.GetAxis("Horizontal");
        movePlayer.x = hor;
        movePlayer.z = ver;
        walkDirection = Vector3.zero;
        CheckDirection();
        CheckPlayerMovement();
        moveRequest = true;
    }

    public void CheckPlayerMovement()
    {
        if (hor > 0f  || ver > 0f || hor < -0f || ver < -0f)
        {
            ResetAnime();
            playerAnime.SetTrigger("isRunning");
        }
        else
        {
            ResetAnime();
            playerAnime.SetTrigger("isIdle");
        }
    }

    public void CheckJumpInput()
    {
        if (rb.useGravity)
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

    public void SetJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            curState = PlayerState.Jumping;
        }
    }

    public void CheckDash()
    {
        if (dashcurdownTime < 0)
        {
            if (Input.GetButtonDown("Dash"))
            {
                curState = PlayerState.Dash;
            }
        }
        else
        {
            dashcurdownTime -= Time.deltaTime;
        }
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

    private void OnTriggerEnter(Collider c)
    {
        if (c.transform.tag == "Ground")
        {
            if (GameManager.instance.soundMan.IsPlaying("Jump"))
            {
                GameManager.instance.soundMan.Stop("Jump");
            }
            if(rb.velocity.y < 0.1f)
            {
                inAir = false;
                PlayAnime("Landing");
                curAmountJump = 0;
            }
        }
    }

    private void OnTriggerStay(Collider c)
    {
        if (c.transform.tag == "Ground")
        {
            inAir = false;
        }
    }

    private void OnTriggerExit(Collider c)
    {
        if (c.transform.tag == "Ground")
        {
            inAir = true;
        }
    }

    public void PlayAnime(string animeName)
    {
        playerAnime.Play(animeName);
    }

    public void ResetAnime()
    {
        playerAnime.ResetTrigger("isIdle");
        playerAnime.ResetTrigger("isRunning");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GetComponent<AbilityBase>().handModel.transform.position + GetComponent<AbilityBase>().handModel.transform.forward, rad);
    }
}