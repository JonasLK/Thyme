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
    public Transform target;
    public Transform chargePoint;
    public ParticleSystem walkingLeft;
    public ParticleSystem walkingRight;
    public OrbScale orb;
    public LayerMask targetMask;
    public Vector3 offset;
    public float interuptionRad;
    public RaycastHit hit;
    public float speedMultiplier;

    private Animator anime;
    public NavMeshAgent agent;

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
        Pillar curPylon = GameManager.instance.pillarMan.pylons[GameManager.instance.pillarMan.curPylon];
        if (curPylon.donePillar && !curPylon.drained)
        {
            chargePoint = curPylon.transform;
        }
        actualRayStart = transform.position + offset;
        if( curHealth <= 0)
        {
            curBossState = BossState.Dying;
        }
        if(GameManager.gameTime <= 0)
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
                CheckCharge();
                float tempDis = Vector3.Distance(actualRayStart, target.transform.position);
                Debug.Log(tempDis);
                if (tempDis < attackRange)
                {
                    curBossState = BossState.Chasing;
                }
                break;
            case BossState.Chasing:
                CheckCharge();
                Move(target);
                CheckIdle();
                break;
            case BossState.ObeliskReady:
                CheckRange();
                Invoke("CheckBlock", 0.2f);
                break;
            case BossState.Blocked:
                if (!nextAttack)
                {
                    CancelInvoke("CheckBlock");
                    Invoke("AttackInvoke", 0f);
                    nextAttack = true;
                }
                break;
            case BossState.Draining:
                CancelInvoke("CheckBlock");
                ResetAnime();
                anime.SetTrigger("isIdle");
                break;
            case BossState.Return:
                Move(midPoint);
                break;
            case BossState.Stunned:
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
        Debug.Log(tempDis);
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
        orb.gameObject.SetActive(true);
        orb.StartCoroutine(orb.SizeIncrease());
        agent.speed *= speedMultiplier;
        chargePoint = null;
        CancelInvoke();
    }

    public void AttackInvoke()
    {
        PlayAnime("Attack");
        agent.isStopped = true;
    }

    public void HitStun()
    {
        //TODO HitStunAnimation
        curBossState = BossState.Stunned;
        if (!IsInvoking("Stun"))
        {
            Invoke("Stun", attackDelay);
        }
    }

    public void Stun()
    {
        CheckCharge();
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
        if (!anime.GetCurrentAnimatorStateInfo(0).IsTag("Landing"))
        {
            PlayAnime("Landing");
        }
        Destroy(gameObject, anime.GetCurrentAnimatorStateInfo(0).length);
    }

    public void CheckCharge()
    {
        if(chargePoint && curBossState == BossState.Stunned)
        {
            CheckBlock();
        }
        if (chargePoint && curBossState != BossState.Draining)
        {
            curBossState = BossState.ObeliskReady;
        }
        if(!chargePoint && curBossState != BossState.Chasing && curBossState != BossState.Idle)
        {
            curBossState = BossState.Return;
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
        Vector3 tempRange = new Vector3(actualRayStart.x, 0, actualRayStart.z);
        Vector3 tempTarget = new Vector3(target.position.x, 0, target.position.z);
        float tempDis = Vector3.Distance(tempRange, tempTarget);
        if (curBossState == BossState.Chasing || curBossState == BossState.ObeliskReady || curBossState == BossState.Return)
        {
            if (tempDis < attackRange && curBossState == BossState.Return)
            {
                ResetAnime();
                anime.SetTrigger("isIdle");
                curBossState = BossState.Idle;
                return;
            }

            if (tempDis < attackRange && curBossState != BossState.Return)
            {
                Invoke("AttackInvoke", 0f);
                return;
            }

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
