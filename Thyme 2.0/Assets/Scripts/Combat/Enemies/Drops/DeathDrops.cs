using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathDrops: MonoBehaviour
{
    public string playerTagNameHere;
    public int maxMoneyDrop;
    public int minMoneyDrop;
    public int tymeChargeAmount, tymeChargeAmountMin, tymeChargeAmountMax;
    public int cogValueSelector;
    public int cogAmount, cogAmountMin, cogAmountMax;
    public GameObject[] cogs;

    void Start()
    {
        //DeathDrop(gameObject);//just for testing, just remove it if needed
    }
    
    public void DeathDrop()
    {
        cogAmount = Random.Range(cogAmountMin, cogAmountMax);
        cogValueSelector = Random.Range(minMoneyDrop, maxMoneyDrop);
        tymeChargeAmount = Random.Range(tymeChargeAmountMin, tymeChargeAmountMax);
        GameObject.FindGameObjectWithTag(playerTagNameHere).GetComponent<PlayerPickUp>().tymeCharge += tymeChargeAmount;
        GameObject cogToChangeValue = Instantiate(cogs[cogAmount], transform.position, Quaternion.identity);
        foreach(GameObject cog in cogToChangeValue.GetComponent<DestroyEmpty>().cogs)
        {
            cog.GetComponent<CogPickup>().RandomizeValue(cogValueSelector);
        }
    }

    public void Update()//this whole update void is for testing purposes only, Remove if needed
    {
        if (Input.GetKeyDown("p"))
        {
            //DeathDrop(gameObject);
        }

        if (Input.GetKeyDown("o"))
        {
            GameObject.FindGameObjectWithTag(playerTagNameHere).GetComponent<PlayerPickUp>().tymeCharge -= 2.5f;
        }
    }
}