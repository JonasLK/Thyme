using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : MonoBehaviour
{
    public GameObject me;

    public Vector3 juggleForce;

    public float speed;

    public int health;

    public bool inAir;
    public bool hit = false;

    public void GotHit()
    {
        hit = false;
    }

    public void AdjustHealth(int i)
    {
        health -= i;

        if (inAir)
        {
            gameObject.GetComponent<Rigidbody>().velocity = juggleForce;
        }

        if(health <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        Destroy(gameObject);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            inAir = false;
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            inAir = true;
        }
    }
}
