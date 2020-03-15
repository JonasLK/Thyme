using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chase : MonoBehaviour
{
    public enum State
    {
        Chase,
        Patrol,
        Falling,
        Landing,
        Hit,
        Bounce,
        Looking,
        Idle,
        Dying,
        SlowingDown
    }
    [Header("Enemy State")]
    public State curState;
    [Header("FOV Enemy")]
    public float viewRadius;
    [Range(0,360)]
    public float viewAngle;
    float curAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public float delay;

    [Header("Misc")]
    public float damage;
    public float attackDelay;
    public float hitStun;
    public float attackRange;
    public float lookingTime;
    public float rotateSpeed;
    public float moveSpeed;
    [HideInInspector]
    public float curMoveSpeed;
    float distanceToTarget;
    public Transform point;
    public Transform target;
    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public NavMeshAgent agent;
    EnemyInfo eInfo;
    public ParticleSystem walkingLeft;
    public ParticleSystem walkingRight;

    public RaycastHit hit;
    public Vector3 startLoc;

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        eInfo = GetComponent<EnemyInfo>();
        agent.stoppingDistance = attackRange;
    }
    private void Update()
    {
        switch (curState)
        {
            case State.Chase:
                curAngle = 360;
                FindVisibleTargets();
                CheckFalling();
                break;
            case State.Patrol:
                curAngle = viewAngle;
                FindVisibleTargets();
                GoToPoint();
                CheckFalling();
                break;
            case State.Idle:
                curAngle = 360;
                break;
            case State.Falling:
                curAngle = 360;
                Fall();
                break;
            case State.Bounce:
                break;
            case State.Landing:
                curAngle = 360;
                CheckLanding();
                break;
            case State.Hit:
                Hit();
                break;
            case State.Looking:
                curAngle = 360;
                FindVisibleTargets();
                if (!IsInvoking("CheckForEnemy"))
                {
                    Invoke("CheckForEnemy",0f);
                }
                break;
            default:
                break;
        }
    }

    public void CheckForEnemy()
    {
        if (!IsInvoking("ResetState"))
        {
            Invoke("ResetState",lookingTime);
        }
        else if(target != null)
        {
            CancelInvoke("ResetState");
        }
    }

    public void Hit()
    {
        agent.speed = 0;
        agent.angularSpeed = 0;
        ResetAnime();
        if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("Hit"))
        {
            if (!IsInvoking("AttackNearby") && curState != State.Bounce && curState != State.Falling)
            {
                Invoke("AttackNearby", hitStun);
            }
        }
    }

    public void AttackNearby()
    {
        if(curState != State.Dying)
        {
            curState = State.Chase;
        }
    }

    public void Fall()
    {
        ResetAnime();
        anim.SetTrigger("isFalling");
        if (!eInfo.inAir)
        {
            curState = State.Landing;
        }
    }

    public void CheckLanding()
    {
        if (!eInfo.inAir && anim.GetCurrentAnimatorStateInfo(0).IsTag("Landing") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            eInfo.rb.useGravity = true;
            ResetState();
        }
    }

    public void CheckFalling()
    {
        if (eInfo.inAir)
        {
            curState = State.Falling;
        }
    }

    public void ResetState()
    {
        agent.speed = agent.speed * eInfo.curSpeedMultiplier * Time.fixedDeltaTime;
        target = null;
        if (curState != State.Looking && curState != State.Falling)
        {
            curState = State.Looking;
        }
        else
        {
            curState = State.Patrol;
        }
    }

    public void GoToPoint()
    {
        if (!point)
        {
            curState = State.Idle;
            return;
        }
        float disTillPoint = Vector3.Distance(transform.position, point.position);
        if(disTillPoint < attackRange)
        {
            if (point.GetComponent<Point>().nextPoint != null)
            {
                point = point.GetComponent<Point>().nextPoint;
            }
            else
            {
                if(!anim.GetCurrentAnimatorStateInfo(0).IsTag("Landing") || !anim.GetCurrentAnimatorStateInfo(0).IsTag("Falling"))
                {
                    ResetAnime();
                    anim.SetTrigger("isIdle");
                    transform.rotation = point.rotation;
                }
            }
        }
        else
        {
            Move(point);
        }
        
    }

    void LookingForTarget()
    {
        SetLooking();
        agent.speed = 0;
        agent.angularSpeed = 0;
    }

    public void SetLooking()
    {
        ResetAnime();
        anim.SetTrigger("isIdle");
        curState = State.Looking;
    }

    void Move(Transform target)
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("Landing") || !anim.GetCurrentAnimatorStateInfo(0).IsTag("Falling"))
        {
            if (!eInfo.inAir)
            {
                agent.destination = target.position;
                agent.speed = moveSpeed * eInfo.curSpeedMultiplier * Time.fixedDeltaTime;
                agent.angularSpeed = rotateSpeed * eInfo.curSpeedMultiplier;
                ResetAnime();
                anim.SetTrigger("isChasing");
            }
        }
    }

    private void CheckPos(float dis,Transform target)
    {
        float arialDis = transform.position.y - target.position.y;
        if(dis > attackRange && arialDis < 1 && arialDis > -1 )
        {
            if (IsInvoking("Attack"))
            {
                CancelInvoke("Attack");
            }
            if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") || anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                Move(target);
            }
        }
        else if (arialDis < -1)
        {
            ResetAnime();
            anim.SetTrigger("isIdle");  
        }
        else
        {
            if (!eInfo.inAir)
            {
                agent.speed = 0;
                agent.angularSpeed = 0;
                ResetAnime();
                Vector3 aimDirection = target.transform.position - transform.position;
                Quaternion dirWeWant = Quaternion.LookRotation(aimDirection);
                Vector3 actualRotation = Quaternion.Lerp(transform.rotation, dirWeWant, (rotateSpeed / 2) * Time.fixedDeltaTime).eulerAngles;
                transform.rotation = Quaternion.Euler(0, actualRotation.y, actualRotation.z);
                if (!IsInvoking("Attack"))
                {
                    InvokeRepeating("Attack",0f, attackDelay);
                }
            }
        }
    }

    void Attack()
    {
        if (!eInfo.inAir && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            anim.Play("Attack",0,0);
        }
    }

    void FindVisibleTargets()
    {
        Collider[] targetsInVieuwRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        for (int i = 0; i < targetsInVieuwRadius.Length; i++)
        {
            Transform tempTarget = targetsInVieuwRadius[i].transform;
            target = tempTarget;
            Vector3 dirToTarget = (tempTarget.position - transform.position).normalized;
            distanceToTarget = Vector3.Distance(transform.position, tempTarget.position);
            if (Vector3.Angle(transform.forward, dirToTarget) < curAngle * 0.5f)
            {
                float tempDstToTarget = Vector3.Distance(transform.position, tempTarget.position);
                if (!Physics.Raycast(transform.position, dirToTarget,out hit, tempDstToTarget, obstacleMask))
                {
                    if (IsInvoking("ResetState"))
                    {
                        CancelInvoke("ResetState");
                    }
                    CheckPos(tempDstToTarget,tempTarget);
                    curState = State.Chase;
                }
                else
                {
                    LookingForTarget();
                }
            }
            else if(distanceToTarget <= attackRange)
            {
                agent.angularSpeed = rotateSpeed;
                SetLooking();
            }
            else
            {
                target = null;
                if(curState != State.Patrol)
                {
                    LookingForTarget();
                }
            }
        }
        if(targetsInVieuwRadius.Length == 0)
        {
            target = null;
            if (curState != State.Patrol)
            {
                LookingForTarget();
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void ResetAnime()
    {
        anim.ResetTrigger("isIdle");
        anim.ResetTrigger("isChasing");
    }
}
