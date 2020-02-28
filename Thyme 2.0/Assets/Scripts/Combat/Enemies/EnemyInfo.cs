using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : MonoBehaviour
{
    public GameObject me;

    public Chase chase;

    public Vector3 movement;

    public Vector3 velocity;
    Vector3 tempVel;
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
            velocity.y -= gravity * Time.deltaTime;
            if (velocity.y < 0 && chase.curState != Chase.State.Bounce)
            {
                chase.ResetAnime();
                chase.anim.SetTrigger("isFalling");
                gettingLaunched = false;
            }
        }
        else if (chase.curState != Chase.State.Bounce)
        {
            velocity.y = 0;
        }
    }

    public void FixedUpdate()
    {
        movement = velocity;

        if (gettingLaunched)
        {
            gameObject.transform.Translate(movement * Time.deltaTime * launchSpeed, Space.World);
            PlayAnime("HitInAir");
        }
        else
        {
            gameObject.transform.Translate(movement * Time.deltaTime * GameManager.gameTime , Space.World);
        }

    }

    public void GotHit()
    {
        hit = false;
    }

    public void AdjustHealth(float i, bool launch)
    {
        curHealth -= i;

        if (!chase.anim.GetCurrentAnimatorStateInfo(0).IsTag("Landing"))
        {
            if (inAir)
            {
                PlayAnime("HitInAir");
            }
            else
            {
                PlayAnime("Hit");
            }
        }

        chase.Hit();

        gettingLaunched = launch;

        if (inAir)
        {
            velocity.y = juggleForce;
            //TODO GettingLaunched in a andere manier zetten
        }

        if(curHealth <= 0)
        {
            Death();
        }
    }

    public void ChangeVel(Vector3 force)
    {
        tempVel = force;
        velocity = force;
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
                if (chase.curState != Chase.State.Bounce)
                {
                    PlayAnime("Landing");
                    GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
                    GetComponent<Rigidbody>().isKinematic = true;
                    GetComponent<Rigidbody>().useGravity = true;
                    inAir = false;
                }
                else
                {
                    ChangeVel(-tempVel);
                    Debug.Log(-tempVel);
                    chase.curState = Chase.State.Falling;
                    tempVel = Vector3.zero;
                }
            }
        }
    }

    //public void OnCollisionExit(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Ground"))
    //    {
    //        inAir = true;
    //    }
    //}
    public void PlayAnime(string animeName)
    {
        chase.anim.Play(animeName);
    }
}
