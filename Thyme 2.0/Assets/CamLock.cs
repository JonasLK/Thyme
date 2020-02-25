using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamLock : MonoBehaviour
{
    public float rotationSpeed;
    public float range;
    public float searchDelay;
    public float lockOnDelay;
    public float disToTarget;
    int currentTarget;
    public List<Transform> nearbyEnemies;
    public LayerMask targets;
    public LayerMask obstacleMask;

    // Update is called once per frame
    void FixedUpdate()
    {
        if(currentTarget > nearbyEnemies.Count - 1 && currentTarget > 0)
        {
            currentTarget = nearbyEnemies.Count - 1;
        }
        if (Input.GetButton("LockOn"))
        {
            Invoke("FindTargets", searchDelay);
            if (nearbyEnemies.Count > 0)
            {
                FindLockOn();
                CheckLockOnInput();
            }
        }
    }

    public void CheckLockOnInput()
    {
        if (Input.GetButtonDown("NextTarget") )
        {
            if(currentTarget < nearbyEnemies.Count - 1)
            {
                currentTarget++;
            }
            else
            {
                currentTarget = 0;
            }
        }
        if (Input.GetButtonDown("PreviousTarget"))
        {
            if(currentTarget > 0)
            {
                currentTarget--;
            }
            else
            {
                currentTarget = nearbyEnemies.Count - 1;
            }
        }
    }

    public void FindTargets()
    {
        Collider[] nearby = Physics.OverlapSphere(transform.position, range, targets);
        for (int i = 0; i < nearby.Length; i++)
        {
            if (!nearbyEnemies.Contains(nearby[i].gameObject.transform))
            {
                nearbyEnemies.Add(nearby[i].gameObject.transform);
                break;
            }
        }
    }

    public void FindLockOn()
    {
        Vector3 dirToTarget = (nearbyEnemies[currentTarget].position - transform.position).normalized;
        float tempDstToTarget = Vector3.Distance(transform.position, nearbyEnemies[currentTarget].position);
        if(tempDstToTarget > range)
        {
            nearbyEnemies.Remove(nearbyEnemies[currentTarget]);
            return;
        }
        if (!Physics.Raycast(transform.position, dirToTarget, tempDstToTarget, obstacleMask) && tempDstToTarget > disToTarget)
        {
            Debug.Log(tempDstToTarget);
            Quaternion dirWeWant = Quaternion.LookRotation(dirToTarget);
            transform.rotation = Quaternion.Lerp(transform.rotation, dirWeWant, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
