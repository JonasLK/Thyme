using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboHolder : MonoBehaviour
{
    public float time;

    public bool inCombo;
    public bool ableToAttack = true;
    public bool inAir;

    public DoesAttack doesAttack;
    public PlayerMovement playerMovement;

    public List<Slash> slashes;
    public List<GameObject> damageAreas;

    public string lightSlashInput;
    public string heavySlashInput;

    public Slash curSlash;
    private AttackInput attack;
    private DirectionalInput directionalInput;

    public void Update()
    {
        DirectionalInputCheck();

        if (curSlash != null && inCombo)
        {
            Timer(curSlash.animTimer, curSlash.maxTimer);
        }
    }

    public void NewAttack(AttackInput attackInput, DirectionalInput directionalInput, bool aerial)
    {
        for (int i = 0; i < slashes.Count; i++)
        {
            if (attackInput == slashes[i].attackInput && directionalInput == slashes[i].directionalInput && slashes[i].aerialAttack == inAir)
            {
                curSlash = slashes[i];

                slashes[i].NewAttack(this, slashes[i]);
                return;
            }
        }

        for (int o = 0; o < slashes.Count; o++)
        {
            if (attackInput == slashes[o].attackInput && slashes[o].directionalInput == DirectionalInput.none && slashes[o].aerialAttack == inAir)
            {
                curSlash = slashes[o];

                slashes[o].NewAttack(this, slashes[o]);
                return;
            }
        }
    }


    public void DirectionalInputCheck()
    {
        if (Input.GetAxis("Vertical") > 0)
        {
            directionalInput = DirectionalInput.forward;
        }

        if (Input.GetAxis("Vertical") < 0)
        {
            directionalInput = DirectionalInput.back;
        }

        if(Input.GetAxis("Horizontal") > 0)
        {
            directionalInput = DirectionalInput.right;
        }

        if(Input.GetAxis("Horizontal") < 0)
        {
            directionalInput = DirectionalInput.left;
        }

        if(Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
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
                    attack = AttackInput.lightAttack;
                    NewAttack(attack, directionalInput, inAir);
                }

                if (Input.GetButtonDown(heavySlashInput))
                {
                    attack = AttackInput.heavyAttack;
                    NewAttack(attack, directionalInput, inAir);
                }
            }
            else
            {
                if (Input.GetButtonDown(lightSlashInput))
                {
                    curSlash.ContinueAttack(this, AttackInput.lightAttack, directionalInput, inAir);
                }
                else if (Input.GetButtonDown(heavySlashInput))
                {
                    curSlash.ContinueAttack(this, AttackInput.heavyAttack, directionalInput, inAir);
                }
            }
        }
    }

    public void Timer(float animTimer, float maxTimer)
    {
        if(time < maxTimer)
        {
            ableToAttack = false;

            time += Time.deltaTime;

            if (time > animTimer)
            {
                if(doesAttack.didAttack == false)
                {
                    doesAttack.DoDamage(curSlash);
                }

                ableToAttack = true;
            }
        }
        else
        {
            time = 0;
            ableToAttack = true;
            inCombo = false;
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
