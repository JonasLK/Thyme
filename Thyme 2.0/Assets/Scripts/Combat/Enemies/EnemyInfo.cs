using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : MonoBehaviour
{
    public GameObject me;

    public Chase chase;

    public Vector3 movement;

    public float verticalVel;
    public float gravity = 5f;
    public float launchSpeed = 5f;
    public float juggleForce = 1.5f;

    public float maxHealth;
    public float curHealth;
    public float timeMuliplier;
    public float curSpeedMultiplier;

    public bool inAir;
    public bool gettingLaunched;
    public bool hit = false;

    public void Start()
    {
        me = gameObject;
        curHealth = maxHealth;
        Physics.IgnoreLayerCollision(9, 11);

        chase = gameObject.GetComponent<Chase>();
    }

    private void Update()
    {
        timeMuliplier = curHealth / maxHealth;
        if(timeMuliplier < GameManager.gameTime)
        {
            curSpeedMultiplier = timeMuliplier;
        }
        else
        {
            curSpeedMultiplier = GameManager.gameTime;
        }
        chase.anim.speed = curSpeedMultiplier;
    }

    public void LateUpdate()
    {
        if (inAir)
        {
            verticalVel -= gravity * Time.deltaTime;
            if (verticalVel < 0)
            {
                chase.ResetAnime();
                chase.anim.SetTrigger("isFalling");
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
            PlayAnime("HitInAir");
        }
        else
        {
            gameObject.transform.Translate(movement * Time.deltaTime * curSpeedMultiplier, Space.World);
        }

    }

    public void GotHit()
    {
        hit = false;
    }

    public void AdjustHealth(float i, bool launch)
    {
        curHealth -= i;
        PlayAnime("Hit");
        chase.Hit();
        gettingLaunched = launch;

        if (inAir)
        {
            verticalVel = juggleForce;
            //TODO GettingLaunched in a andere manier zetten
        }

        if(curHealth <= 0)
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
            if (inAir)
            {
                PlayAnime("Landing");
            }
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
    public void PlayAnime(string animeName)
    {
        chase.anim.Play(animeName);
    }
}
