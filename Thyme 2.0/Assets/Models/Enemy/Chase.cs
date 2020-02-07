using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : MonoBehaviour
{
    public float range;
    public float speed;
    public float angel;
    public float attackRange;
    public Transform player;
    public Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = player.position - transform.position;
        float angle = Vector3.Angle(dir,transform.forward);
        if (Vector3.Distance(player.position, transform.position) < range && angle < angel)
        {
            dir.y = 0;

            transform.rotation = Quaternion.Slerp(transform.rotation,
                                     Quaternion.LookRotation(dir), 0.1f);
            if(dir.magnitude > attackRange)
            {
                transform.Translate(0, 0, speed);
                ResetAnime();
                anim.SetTrigger("isChasing");
            }
            else
            {
                ResetAnime();
                anim.SetTrigger("isAttacking");
            }
          
        }
        else
        {
            ResetAnime();
            anim.SetTrigger("isIdle");
        }

    }

    public void ResetAnime()
    {
        anim.ResetTrigger("isIdle");
        anim.ResetTrigger("isChasing");
        anim.ResetTrigger("isAttacking");
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
