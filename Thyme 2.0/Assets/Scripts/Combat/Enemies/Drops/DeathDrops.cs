using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathDrops: MonoBehaviour
{
    public int maxMoneyDrop;
    public int minMoneyDrop;
    public int timeCrystalChargeAmount;
    public int cogValueSelector;
    public int cogAmount;
    public int cogAmountMax;
    public GameObject[] cogs;
    public GameObject cogValue;

    void Start()
    {
        DeathDrop(gameObject);//just for testing, just remove it if needed
    }
    
    public void DeathDrop(GameObject attachedEnemy)
    {
        cogAmount = Random.Range(0, cogAmountMax);
        cogValueSelector = Random.Range(minMoneyDrop, maxMoneyDrop);
        GameObject cogToChangeValue = Instantiate(cogs[cogAmount], attachedEnemy.transform.position, Quaternion.identity);
        foreach(GameObject cog in cogToChangeValue.GetComponent<DestroyEmpty>().cogs)
        {
            cog.GetComponent<CogPickup>().RandomizeValue(cogValueSelector);
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            DeathDrop(gameObject);
        }
    }
}