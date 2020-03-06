using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoesAttack : MonoBehaviour
{
    public ComboHolder comboHolder;
    public Vector3 range;
    public bool didAttack;
    public LayerMask enemyLayer;

    public void DoDamage(Slash slash)
    {
        if(slash.chargeTimer > slash.chargeMax)
        {
            slash.chargeTimer = slash.chargeMax;
        }

        Collider[] enemies = Physics.OverlapBox(transform.position, range,Quaternion.identity,enemyLayer);

        comboHolder.GetComponent<PlayerMovement>().AddVel(slash.launchForce.y * slash.chargeTimer);

            Debug.Log("enemies");
        foreach (Collider allenemies in enemies)
        {
            if (!allenemies.GetComponent<EnemyInfo>().hit)
            {
                if (slash.launchAttack)
                {
                    allenemies.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
                    allenemies.GetComponent<Rigidbody>().isKinematic = false;

                    if (allenemies.GetComponent<EnemyInfo>().inAir && slash.launchForce.y < 0)
                    {
                        allenemies.GetComponent<Chase>().curState = Chase.State.Bounce;
                        allenemies.GetComponent<EnemyInfo>().ChangeVel(slash.launchForce.y * slash.chargeTimer);
                        GetComponentInParent<PlayerMovement>().AddVel(slash.launchForce.y * slash.chargeTimer);
                    }
                    else
                    {
                        allenemies.GetComponent<EnemyInfo>().ChangeVel(slash.launchForce.y*slash.chargeTimer);
                        allenemies.GetComponent<EnemyInfo>().inAir = true;
                        GetComponentInParent<PlayerMovement>().AddVel(slash.launchForce.y * slash.chargeTimer);
                    }
                }
                allenemies.GetComponent<EnemyInfo>().hit = true;
                allenemies.GetComponent<EnemyInfo>().AdjustHealth(slash.damage, slash.launchAttack);
            }
            allenemies.GetComponent<EnemyInfo>().GotHit();
        }
        didAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, range);
    }
}
