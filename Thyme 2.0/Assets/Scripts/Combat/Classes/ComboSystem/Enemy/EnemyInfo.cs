using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : MonoBehaviour
{
    public GameObject me;

    public Vector3 movement;

    public float verticalVel;
    public float gravity = 5f;
    public float launchSpeed = 5f;
    public float juggleForce = 1.5f;

    public float health;

    public bool inAir;
    public bool gettingLaunched;
    public bool hit = false;

    public void Start()
    {
        me = gameObject;

        Physics.IgnoreLayerCollision(9, 11);
    }

    public void LateUpdate()
    {
        if (inAir)
        {
            verticalVel -= gravity * Time.deltaTime;

            if (verticalVel < 0)
            {
                gettingLaunched = false;
            }
        }
        else
        {
            verticalVel = 0;
        }
    }

    public void FixedUpdate()
    {
        movement = new Vector3 (0, verticalVel, 0);

        if (gettingLaunched)
        {
            gameObject.transform.Translate(movement * Time.deltaTime * launchSpeed, Space.World);
        }
        else
        {
            gameObject.transform.Translate(movement * Time.deltaTime, Space.World);
        }

    }

    public void GotHit()
    {
        hit = false;
    }

    public void AdjustHealth(int i)
    {
        health -= i;

        gettingLaunched = true;

        if (inAir)
        {
            verticalVel = juggleForce;
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
