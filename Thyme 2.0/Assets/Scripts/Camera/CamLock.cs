using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraMode
{
    Normal,
    Lockon
}

public class CamLock : MonoBehaviour
{
    public CameraMode camMode = CameraMode.Normal;
    public float rotationSpeed;
    public float range;
    public float searchDelay;
    public float lockOnDelay;
    public float disToTarget;
    int currentTarget;
    public List<Transform> nearbyEnemies;
    public LayerMask targets;
    public LayerMask obstacleMask;
    public bool boss;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.instance.uiMan.tutorial.activeSelf == true)
        {
            return;
        }
        if (currentTarget > nearbyEnemies.Count - 1 && currentTarget > 0)
        {
            currentTarget = nearbyEnemies.Count - 1;
        }
        if(currentTarget < 0)
        {
            currentTarget = 0;
        }
        
        switch (camMode)
        {
            case CameraMode.Normal:
                CheckInput();
                if (GetComponentInChildren<CameraRotation>().CheckInput())
                {
                    GetComponentInChildren<CameraRotation>().CameraMovement();
                }
                else
                {
                    GetComponentInChildren<CameraRotation>().CameraMovement();
                }
                break;
            case CameraMode.Lockon:
                CheckInput();
                if (boss)
                {
                    if (!IsInvoking("FindLockOn"))
                    {
                        InvokeRepeating("FindLockOn", 0f, searchDelay);
                    }
                }
                else
                {
                    FindAndLock();
                    CheckLockOnInput();
                }
                break;
            default:
                break;
        }
    }

    public void CheckInput()
    {
        if (Input.GetButtonDown("LockOn"))
        {
            if (camMode == CameraMode.Normal)
            {
                camMode = CameraMode.Lockon;
            }
            else
            {
                camMode = CameraMode.Normal;
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

    public void FindAndLock()
    {
        FindTargets();
        if (nearbyEnemies.Count <= 0)
        {
            camMode = CameraMode.Normal;
            return;
        }
        if (!IsInvoking("FindLockOn") && nearbyEnemies.Count > 0)
        {
            InvokeRepeating("FindLockOn", 0f,searchDelay);
        }
    }

    public void FindTargets()
    {
        Collider[] nearby = Physics.OverlapSphere(transform.position, range, targets);
        for (int i = 0; i < nearby.Length; i++)
        {
            if (!nearbyEnemies.Contains(nearby[i].gameObject.transform) && Vector3.Distance(transform.position, nearby[i].transform.position) > disToTarget)
            {
                nearbyEnemies.Add(nearby[i].gameObject.transform);
                break;
            }
        }
    }

    public void FindLockOn()
    {
        if (!nearbyEnemies[currentTarget])
        {
            return;
        }
        Vector3 dirToTarget = (nearbyEnemies[currentTarget].position - transform.position).normalized;
        float tempDstToTarget = Vector3.Distance(transform.position, nearbyEnemies[currentTarget].position);
        if(tempDstToTarget > range && !boss)
        {
            nearbyEnemies.Remove(nearbyEnemies[currentTarget]);
            return;
        }
        if (!Physics.Raycast(transform.position, dirToTarget, tempDstToTarget, obstacleMask) && tempDstToTarget > disToTarget || boss)
        {
            Quaternion dirWeWant = Quaternion.LookRotation(dirToTarget);
            Vector3 rotate = Quaternion.Lerp(transform.rotation, dirWeWant, rotationSpeed * Time.fixedDeltaTime).eulerAngles;
            transform.rotation = Quaternion.Euler(rotate.x, rotate.y, 0);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
