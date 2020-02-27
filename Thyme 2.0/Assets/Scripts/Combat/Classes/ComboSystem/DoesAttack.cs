using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoesAttack : MonoBehaviour
{
    public ComboHolder comboHolder;

    public List<GameObject> enemies;

    public bool didAttack;

    public void DoDamage(Slash slash)
    {
        if (enemies != null)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (!enemies[i].GetComponent<EnemyInfo>().hit)
                {
                    enemies[i].GetComponent<EnemyInfo>().hit = true;
                    enemies[i].GetComponent<EnemyInfo>().AdjustHealth(slash.damage, slash.launchAttack);

                    if (slash.launchAttack)
                    {
                        enemies[i].GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
                        enemies[i].GetComponent<Rigidbody>().isKinematic = false;
                        enemies[i].GetComponent<Rigidbody>().useGravity = false;

                        if(slash.chargeTimer > slash.chargeMax)
                        {
                            slash.chargeTimer = slash.chargeMax;
                        }

                        enemies[i].GetComponent<EnemyInfo>().velocity = new Vector3(slash.launchForce.x, slash.launchForce.y * slash.chargeTimer, slash.launchForce.z);
                        enemies[i].GetComponent<EnemyInfo>().inAir = true;
                        enemies[i].GetComponent<EnemyInfo>().gettingLaunched = true;
                    }
                }

                if (enemies[i].GetComponent<EnemyInfo>().curHealth <= 0)
                {
                    enemies.Remove(enemies[i]);
                    DoDamage(slash);
                    break;
                }
            }

            for(int i = 0; i < enemies.Count; i++)
            {
                enemies[i].GetComponent<EnemyInfo>().GotHit();
            }

            didAttack = true;
        }     
    }

    public void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "Enemy")
        {
            enemies.Add(c.gameObject);
        }
    }

    public void OnTriggerExit(Collider c)
    {
        if(c.gameObject.tag == "Enemy")
        {
            enemies.Remove(c.gameObject);
        }
    }
}
