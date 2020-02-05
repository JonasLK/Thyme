using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CogPickup : MonoBehaviour
{
    //attach empty with sphere collider to player, set trigger, give empty tag \/\/\/\/\/, set nameOfEmptyRigidbodyTag to that name
    public string nameOfEmptyRigidbodyTag;
    //make cogs move to player, on collision add coins and destroy, maybe add particles
    public GameObject player;
    public GameObject target;
    public float speed;

    void Start()
    {
        player =  gameObject.GetComponentInParent<Transform>().gameObject;
    }

    public void Update()
    {
        if(target != null)
        {
            transform.LookAt(target.transform);
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    public void OnTriggerEnter(Collider c)
    {
        if(c.gameObject.tag == nameOfEmptyRigidbodyTag)
        {
            target = player;
        }
    }

    public void OnCollisionEnter(Collision c)
    {
        if(c.gameObject == player)
        {
            //add coins
            Destroy(gameObject);
        }
    }
}
// maybe add a spinning animation on loop