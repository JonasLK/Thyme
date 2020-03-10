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
        Charge,
        Interacting,
        Landing
    }

    [Header("PlayerState")]
    public PlayerState curState = PlayerState.Normal;

    [Header("PlayerStats")]
    public float playerHp;
    public float curplayerHp;

    [Header("Player Movement")]
    [SerializeField] public float moveSpeed = 10;
    [SerializeField] public float rotateSpeed = 10;
    public float softLockSpeed = 2f;
    private float ver, hor;
    private bool moveRequest;
    private Vector3 movePlayer;

    [Header("PlayerJump")]
    [Range(0,1)]
    public float jumpPower = 0.65f;
    public int maxAmountJumps = 3;
    int curAmountJump;
    public bool inAir;
    public bool jumpRequest;
    Rigidbody rb;

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

    [Header("Collider")]
    public float range = 5f;

    [Header("Misc")]
    [SerializeField] public CollisionDetectionMode collisionSet = CollisionDetectionMode.Continuous;
    [SerializeField] public LayerMask wallMask;
    [SerializeField] public GameObject actualPlayer;
    [SerializeField] public Animator playerAnime;
    public float tempForce;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float hitMultiplier = 2f;
    public float jumpingVel;
    public float curMovespeed;
    public List<GameObject> nearbyEnemy = new List<GameObject>();
    ComboHolder combo;


    private void Awake()
    {
        if (actualPlayer)
        {
            playerAnime = actualPlayer.GetComponent<Animator>();
        }
        curMovespeed = moveSpeed;
        curplayerHp = playerHp;
        rb = GetComponent<Rigidbody>();
        combo = GetComponent<ComboHolder>();
    }

    // Update is called once per frame
    void Update()
    {
        combo.DirectionalInputCheck();

        combo.InputCheck();

        if (combo.curSlash != null && combo.inCombo)
        {
            combo.Timer(combo.curSlash.animTimer, combo.curSlash.maxTimer);
        }

        if(curplayerHp <= 0)
        {
            curState = PlayerState.Death;
        }

        switch (curState)
        {
            case PlayerState.Normal:
                GroundMovement();
                SetJump();
                CheckAttack();
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
                GroundMovement();
                SetJump();
                CheckAttack();
                CheckDash();
                break;
            case PlayerState.Interacting:
                break;
            case PlayerState.Attack:
                CheckNearbyEnemy();
                CheckDash();
                ResetAnime();
                if (!IsInvoking() && playerAnime.GetCurrentAnimatorStateInfo(0).IsTag("Attack") && playerAnime.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    if (GetComponent<ComboHolder>().curSlash != null)
                    {
                        Invoke("ReturnState", GetComponent<ComboHolder>().curSlash.animTimer);
                    }
                    else
                    {
                        Invoke("ReturnState", 0);
                    }
                }
                break;
            case PlayerState.Charge:
                ResetAnime();
                playerAnime.SetTrigger("isIdle");
                CheckAttack();
                break;
            case PlayerState.Landing:
                if (!IsInvoking())
                {
                    if (playerAnime.GetCurrentAnimatorStateInfo(0).IsTag("Landing") && playerAnime.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                    {
                        Invoke("ReturnState", 0);
                    }
                }
                break;
            case PlayerState.Death:
                Time.timeScale = 0;
                break;
        }
    }

    private void CheckNearbyEnemy()
    {
        Collider[] collider = Physics.OverlapSphere(actualPlayer.transform.position,range);
        for (int i = 0; i < collider.Length; i++)
        {
            if (collider[i].transform.tag == "Enemy" && !nearbyEnemy.Contains(collider[i].gameObject))
            {
                nearbyEnemy.Add(collider[i].gameObject);
                break;
            }
        }
        if(nearbyEnemy.Count > 0)
        {
            if (!nearbyEnemy[0])
            {
                nearbyEnemy.Remove(nearbyEnemy[0]);
                return;
            }
            float distoEnemy = Vector3.Distance(actualPlayer.transform.position, nearbyEnemy[0].transform.position);
            if(distoEnemy > range)
            {
                nearbyEnemy.Remove(nearbyEnemy[0]);
                return;
            }
            Vector3 aimDirection = nearbyEnemy[0].transform.position - actualPlayer.transform.position;
            Quaternion dirWeWant = Quaternion.LookRotation(aimDirection);
            Vector3 actualRotation = Quaternion.Lerp(actualPlayer.transform.rotation, dirWeWant, softLockSpeed * Time.fixedDeltaTime).eulerAngles;
            actualPlayer.transform.rotation = Quaternion.Euler(0, actualRotation.y, 0);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(actualPlayer.transform.position, range);
    }

    public void CheckAttack()
    {
        if (GetComponent<ComboHolder>().ableToAttack == false)
        {
            curState = PlayerState.Attack;
            ResetAnime();
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
            if (actualCam.GetComponent<CamLock>().camMode == CameraMode.Normal)
            {
                SetCharacterWalkingRotation();
            }
            else if (actualCam.GetComponent<CamLock>().camMode == CameraMode.Lockon)
            {
                SetLockRotation();
            }
            transform.Translate(movePlayer * curMovespeed * Time.fixedDeltaTime);
            moveRequest = false;
        }
        if (jumpRequest)
        {
            if (!GameManager.instance.soundMan.IsPlaying("Jump"))
            {
                GameManager.instance.soundMan.Play("Jump");
            }
            AddVel(jumpPower);
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

    public void AddVel(float power)
    {
        rb.velocity = Vector3.zero;
        rb.velocity = Vector3.up * -power * Physics.gravity.y;
    }

    public void SetAbility()
    {
        ResetAnime();
        playerAnime.SetTrigger("isIdle");
        curState = PlayerState.Ability;
    }

    public void ReturnState()
    {
        GameManager.instance.particleMan.swordSlash.Stop();
        curState = PlayerState.Normal;
    }

    public void SetLockRotation()
    {
        Vector3 aimDirection = new Vector3(0, actualCam.forward.y, 0);
        Quaternion dirWeWant = Quaternion.LookRotation(aimDirection);
        Vector3 rotate = Quaternion.Lerp(actualPlayer.transform.rotation, dirWeWant, rotateSpeed * Time.fixedDeltaTime).eulerAngles;
        actualPlayer.transform.rotation = Quaternion.Euler(0,rotate.y,0);
    }

    public void SetCharacterWalkingRotation()
    {
        if (walkDirection != Vector3.zero)
        {
            Quaternion dirWeWant = Quaternion.LookRotation(walkDirection);
            actualPlayer.transform.rotation = dirWeWant;
        }
    }

    public void SetCharacterRotation()
    {
        if(walkDirection != Vector3.zero)
        {
            Quaternion dirWeWant = Quaternion.LookRotation(walkDirection);
            actualPlayer.transform.rotation = dirWeWant;
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
        ResetAnime();
        if (!inAir)
        {
            if (hor > 0f  || ver > 0f || hor < -0f || ver < -0f)
            {
                playerAnime.SetTrigger("isRunning");
            }
            else
            {
                playerAnime.SetTrigger("isIdle");
            }
        }
        else
        {
            if (!playerAnime.GetCurrentAnimatorStateInfo(0).IsTag("Jumping"))
            {
                playerAnime.Play("Jump 0", 0, 0);
            }
        }
    }

    public void CheckJumpInput()
    {
        if (rb.useGravity)
        {
            if (curAmountJump < maxAmountJumps)
            {
                GetComponent<PlayerMovement>().PlayAnime("Jump " + curAmountJump.ToString());
                GameManager.instance.particleMan.jumpEffect.Play();
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
            if(curState == PlayerState.Attack || curState == PlayerState.Charge && inAir)
            {
                //Todo Add Force with curSlash
                rb.velocity = Vector3.zero;
                rb.velocity += Vector3.up * Physics.gravity.y * tempForce * Time.fixedDeltaTime;
                return;
            }
            else
            {
                rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            }
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            if (curState == PlayerState.Attack)
            {
                return;
            }
            else
            {
                rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
            }
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
            if(inAir)
            {
                inAir = false;
                ResetAnime();
                PlayAnime("Landing");
                GameManager.instance.particleMan.landsEffect.Play();
                curState = PlayerState.Landing;
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
        if (c.transform.tag == "Ground" && rb.velocity.y > jumpingVel || rb.velocity.y < -jumpingVel)
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
}