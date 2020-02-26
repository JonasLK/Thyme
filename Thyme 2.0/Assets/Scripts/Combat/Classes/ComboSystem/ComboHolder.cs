using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboHolder : MonoBehaviour
{
    public float time;
    public float chargeTimer;

    public bool inCombo;
    public bool ableToAttack = true;
    public bool inAir;
    public bool charging;

    public DoesAttack doesAttack;
    public PlayerMovement playerMovement;

    public List<Slash> slashes;
    public List<GameObject> damageAreas;

    public string lightSlashInput;
    public string heavySlashInput;

    public Slash curSlash;
    public Slash baseSlash;
    public Slash nextSlash;
    private AttackInput attack;
    public DirectionalInput directionalInput;

    public void NewAttack(Slash slash, AttackInput attackInput, DirectionalInput directionalInput, bool aerial)
    {
        for (int i = 0; i < slashes.Count; i++)
        {
            if (attackInput == slashes[i].attackInput && directionalInput == slashes[i].directionalInput && slashes[i].aerialAttack == inAir)
            {
                curSlash = slashes[i];

                if (!charging)
                {
                    slashes[i].NewAttack(this, slashes[i]);
                }
                return;
            }
        }

        for (int o = 0; o < slashes.Count; o++)
        {
            if (attackInput == slashes[o].attackInput && slashes[o].directionalInput == DirectionalInput.none && slashes[o].aerialAttack == inAir)
            {
                curSlash = slashes[o];

                if (!charging)
                {
                    slashes[o].NewAttack(this, slashes[o]);
                }
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
                    attack = AttackInput.lightAttack;
                    baseSlash.ContinueAttack(this, attack, directionalInput, inAir);
                }

                else if (Input.GetButtonDown(heavySlashInput))
                {
                    attack = AttackInput.heavyAttack;
                    chargeTimer = 1;
                }

                if (Input.GetButton(heavySlashInput))
                {
                    charging = true;
                    chargeTimer += Time.deltaTime;
                }
                else
                {
                    charging = false;
                }

                if (Input.GetButtonUp(heavySlashInput))
                {
                    baseSlash.ContinueAttack(this, attack, directionalInput, inAir);
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
                    attack = AttackInput.heavyAttack;
                    chargeTimer = 1;
                    curSlash.ContinueAttack(this, attack, directionalInput, inAir);
                }

                if (Input.GetButton(heavySlashInput))
                {
                    charging = true;
                    chargeTimer += Time.deltaTime;
                }
                else
                {
                    charging = false;
                }

                if (Input.GetButtonUp(heavySlashInput))
                {
                    curSlash.ContinueAttack(this, attack, directionalInput, inAir);
                    Debug.Log("Charged");
                }
            }
        }
    }

    public void Timer(float animTimer, float maxTimer)
    {
        if(time < maxTimer)
        {
            playerMovement.curState = PlayerMovement.PlayerState.Attack;

            ableToAttack = false;

            time += Time.deltaTime;

            if (time > animTimer && !charging)
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
            playerMovement.ReturnState();
            time = 0;
            ableToAttack = true;
            inCombo = false;
            curSlash = null;
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
