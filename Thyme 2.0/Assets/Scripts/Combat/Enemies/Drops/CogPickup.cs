using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CogPickup : MonoBehaviour
{
    //maybe add particles on pickup
    public GameObject target;
    public int value;
    public float speed;
    public float pickUpDistance;
    private GameObject canvas;
    private DestroyEmpty parent;

    public void Start()
    {
        parent = gameObject.GetComponentInParent<DestroyEmpty>();
    }

    public void Update()
    {
        if(target != null)
        {
            transform.LookAt(target.transform);
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            if (Vector3.Distance(gameObject.transform.position, target.transform.position) <= pickUpDistance)
            {
                canvas = GameObject.FindGameObjectWithTag("Canvas");
                canvas.GetComponent<Cogs>().UpdateCogValuePlus(value);
                canvas.GetComponent<Cogs>().UpdateCogText();
                parent.CheckIfEmpty(gameObject);
                Destroy(gameObject);
            }
        }
    }

    public void RandomizeValue(int randomValue)
    {
        value = randomValue;
    }
}