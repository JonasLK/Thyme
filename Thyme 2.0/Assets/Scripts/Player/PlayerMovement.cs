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
    [HideInInspector]
    public Rigidbody rb;

    [Header("Player Dash")]
    [SerializeField] public float dashSpeed = 50;
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
    float minimumFloat = 0.1f;
    public List<GameObject> nearbyEnemy = new List<GameObject>();
    ComboHolder combo;

    [Header("Slope Check")]
    public float height = 0.5f;
    public float heightPadding = 0.05f;
    public LayerMask ground;
    public float maxGround = 120;
    public bool debug;
    public float rangeCast;

    float prevVel;
    Vector3 forward;
    RaycastHit hit;
    public float minimumVel;
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
        playerAnime.speed = GameManager.gameTime;
        if (SlowDownCheck())
        {
            return;
        }
        //CheckGround();

        combo.DirectionalInputCheck();

        combo.InputCheck();

        if (combo.curSlash != null && combo.inCombo)
        {
            combo.Timer(combo.curSlash.animTimer, combo.curSlash.maxTimer);
        }

        if (curplayerHp <= 0 && !DevMode.devMode)
        {
            curState = PlayerState.Death;
        }

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
        switch (curState)
        {
            case PlayerState.Normal:
                GroundMovement();
                SetJump();
                CheckAttack();
                break;
            case PlayerState.Dash:
                if (combo.doesAttack.AbleToMoveCheck(combo.doesAttack.rangeToMove))
                {
                    CancelInvoke();
                    StartCoroutine(DashForward());
                    ReturnState();
                }
                else
                {
                    ReturnState();
                }
                break;
            case PlayerState.Jumping:
                CheckJumpInput();
                ReturnState();
                break;
            case PlayerState.Ability:
                GroundMovement();
                SetJump();
                CheckAttack();
                break;
            case PlayerState.Interacting:
                break;
            case PlayerState.Attack:

                CheckNearbyEnemy();
                ResetAnime();
                if (!IsInvoking() && GameManager.IsPlaying(playerAnime, 0, "Attack") && playerAnime.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
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
                if(rb.collisionDetectionMode == CollisionDetectionMode.ContinuousDynamic)
                {
                    rb.collisionDetectionMode = collisionSet;
                }
                if (!IsInvoking())
                {
                    if (GameManager.IsPlaying(playerAnime,0,"Landing") && playerAnime.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
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

    public bool SlowDownCheck()
    {
        if(GameManager.gameTime == 0)
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            return true;
        }
        else
        {
            rb.useGravity = true;
            return false;
        }
    }

    public void DoDamage(float dam)
    {
        PlayAnime("Idle");
        curplayerHp -= dam;
        GameManager.instance.uiMan.SetBloodScreen();
        GameManager.instance.uiMan.Invoke("ResetBloodScreen", GameManager.instance.uiMan.dur);
    }

    public void AddForceToPlayer(Vector3 dir, float amount)
    {
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.velocity += dir * amount;
    }

    private void CheckNearbyEnemy()
    {
        Collider[] collider = Physics.OverlapSphere(actualPlayer.transform.position,range);
        for (int i = 0; i < collider.Length; i++)
        {
            if (collider[i].transform.tag == "Boss" || collider[i].transform.tag == "Enemy" && !nearbyEnemy.Contains(collider[i].gameObject))
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
            SetCharacterRotation();
            if (actualCam.GetComponent<CamLock>().camMode == CameraMode.Normal)
            {
                transform.Translate(actualPlayer.transform.forward * curMovespeed * GameManager.gameTime * Time.fixedDeltaTime);
            }
            else if (actualCam.GetComponent<CamLock>().camMode == CameraMode.Lockon)
            {
                transform.Translate(movePlayer * curMovespeed * GameManager.gameTime * Time.fixedDeltaTime);
            }
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
            if(curState == PlayerState.Ability)
            {
                rb.velocity = Vector3.zero;
                rb.velocity += actualPlayer.transform.forward * GetComponent<AbilityBase>().dashSpeed;
                GetComponent<AbilityBase>().curEnhancedDashCooldown = GetComponent<AbilityBase>().dashCooldown;
            }
            else
            {
                rb.velocity = Vector3.zero;
                rb.velocity += actualPlayer.transform.forward * dashSpeed;
            }
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

    public void SetCharacterRotation()
    {
        if (walkDirection != Vector3.zero)
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
        //DrawDebugLine();
        if(CheckInput())
        {
            moveRequest = true;
        }
    }

    private void DrawDebugLine()
    {
        if (!debug) return;
        Debug.DrawLine(actualPlayer.transform.position, actualPlayer.transform.position - Vector3.up , Color.green);
    }

    public void CheckGround()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rangeCast, ground))
        {
            if (rb.velocity.y <= 0.01 || rb.velocity.y >= -0.01)
            {
                if (playerAnime.GetCurrentAnimatorStateInfo(0).IsTag("Jumping"))
                {
                    rb.velocity = Vector3.zero;
                }
            }
            //actualPlayer.transform.rotation = Quaternion.LookRotation(actualPlayer.transform.forward, hit.normal);

            curAmountJump = 0;
            inAir = false;
        }
        else
        {
            inAir = true;
        }

        if (rb.velocity.y == 0)
        {
            if (prevVel < -minimumVel || playerAnime.GetCurrentAnimatorStateInfo(0).IsTag("Jumping"))
            {
                if (!playerAnime.GetCurrentAnimatorStateInfo(0).IsTag("Landing"))
                {
                    //actualPlayer.transform.up = hit.normal;
                    PlayAnime("Landing");
                    GameManager.instance.particleMan.landsEffect.Play();
                    curState = PlayerState.Landing;
                }
            }
        }
        prevVel = Mathf.Round(rb.velocity.y);
    }

    public bool CheckInput()
    {
        if (hor > minimumFloat || ver > minimumFloat || hor < -minimumFloat || ver < -minimumFloat)
            return true;
        else
        {
            return false;
        }
    }

    public void CheckPlayerMovement()
    {
        ResetAnime();
        if (!inAir)
        {
            if (CheckInput())
            {
                playerAnime.SetTrigger("isRunning");
            }
            else
            {
                playerAnime.SetTrigger("isIdle");
            }
        }
        else if(rb.velocity.y > minimumVel)
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
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.velocity += Vector3.up * Physics.gravity.y * tempForce * Time.fixedDeltaTime * GameManager.gameTime;
                return;
            }
            else
            {
                rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime * GameManager.gameTime;
            }
        }
        else if (!inAir && !Input.GetButton("Jump"))
        {
            if (curState == PlayerState.Attack)
            {
                return;
            }
            else
            {
                rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime * GameManager.gameTime;
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

    public IEnumerator DashForward()
    {
        while (true)
        {
            dashRequest = true;
            GetComponent<CapsuleCollider>().enabled = false;
            yield return new WaitForSeconds(startDashTime);
            ReturnState();
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            dashcurdownTime = dashCooldownTime;
            GetComponent<Rigidbody>().collisionDetectionMode = collisionSet;
            GetComponent<CapsuleCollider>().enabled = true;
            yield break;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(rb.velocity.x != 0 || rb.velocity.z != 0)
        {
            rb.velocity = Vector3.zero;
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
            if (inAir)
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