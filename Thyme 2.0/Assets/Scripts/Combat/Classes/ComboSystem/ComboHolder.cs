using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboHolder : MonoBehaviour
{
    public float time;
    public float chargeTime =  1;

    public bool inCombo;
    public bool ableToAttack = true;
    public bool inAir;

    public DoesAttack doesAttack;
    public PlayerMovement playerMovement;

    public List<Slash> slashes;
    public List<GameObject> damageAreas;

    public string lightSlashInput;
    public string heavySlashInput;

    public Slash baseSlash;
    public Slash curSlash;
    public Slash nextSlash;
    private AttackInput attack;
    public DirectionalInput directionalInput;

    public void NewAttack(AttackInput attackInput, DirectionalInput directionalInput, bool aerial)
    {
        for (int i = 0; i < slashes.Count; i++)
        {
            if (attackInput == slashes[i].attackInput && directionalInput == slashes[i].directionalInput && slashes[i].aerialAttack == inAir)
            {
                curSlash = slashes[i];
                slashes[i].NewAttack(this, slashes[i], chargeTime);
                return;
            }
        }

        for (int o = 0; o < slashes.Count; o++)
        {
            if (attackInput == slashes[o].attackInput && slashes[o].directionalInput == DirectionalInput.none && slashes[o].aerialAttack == inAir)
            {
                curSlash = slashes[o];
                slashes[o].NewAttack(this, slashes[o], chargeTime);
                return;
            }
        }
    }


    public void DirectionalInputCheck()
    {
        if (Input.GetAxis("Vertical") > 0.1)
        {
            directionalInput = DirectionalInput.forward;
        }

        else if (Input.GetAxis("Vertical") < -0.1)
        {
            directionalInput = DirectionalInput.back;
        }

        else if(Input.GetAxis("Horizontal") > 0.1)
        {
            directionalInput = DirectionalInput.right;
        }

        else if(Input.GetAxis("Horizontal") < -0.1)
        {
            directionalInput = DirectionalInput.left;
        }

        else
        {
            directionalInput = DirectionalInput.neutral;
        }
    }

    public void InputCheck()
    {
        inAir = playerMovement.inAir;
        if (ableToAttack)
        {
            if (!inCombo)
            {
                if (Input.GetButtonDown(lightSlashInput))
                {
                    curSlash.NewHeavyAttack(this, AttackInput.lightAttack, directionalInput, inAir, chargeTime);
                    curSlash.NewAttack(this, nextSlash, chargeTime);
                }

                if (Input.GetButtonDown(heavySlashInput))
                {
                    curSlash.NewHeavyAttack(this, AttackInput.heavyAttack, directionalInput, inAir, chargeTime);
                }

                if (Input.GetButton(heavySlashInput))
                {
                    chargeTime += Time.deltaTime;
                }

                if (Input.GetButtonUp(heavySlashInput))
                {
                    curSlash.NewAttack(this, nextSlash, chargeTime);   
                }
            }
            else
            {
                if (Input.GetButtonDown(lightSlashInput))
                {
                    curSlash.ContinueAttack(this, AttackInput.lightAttack, directionalInput, inAir, chargeTime);
                }

                if (Input.GetButtonDown(heavySlashInput))
                {
                    curSlash.NewHeavyAttack(this, AttackInput.heavyAttack, directionalInput, inAir, chargeTime);
                }

                if (Input.GetButton(heavySlashInput))
                {
                    chargeTime += Time.deltaTime;
                }

                if (Input.GetButtonUp(heavySlashInput))
                {
                    attack = AttackInput.heavyAttack;
                    curSlash.NewAttack(this, nextSlash, chargeTime);
                }
            }
        }
    }

    public void Timer(float animTimer, float maxTimer)
    {
        if(time < maxTimer && nextSlash)
        {
            //playerMovement.curState = PlayerMovement.PlayerState.Attack;

            ableToAttack = false;

            time += Time.deltaTime;

            if (time > animTimer)
            {
                if(doesAttack.didAttack == false)
                {
                    doesAttack.DoDamage(curSlash);
                    chargeTime = 1;
                }

                ableToAttack = true;
            }
        }
        else
        {
            //playerMovement.ReturnState();
            time = 0;
            ableToAttack = true;
            inCombo = false;
            curSlash = baseSlash;
        }
    }
}
    public enum AttackInput
    {
        lightAttack,
        heavyAttack
    }

    public enum DirectionalInput
    {
        none,
        neutral,
        forward,
        back,
        left,
        right
    }

    public enum HitArea
    {
        frontCone,
        forwardTrust,
        sphereAround
    }
