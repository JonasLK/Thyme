using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUp : MonoBehaviour
{
    public string pickUpTagName;

    public void OnTriggerEnter(Collider o)
    {
        if (o.gameObject.tag == pickUpTagName)
        {
            o.gameObject.GetComponent<CogPickup>().target = gameObject;
        }
    }
}
