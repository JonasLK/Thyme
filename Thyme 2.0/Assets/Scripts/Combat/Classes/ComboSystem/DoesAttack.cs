using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoesAttack : MonoBehaviour
{
    public ComboHolder comboHolder;
    public Vector3 range;
    public float rangeToMove;
    public bool didAttack, ableToMove = true;
    public float forwardSpeed = 10f;
    public LayerMask enemyLayer;

    public void DoDamage(Slash slash)
    {
        if (slash.chargeTimer > slash.chargeMax)
        {
            slash.chargeTimer = slash.chargeMax;
        }

        Collider[] enemies = Physics.OverlapBox(transform.position, range, Quaternion.identity, enemyLayer);

        if(AbleToMoveCheck(rangeToMove))
        {
            Debug.Log("works");
            //comboHolder.player.transform.Translate(comboHolder.playerMovement.actualPlayer.transform.forward * slash.forwardMov * forwardSpeed * GameManager.gameTime * Time.fixedDeltaTime);
        }

        comboHolder.GetComponent<PlayerMovement>().AddVel(slash.launchForce.y * slash.chargeTimer);

        Debug.Log("enemies");
        foreach (Collider allenemies in enemies)
        {
            if (allenemies.tag == "Enemy")
            {
                if (slash.launchAttack)
                {
                    if (allenemies.GetComponent<EnemyInfo>().inAir && slash.launchForce.y < 0)
                    {
                        allenemies.GetComponent<EnemyInfo>().ChangeVel(slash.launchForce.y * slash.chargeTimer);
                        GetComponentInParent<PlayerMovement>().AddVel(slash.launchForce.y * slash.chargeTimer);
                    }
                    else
                    {
                        allenemies.GetComponent<EnemyInfo>().ChangeVel(slash.launchForce.y * slash.chargeTimer);
                        GetComponentInParent<PlayerMovement>().AddVel(slash.launchForce.y * slash.chargeTimer);
                    }
                }
                allenemies.GetComponent<EnemyInfo>().hit = true;
                allenemies.GetComponent<EnemyInfo>().AdjustHealth(slash.damage * slash.chargeTimer, slash.launchAttack);
            }
            else if (allenemies.tag == "Boss")
            {
                allenemies.GetComponent<BossInfo>().AdjustHealth(slash.damage * slash.chargeTimer);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, range);
    }

    public bool AbleToMoveCheck(float r)
    {
        RaycastHit hit;
        if(Physics.Raycast(comboHolder.playerMovement.actualPlayer.transform.position, comboHolder.playerMovement.actualPlayer.transform.forward, out hit, r))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
