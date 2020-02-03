using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoesAttack : MonoBehaviour
{
    public List<GameObject> enemies;

    public Vector3 launchForce;

    public bool didAttack;

    public void DoDamage(Slash slash)
    {
        if(enemies != null)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (!enemies[i].GetComponent<EnemyInfo>().hit)
                {
                    enemies[i].GetComponent<EnemyInfo>().hit = true;
                    enemies[i].GetComponent<EnemyInfo>().AdjustHealth(slash.damage);

                    if (slash.launchAttack)
                    {
                        enemies[i].GetComponent<EnemyInfo>().verticalVel = launchForce.y;
                        enemies[i].GetComponent<EnemyInfo>().inAir = true;
                        enemies[i].GetComponent<EnemyInfo>().gettingLaunched = true;
                    }
                }

                if (enemies[i].GetComponent<EnemyInfo>().health <= 0)
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
        if(c.gameObject.tag == "Enemy")
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
