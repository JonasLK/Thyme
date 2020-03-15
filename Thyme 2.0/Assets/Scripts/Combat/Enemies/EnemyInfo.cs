using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : MonoBehaviour
{
    public GameObject me;

    public Chase chase;
    public Rigidbody rb;

    public Vector3 movement;

    public Vector3 velocity;
    float tempVel;
    public float fallMultiplier = 0.25f;
    public float launchSpeed = 0.5f;
    public float juggleForce = 0.85f;

    public float maxHealth = 100;
    public float curHealth;
    public float timeMuliplier;
    public float curSpeedMultiplier;
    public float minimumSpeedTreshhold = 0.1f;
    public float deathDelay = 1.5f;

    public bool inAir;
    public bool gettingLaunched;
    public bool hit = false;

    public void Start()
    {
        me = gameObject;
        curHealth = maxHealth;
        Physics.IgnoreLayerCollision(9, 11);
        rb = GetComponent<Rigidbody>();
        chase = gameObject.GetComponent<Chase>();
        curSpeedMultiplier = 1f;
    }

    public void Update()
    {
        CheckSpeed();
    }

    public virtual void CheckSpeed()
    {
        if (curHealth > 0 && chase.curState != Chase.State.Dying)
        {
            timeMuliplier = curHealth / maxHealth;
        }
        else
        {
            timeMuliplier = minimumSpeedTreshhold;
            curSpeedMultiplier = minimumSpeedTreshhold;
        }
        if (timeMuliplier <= curSpeedMultiplier && chase.curState != Chase.State.SlowingDown)
        {
            curSpeedMultiplier = timeMuliplier;
        }
        chase.anim.speed = curSpeedMultiplier;
    }

    public void FixedUpdate()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        if (inAir)
        {
            if (rb.velocity.y < 0 && chase.curState != Chase.State.Bounce)
            {
                chase.ResetAnime();
                chase.anim.SetTrigger("isFalling");
                gettingLaunched = false;
            }
        }
        else if (chase.curState != Chase.State.Bounce)
        {
            velocity = Vector3.zero;
        }
    }

    public void GotHit()
    {
        hit = false;
    }

    public virtual void AdjustHealth(float i, bool launch)
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

        if(inAir && !launch)
        {
            rb.velocity = Vector3.zero;
        }

        if (curHealth <= 0)
        {
            chase.curState = Chase.State.Dying;
            Invoke("Death", 0f);
        }
    }

    public virtual void ChangeVel(float power)
    {
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        rb.isKinematic = false;
        if(power > 0)
        {
            GetComponent<EnemyInfo>().inAir = true;
        }
        else
        {
            chase.curState = Chase.State.Bounce;
        }
        tempVel = -power * 0.5f;
        if(chase.curState != Chase.State.Bounce)
        {
            rb.velocity = Vector3.zero;
            rb.velocity = Vector3.up * -power * Physics.gravity.y;
            gettingLaunched = true;
        }
        else
        {
            rb.velocity = Vector3.zero;
            rb.velocity = Vector3.up * -power * Physics.gravity.y;
        }
    }

    public void Death()
    {
        if (!inAir)
        {
            if (!chase.anim.GetCurrentAnimatorStateInfo(0).IsTag("Landing"))
            {
                PlayAnime("Landing");
            }
            Destroy(gameObject, chase.anim.GetCurrentAnimatorStateInfo(0).length);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (inAir)
            {
                if (chase.curState != Chase.State.Bounce && chase.curState != Chase.State.Dying)
                {
                    PlayAnime("Landing");
                    GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
                    rb.isKinematic = true;
                }
                else if (chase.curState != Chase.State.Dying)
                {
                    ChangeVel(tempVel);
                    chase.curState = Chase.State.Falling;
                }
                inAir = false;
            }
            if(chase.curState == Chase.State.Dying)
            {
                PlayAnime("Landing");
                Invoke("Death", 0f);
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
