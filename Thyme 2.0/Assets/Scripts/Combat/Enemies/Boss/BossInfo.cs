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
    Draining,
    Return,
    Stunned,
    Dying
}

public class BossInfo : MonoBehaviour
{
    [Header("State")]
    public BossState curBossState = BossState.Idle;

    [Header("Stats")]
    public float maxHealth = 100;
    public float curHealth;
    public float damage = 15;
    public float attackRange;
    public float interactRange;
    public float attackDelay;
    public float drainTime;

    [Header("Misc")]
    public Transform midPoint;
    public Transform playerTarget;
    public Transform chargePoint;
    public ParticleSystem walkingLeft;
    public ParticleSystem walkingRight;
    public OrbScale orb;
    public LayerMask targetMask;
    public float interuptionRad;
    public RaycastHit hit;
    public float speedMultiplier;
    public Transform gem;

    private Animator anime;
    public NavMeshAgent agent;

    [HideInInspector]
    public Vector3 actualRayStart;
    public Vector3 offSet;
    Transform tempTransTarget;
    public bool reset;

    private void Awake()
    {
        curHealth = maxHealth;
        anime = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = attackRange;
    }

    private void Update()
    {
        actualRayStart = transform.position + offSet;
        Pillar curPylon = GameManager.instance.pillarMan.pylons[GameManager.instance.pillarMan.curPylon];
        if (curPylon.donePillar && !curPylon.drained)
        {
            chargePoint = curPylon.transform;
        }
        if( curHealth <= 0)
        {
            curBossState = BossState.Dying;
        }
        if(GameManager.gameTime <= 0 && !reset)
        {
            curBossState = BossState.Chasing;
        }
        States();
    }

    public void States()
    {
        switch (curBossState)
        {
            case BossState.Idle:
                ResetAnime();
                anime.SetTrigger("isIdle");
                CheckCharge();
                Collider[] c = Physics.OverlapSphere(actualRayStart + transform.forward, attackRange, targetMask, QueryTriggerInteraction.Ignore);
                for (int i = 0; i < c.Length; i++)
                {
                    if (c[i] != null)
                    {
                        Vector3 rotToTarget = c[i].transform.position - transform.position;
                        Quaternion lookRot = Quaternion.LookRotation(rotToTarget);
                        Vector3 actualRotation = Quaternion.Lerp(transform.rotation, lookRot, agent.angularSpeed * Time.fixedDeltaTime).eulerAngles;
                        transform.rotation = Quaternion.Euler(0, actualRotation.y, 0);
                        if (!IsInvoking("AttackInvoke"))
                        {
                            Invoke("AttackInvoke", 0f);
                        }
                    }
                }
                break;
            case BossState.Chasing:
                CheckCharge();
                Move(playerTarget);
                break;
            case BossState.ObeliskReady:
                CheckRange();
                CheckBlock();
                break;
            case BossState.Blocked:
                if (!IsInvoking("AttackInvoke"))
                {
                    Invoke("AttackInvoke", 0f);
                }
                break;
            case BossState.Draining:
                ResetAnime();
                anime.SetTrigger("isIdle");
                break;
            case BossState.Return:
                CheckCharge();
                Move(midPoint);
                if (!reset)
                {
                    CheckBlock();
                }
                break;
            case BossState.Stunned:
                if (!IsInvoking("Stun"))
                {
                    agent.isStopped = true;
                    Invoke("Stun", attackDelay);
                }
                ResetAnime();
                anime.SetTrigger("isIdle");
                break;
            case BossState.Dying:
                Death();
                break;
            default:
                break;
        }
    }

    private void CheckRange()
    {
        float tempDis = Vector3.Distance(actualRayStart, chargePoint.position);
        if(tempDis < interactRange)
        {
            curBossState = BossState.Draining;
            if (!IsInvoking("Drain"))
            {
                Invoke("Drain", drainTime);
            }
        }
        else
        {
            Move(chargePoint);
        }
    }

    public void Drain()
    {
        damage *= chargePoint.GetComponentInParent<Pillar>().multiplier;
        chargePoint.GetComponentInParent<Pillar>().drained = true;
        try
        {
            orb.gameObject.SetActive(true);
        }
        catch
        {
            orb = Instantiate(GameManager.instance.timeOrb,gem.transform.position,Quaternion.identity);
        }
        orb.StartCoroutine(orb.SizeIncrease());
        agent.speed *= speedMultiplier;
        chargePoint = null;
        CancelInvoke();
    }

    public void AttackInvoke()
    { 
        if (!GameManager.IsPlaying(anime, 0, "Attack"))
        {
            anime.Play("Attack");
            agent.isStopped = true;
        }
    }

    public void HitStun()
    {
        //TODO HitStunAnimation
        curBossState = BossState.Stunned;
    }

    public void Stun()
    {
        curBossState = BossState.Chasing;
    }

    public void AdjustHealth(float i)
    {
        curHealth -= i;
        if (!anime.GetCurrentAnimatorStateInfo(0).IsTag("Attack") && curBossState == BossState.Stunned)
        {
            anime.Play("Hit");
        }
    }

    public void Death()
    {
        //TODO DEATH ANIMATION
        if (!anime.GetCurrentAnimatorStateInfo(0).IsTag("Landing"))
        {
            agent.isStopped = true;
            anime.Play("Landing");
        }

        Destroy(gameObject, anime.GetCurrentAnimatorStateInfo(0).length);
    }

    public void CheckCharge()
    {
        if (chargePoint && curBossState != BossState.Draining)
        {
            curBossState = BossState.ObeliskReady;
        }
    }

    public void CheckBlock()
    {
        Collider[] c = Physics.OverlapSphere(actualRayStart + transform.forward, attackRange, targetMask);
        for (int i = 0; i < c.Length; i++)
        {
            if(c[i].tag =="Player")
            {
                agent.isStopped = true;
                curBossState = BossState.Blocked;
            }
            else
            {
                CheckCharge();
            }
        }
    }

    public void Move(Transform target)
    {
        tempTransTarget = target;
        Vector3 tempRange = new Vector3(actualRayStart.x, 0, actualRayStart.z) + transform.forward;
        Vector3 tempTarget = new Vector3(target.position.x, 0, target.position.z);
        float tempDis = Vector3.Distance(tempRange, tempTarget);
        if (curBossState == BossState.Chasing || curBossState == BossState.ObeliskReady || curBossState == BossState.Return)
        {
            if (tempDis < attackRange)
            {
                tempTransTarget = null;
                if (curBossState == BossState.Return)
                {
                    ResetAnime();
                    anime.SetTrigger("isIdle");
                    curBossState = BossState.Idle;
                    return;
                }
                else if(target.tag == "Player")
                {
                    if (target.GetComponent<PlayerMovement>().inAir && GameManager.gameTime <= 0)
                    {
                        target.transform.position = actualRayStart + transform.forward;
                    }
                    if (!IsInvoking("AttackInvoke"))
                    {
                        Invoke("AttackInvoke", 0f);
                    }
                    return;
                }
            }
            else
            {
                ResetAnime();
                anime.SetTrigger("isChasing");
                if (agent.isStopped)
                {
                    agent.isStopped = false;
                }
                agent.destination = target.position;
            }
        }
    }

    private void ResetAnime()
    {
        anime.ResetTrigger("isIdle");
        anime.ResetTrigger("isChasing");
    }
}
