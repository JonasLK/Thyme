using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    [HideInInspector]
    public float chargeMultiplier;

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

}
