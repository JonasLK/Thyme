using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : MonoBehaviour
{
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
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(FindTargetsWithDelay(delay));
    }

    private void CheckPos(float dis)
    {
        if(dis > attackRange)
        {
            transform.Translate(new Vector3(0, 0, speed) * Time.fixedDeltaTime);
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
            if(Vector3.Angle(transform.forward, dirToTarget) < viewAngle * 0.5f)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation,
                                    Quaternion.LookRotation(dirToTarget), rotateSpeed * Time.fixedDeltaTime);
                    CheckPos(dstToTarget);
                    
                }
                else
                {
                    ResetAnime();
                    anim.SetTrigger("isIdle");
                }
            }
        }
        if(targetsInVieuwRadius.Length == 0)
        {
            ResetAnime();
            anim.SetTrigger("isIdle");
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
