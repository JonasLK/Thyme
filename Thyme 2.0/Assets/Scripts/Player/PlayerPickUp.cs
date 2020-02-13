using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUp : MonoBehaviour
{
    public float tymeCharge;
    public float amountOfTymeCrystals;
    public float portalCloseCost; //amount of tymecharge needed to fill 1 crystal
    public List<GameObject> tymeCrystals;
    public string pickUpTagName;

    public void Start()
    {
        tymeCrystals[0].transform.localScale = new Vector3(tymeCrystals[0].transform.localScale.x, 0, tymeCrystals[0].transform.localScale.z);
        tymeCrystals[1].transform.localScale = new Vector3(tymeCrystals[1].transform.localScale.x, 0, tymeCrystals[1].transform.localScale.z);
        tymeCrystals[2].transform.localScale = new Vector3(tymeCrystals[2].transform.localScale.x, 0, tymeCrystals[2].transform.localScale.z);
    }

    public void OnTriggerEnter(Collider o)
    {
        if (o.gameObject.tag == pickUpTagName)
        {
            o.gameObject.GetComponent<CogPickup>().target = gameObject;
        }
    }

    public void Update()
    {
        if(tymeCharge > tymeCrystals.Capacity * portalCloseCost)
        {
            tymeCharge = tymeCrystals.Capacity * portalCloseCost;
        }

        if(tymeCharge <= portalCloseCost)
        {
            tymeCrystals[0].transform.localScale = new Vector3(tymeCrystals[0].transform.localScale.x, tymeCharge/portalCloseCost, tymeCrystals[0].transform.localScale.z);
            tymeCrystals[1].transform.localScale = new Vector3(tymeCrystals[1].transform.localScale.x, 0, tymeCrystals[1].transform.localScale.z);
            tymeCrystals[2].transform.localScale = new Vector3(tymeCrystals[2].transform.localScale.x, 0, tymeCrystals[2].transform.localScale.z);
        }
        else if(tymeCharge > portalCloseCost && tymeCharge <= (portalCloseCost*2))
        {
            tymeCrystals[0].transform.localScale = new Vector3(tymeCrystals[0].transform.localScale.x, 1, tymeCrystals[0].transform.localScale.z);
            tymeCrystals[1].transform.localScale = new Vector3(tymeCrystals[1].transform.localScale.x, (tymeCharge-portalCloseCost)/portalCloseCost, tymeCrystals[1].transform.localScale.z);
            tymeCrystals[2].transform.localScale = new Vector3(tymeCrystals[2].transform.localScale.x, 0, tymeCrystals[2].transform.localScale.z);
        }
        else if(tymeCharge > portalCloseCost * 2)
        {
            tymeCrystals[0].transform.localScale = new Vector3(tymeCrystals[0].transform.localScale.x, 1, tymeCrystals[0].transform.localScale.z);
            tymeCrystals[1].transform.localScale = new Vector3(tymeCrystals[1].transform.localScale.x, 1, tymeCrystals[1].transform.localScale.z);
            tymeCrystals[2].transform.localScale = new Vector3(tymeCrystals[2].transform.localScale.x, (tymeCharge - portalCloseCost*2)/portalCloseCost, tymeCrystals[2].transform.localScale.z);
        }
    }
}