using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    [HideInInspector]
    public float chargeMultiplier;


    public void JumpParticle()
    {
        GameManager.instance.particleMan.jumpEffect.Play();
    }

    public void LandParticle()
    {
        GameManager.instance.particleMan.landsEffect.Play();
    }

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

    public void LeftWalkingParticle()
    {
        GameManager.instance.particleMan.leftFootGround.Play();
    }

    public void RightWalkingParticle()
    {
        GameManager.instance.particleMan.rightFootGround.Play();
    }

    public void DoDamage(float damage)
    {
        Collider[] c = Physics.OverlapSphere(transform.position + transform.forward, GetComponentInParent<Chase>().attackRange);
        for (int i = 0; i < c.Length; i++)
        {
            if (c[i].transform.tag == "Player")
            {
                c[i].GetComponent<PlayerMovement>().curplayerHp -= damage;
            }
        }
    }

    public void LaunchUp(float BoostPower)
    {
        GetComponentInParent<PlayerMovement>().AddVel(BoostPower*chargeMultiplier);
    }
}
