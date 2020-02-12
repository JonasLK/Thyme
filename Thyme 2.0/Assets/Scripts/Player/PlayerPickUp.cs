using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUp : MonoBehaviour
{
    public float test;
    public float tymeCharge;
    public float amountOfTymeCrystals;
    public float portalCloseCost; //amount of tymecharge needed to fill 1 crystal
    public GameObject tymeCrystal1, tymeCrystal2, tymeCrystal3;
    public string pickUpTagName;

    public void Start()
    {
        tymeCrystal1.transform.localScale = new Vector3(tymeCrystal1.transform.localScale.x, 0, tymeCrystal1.transform.localScale.z);
        tymeCrystal2.transform.localScale = new Vector3(tymeCrystal2.transform.localScale.x, 0, tymeCrystal2.transform.localScale.z);
        tymeCrystal3.transform.localScale = new Vector3(tymeCrystal3.transform.localScale.x, 0, tymeCrystal3.transform.localScale.z);
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
        if(tymeCharge > amountOfTymeCrystals * portalCloseCost)
        {
            tymeCharge = amountOfTymeCrystals * portalCloseCost;
        }

        if(tymeCharge <= portalCloseCost)
        {
            test = tymeCharge/portalCloseCost;
            tymeCrystal1.transform.localScale = new Vector3(tymeCrystal1.transform.localScale.x, tymeCharge/portalCloseCost, tymeCrystal1.transform.localScale.z);
            tymeCrystal2.transform.localScale = new Vector3(tymeCrystal2.transform.localScale.x, 0, tymeCrystal2.transform.localScale.z);
            tymeCrystal3.transform.localScale = new Vector3(tymeCrystal3.transform.localScale.x, 0, tymeCrystal3.transform.localScale.z);
        }
        else if(tymeCharge > portalCloseCost && tymeCharge <= (portalCloseCost*2))
        {
            tymeCrystal1.transform.localScale = new Vector3(tymeCrystal1.transform.localScale.x, 1, tymeCrystal1.transform.localScale.z);
            tymeCrystal2.transform.localScale = new Vector3(tymeCrystal2.transform.localScale.x, (tymeCharge-portalCloseCost)/portalCloseCost, tymeCrystal2.transform.localScale.z);
            tymeCrystal3.transform.localScale = new Vector3(tymeCrystal3.transform.localScale.x, 0, tymeCrystal3.transform.localScale.z);
        }
        else if(tymeCharge > portalCloseCost * 2)
        {
            tymeCrystal1.transform.localScale = new Vector3(tymeCrystal1.transform.localScale.x, 1, tymeCrystal1.transform.localScale.z);
            tymeCrystal2.transform.localScale = new Vector3(tymeCrystal2.transform.localScale.x, 1, tymeCrystal2.transform.localScale.z);
            tymeCrystal3.transform.localScale = new Vector3(tymeCrystal3.transform.localScale.x, (tymeCharge - portalCloseCost*2)/portalCloseCost, tymeCrystal3.transform.localScale.z);
        }
    }
}