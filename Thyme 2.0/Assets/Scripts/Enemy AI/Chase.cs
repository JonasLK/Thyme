using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : MonoBehaviour
{
    public enum State
    {
        Chase,
        Patrol,
        Falling,
        Hit

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
    public float rotateSpeed;
    public float speed;
    public float attackRange;
    public Transform point;
    public Animator anim;

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
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
            case State.Falling:
                Fall();
                break;
            case State.Hit:
                Hit();
                break;
            default:
                break;
        }
    }

    public void Hit()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("Hit"))
        {
            tempState = curState;
            curState = State.Hit;
            ResetState();
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
    }

    public void GoToPoint()
    {
        Vector3 dirToPoint = (point.position - transform.position).normalized;
        dirToPoint.y = 0;
        transform.rotation = Quaternion.Lerp(transform.rotation,
                                    Quaternion.LookRotation(dirToPoint),
                                    rotateSpeed * Time.fixedDeltaTime * GetComponent<EnemyInfo>().timeMuliplier * GameManager.gameTime);
        Move();
        ResetAnime();
        anim.SetTrigger("isChasing");
    }

    void Move()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("Landing"))
        {
            transform.Translate(new Vector3(0, 0, speed) * Time.fixedDeltaTime * GetComponent<EnemyInfo>().timeMuliplier * GameManager.gameTime);
        }
    }

    private void CheckPos(float dis)
    {
        if(dis > attackRange)
        {
            Move();
            ResetAnime();
            anim.SetTrigger("isChasing");
        }
        else
        {
            ResetAnime();
            anim.SetTrigger("isAttacking");
        }
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        Collider[] targetsInVieuwRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        for (int i = 0; i < targetsInVieuwRadius.Length; i++)
        {
            Transform target = targetsInVieuwRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            dirToTarget.y = 0;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle * 0.5f)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation,
                                    Quaternion.LookRotation(dirToTarget),
                                    rotateSpeed * Time.fixedDeltaTime * GetComponent<EnemyInfo>().timeMuliplier * GameManager.gameTime);
                    CheckPos(dstToTarget);
                    Debug.Log("Target Found");
                    curState = State.Chase;
                }
                else
                {
                    curState = State.Patrol;
                }
            }
            else
            {
                curState = State.Patrol;
            }
        }
        if(targetsInVieuwRadius.Length == 0)
        {
            curState = State.Patrol;
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
