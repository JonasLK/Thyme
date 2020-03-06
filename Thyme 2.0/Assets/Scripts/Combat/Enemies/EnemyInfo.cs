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
    public float fallMultiplier = 5f;
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
        rb = GetComponent<Rigidbody>();
        chase = gameObject.GetComponent<Chase>();
    }

    private void Update()
    {
        if(curHealth > 0 && chase.curState != Chase.State.Dying)
        {
            timeMuliplier = curHealth / maxHealth;
        }
        else
        {
            timeMuliplier = 0.1f;
        }
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

        if(inAir && !launch)
        {
            rb.velocity = Vector3.zero;
        }

        if(curHealth <= 0)
        {
            chase.curState = Chase.State.Dying;
        }
    }

    public void ChangeVel(float power)
    {
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
        Destroy(gameObject);
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
                    inAir = false;
                }
                else if (chase.curState != Chase.State.Dying)
                {
                    ChangeVel(tempVel);
                    chase.curState = Chase.State.Falling;
                }
            }
            if(chase.curState == Chase.State.Dying)
            {
                PlayAnime("Landing");
                Invoke("Death", chase.anim.GetCurrentAnimatorStateInfo(0).length);
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
