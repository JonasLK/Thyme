using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum BossState
{
    Idle,
    Chasing,
    ObeliskReady,
    Blocked,
    Dying
}

public class BossInfo : MonoBehaviour
{
    [Header("State")]
    public BossState curBossState = BossState.Idle;

    [Header("Stats")]
    public float maxHealth = 100;
    public float curHealth;
    public float attackRange;
    public float attackDelay;

    [Header("FOV Boss")]
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    public LayerMask targetMask;

    [Header("Misc")]
    public Transform target;
    public Transform chargePoint;
    public ParticleSystem walkingLeft;
    public ParticleSystem walkingRight;
    public Vector3 offset;
    public float interuptionRad;

    private Animator anime;
    private NavMeshAgent agent;
    private float distanceToTarget;
    private RaycastHit hit;

    [HideInInspector]
    public Vector3 actualRayStart;
    Vector3 dirtoChargePoint;
    float tempDis;
    bool nextAttack;


    private void Awake()
    {
        curHealth = maxHealth;
        anime = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = attackRange;
    }

    private void Update()
    {
        actualRayStart = transform.position + offset;
        if( curHealth <= 0)
        {
            curBossState = BossState.Dying;
        }
        switch (curBossState)
        {
            case BossState.Idle:
                CheckCharge();
                FindVisibleTargets();
                break;
            case BossState.Chasing:
                CheckCharge();
                Move(target);
                CheckIdle();
                break;
            case BossState.ObeliskReady:
                Move(chargePoint);
                Invoke("CheckBlock",0.5f);
                break;
            case BossState.Blocked:
                if (!nextAttack)
                {
                    CancelInvoke("CheckBlock");
                    Invoke("AttackInvoke",0f);
                    nextAttack = true;
                }
                break;
            case BossState.Dying:
                Death();
                break;
            default:
                break;
        }
    }

    public void AttackInvoke()
    {
        PlayAnime("Attack");
    }

    public void HitStun()
    {
        //TODO HitStunAnimation
        ResetAnime();
        anime.SetTrigger("isIdle");
        if (!IsInvoking("Stun"))
        {
            Invoke("Stun", attackDelay);
        }
    }

    public void Stun()
    {
        CheckCharge();
    }

    public void AdjustHealth(float i, bool launch)
    {
        curHealth -= i;
        anime.Play("Hit");
    }

    public void Death()
    {
        if (!anime.GetCurrentAnimatorStateInfo(0).IsTag("Landing"))
        {
            PlayAnime("Landing");
        }
        Destroy(gameObject, anime.GetCurrentAnimatorStateInfo(0).length);
    }

    public void CheckCharge()
    {
        if (chargePoint)
        {
            curBossState = BossState.ObeliskReady;
        }
        else
        {
            curBossState = BossState.Idle;
        }
    }

    public void CheckBlock()
    {
        dirtoChargePoint = (chargePoint.position - transform.position);
        tempDis = Vector3.Distance(actualRayStart, chargePoint.position);
        if (Physics.SphereCast(actualRayStart, interuptionRad, dirtoChargePoint, out hit, tempDis, targetMask, QueryTriggerInteraction.Ignore))
        {
            if(hit.distance < attackRange)
            {
                agent.isStopped = true;
                curBossState = BossState.Blocked;
            }
        }
        else
        {
            curBossState = BossState.ObeliskReady;
        }
        nextAttack = false;
    }

    void Move(Transform target)
    {
        float tempDis = Vector3.Distance(transform.position, target.position);
        if (tempDis < viewRadius || curBossState == BossState.ObeliskReady)
        {
            ResetAnime();
            anime.SetTrigger("isChasing");
            if (agent.isStopped && anime.GetCurrentAnimatorStateInfo(0).IsTag("Run"))
            {
                agent.isStopped = false;
            }
            agent.destination = target.position;
        }
    }

    public void CheckIdle()
    {
        //Check if Pylons are Ready
        if (!target)
        {
            ResetAnime();
            curBossState = BossState.Idle;
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
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle * 0.5f && curBossState != BossState.ObeliskReady)
            {
                curBossState = BossState.Chasing;
            }
            else
            {
                target = null;
            }
        }
        if(targetsInVieuwRadius.Length == 0)
        {
            target = null;
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

    private void PlayAnime(string v)
    {
        anime.Play(v);
    }

    private void ResetAnime()
    {
        anime.ResetTrigger("isIdle");
        anime.ResetTrigger("isChasing");
    }

    private void OnDrawGizmosSelected()
    {
        if (chargePoint)
        {
            Gizmos.color = Color.cyan;
            Debug.DrawLine(actualRayStart, actualRayStart + dirtoChargePoint * tempDis, Color.cyan);
            Gizmos.DrawWireSphere(actualRayStart + dirtoChargePoint * tempDis, interuptionRad);
        }
    }
}
