using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathDrops: MonoBehaviour
{
    public int maxMoneyDrop;
    public int minMoneyDrop;
    public int timeCrystalChargeAmount;
    public GameObject cogs;

    void Start()
    {
        
    }
    
    public void DeathDrop(GameObject attachedEnemy)
    {
        Instantiate(cogs, attachedEnemy.transform.position, Quaternion.identity);

    }
}