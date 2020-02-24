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
        Hit,
        Looking,
        Idle

    }
    [Header("Enemy State")]
    public State curState;
    private State tempState;
    [Header("FOV Enemy")]
    public float viewRadius;
    [Range(0,360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public float delay;

    [Header("Misc")]
    public float hitStun;
    public float attackRange;
    public float lookingTime;
    public float rotateSpeed;
    public float moveSpeed;
    public float curMoveSpeed;
    float distanceToTarget;
    public Transform point;
    public Transform target;
    public Animator anim;
    public NavMeshAgent agent;
    public RaycastHit hit;
    public Vector3 startLoc;

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = attackRange;
    }
    private void Update()
    {
        switch (curState)
        {
            case State.Chase:
                FindVisibleTargets();
                CheckFalling();
                break;
            case State.Patrol:
                GoToPoint();
                FindVisibleTargets();
                CheckFalling();
                break;
            case State.Idle:
                break;
            case State.Falling:
                Fall();
                break;
            case State.Hit:
                Hit();
                break;
            case State.Looking:
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
        if(target != null)
        { 
            Invoke("ResetState",lookingTime);
        }
    }

    public void Hit()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("Hit"))
        {
            if (!IsInvoking("ResetState"))
            {
                tempState = curState;
                curState = State.Hit;
                Invoke("ResetState", hitStun);
            }
        }
    }

    public void Fall()
    {
        //ResetAnime();
        //anim.SetTrigger("isFalling");
        if (!GetComponent<EnemyInfo>().inAir)
        {
            ResetState();
        }
    }

    public void CheckFalling()
    {
        if (GetComponent<EnemyInfo>().inAir)
        {
            tempState = curState;
            curState = State.Falling;
            Debug.Log("CurFalling");
        }
    }

    public void ResetState()
    {
        curState = tempState;
        tempState = State.Patrol;
        target = null;
    }

    public void GoToPoint()
    {
        if (!point)
        {
            
            curState = State.Idle;
            return;
        }
        float disTillPoint = Vector3.Distance(transform.position,point.position);
        if(disTillPoint < attackRange)
        {
            point = point.GetComponent<Point>().nextPoint;
        }
        
        Move(point);
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
        if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("Landing"))
        {
            agent.destination = target.position;
            agent.speed = moveSpeed * GetComponent<EnemyInfo>().curSpeedMultiplier * Time.fixedDeltaTime;
            agent.angularSpeed = rotateSpeed * GetComponent<EnemyInfo>().curSpeedMultiplier;
            ResetAnime();
            anim.SetTrigger("isChasing");
        }
    }

    private void CheckPos(float dis,Transform target)
    {
        if(dis > attackRange)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") || anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                Move(target);
            }
        }
        else
        {
            anim.Play("Attack");
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
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle * 0.5f)
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
                    ResetState();
                }
            }
            else if(distanceToTarget < attackRange)
            {
                agent.angularSpeed = rotateSpeed;
                SetLooking();
            }
            else
            {
                target = null;
                ResetState();
            }
        }
        if(targetsInVieuwRadius.Length == 0)
        {
            if (target != null)
            {
                LookingForTarget();
            }
            else
            {
                ResetState();
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
        anim.ResetTrigger("isAttacking");
    }
}
