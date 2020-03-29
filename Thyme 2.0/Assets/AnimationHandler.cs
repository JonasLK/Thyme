using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    [HideInInspector]
    public float chargeMultiplier;
    public float bossForce;
    public float bossUpForce;

    //Player
    public void SwordParticle(int swordParticle)
    {
        if (swordParticle == 1)
        {
            GameManager.instance.particleMan.swordSlash.Play();
        }
        else
        {
            GameManager.instance.particleMan.swordSlash.Stop();
        }
    }

    public void Damage()
    {
        GetComponentInChildren<DoesAttack>().DoDamage(GetComponentInParent<ComboHolder>().curSlash);
    }

    public void WalkingParticle(string currentParticle)
    {
        if (currentParticle == GameManager.instance.particleMan.leftFootPlayer)
        {
            AudioClip randomClip = GameManager.instance.soundMan.GetComponent<StepList>().leftFoot[GameManager.instance.soundMan.GetComponent<StepList>().Randomizer()];
            GameManager.instance.soundMan.ChangeClip("Walk", randomClip);
            GameManager.instance.soundMan.Play("Walk");
            GameManager.instance.particleMan.leftFootGround.Play();
        }

        if (currentParticle == GameManager.instance.particleMan.rightFootPlayer)
        {
            AudioClip randomClip = GameManager.instance.soundMan.GetComponent<StepList>().rightFoot[GameManager.instance.soundMan.GetComponent<StepList>().Randomizer()];
            GameManager.instance.soundMan.ChangeClip("Walk", randomClip);
            GameManager.instance.soundMan.Play("Walk");
            GameManager.instance.particleMan.rightFootGround.Play();
        }
    }

    //Enemy
    public void WalkingParticleEnemy(string currentParticle)
    {
        if(currentParticle == GetComponentInParent<Chase>().walkingLeft.name)
        {
            GetComponentInParent<Chase>().walkingLeft.Play();
        }
        if (currentParticle == GetComponentInParent<Chase>().walkingRight.name)
        {
            GetComponentInParent<Chase>().walkingRight.Play();
        }
    }


    public void DoDamage(float damage)
    {
        Collider[] c = Physics.OverlapSphere(transform.position + transform.forward, GetComponentInParent<Chase>().attackRange);
        for (int i = 0; i < c.Length; i++)
        {
            if (c[i].transform.tag == "Player")
            {
                if (!FindObjectOfType<Hit_Effect>().IsInvoking("DisplayBloodScreenImage"))
                {
                    FindObjectOfType<Hit_Effect>().Invoke("DisplayBloodScreenImage",0f);
                }
                c[i].GetComponent<PlayerMovement>().curplayerHp -= damage;
            }
        }
    }

    //Boss
    public void DoBossDamage()
    {
        BossInfo bInfo = GetComponentInParent<BossInfo>();
        float randomSide = Random.Range(0, 2);
        Collider[] c = Physics.OverlapSphere(bInfo.actualRayStart, bInfo.attackRange * 2, bInfo.targetMask);
        for (int i = 0; i < c.Length; i++)
        {
            if (c[i].transform.tag == "Player")
            {
                PlayerMovement pMove = c[i].GetComponent<PlayerMovement>();
                if (GameManager.gameTime == 0)
                {
                    bInfo.orb.StopCoroutine(bInfo.orb.SizeIncrease());
                    bInfo.orb.StartCoroutine(bInfo.orb.SizeDecrease());
                    bInfo.agent.speed /= bInfo.speedMultiplier;
                    bInfo.reset = true;
                    bInfo.drainChase = false;
                    bInfo.curBossState = BossState.Return;
                    bInfo.orb.HoldDamage(bInfo.damage);
                    bInfo.orb.HoldUpforce(transform.up, bossUpForce);
                    if (randomSide == 0)
                    {
                        bInfo.orb.HoldForce(transform.right, bossForce);
                    }
                    else
                    {
                        bInfo.orb.HoldForce(-transform.right, bossForce);
                    }
                }
                else
                {
                    pMove.DoDamage(bInfo.damage);
                    pMove.AddForceToPlayer(transform.up, bossUpForce);
                    if (randomSide == 0)
                    {
                        pMove.AddForceToPlayer(transform.right, bossForce);
                    }
                    else
                    {
                        pMove.AddForceToPlayer(-transform.right, bossForce);
                    }
                    bInfo.CheckCharge();
                }
                pMove.attackHit = true;
                break;
            }
        }
        if(c.Length == 0)
        {
            bInfo.Invoke("HitStun", 0f);
        }
    }

    public void WalkingParticleBoss(string currentParticle)
    {
        if (currentParticle == GetComponentInParent<BossInfo>().walkingLeft.name)
        {
            GetComponentInParent<BossInfo>().walkingLeft.Play();
        }
        if (currentParticle == GetComponentInParent<BossInfo>().walkingRight.name)
        {
            GetComponentInParent<BossInfo>().walkingRight.Play();
        }
    }

}
