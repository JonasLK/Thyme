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

    public string lightSlashInput;
    public string heavySlashInput;

    public Slash curSlash;
    public Slash baseSlash;
    public Slash nextSlash;
    private AttackInput attack;
    public DirectionalInput directionalInput;

    public void NewAttack(Slash slash, AttackInput attackInput, DirectionalInput directionalInput, bool aerial)
    {
        for (int i = 0; i < slash.combos.Count; i++)
        {
            if (attackInput == slash.combos[i].attackInput && directionalInput == slash.combos[i].directionalInput && slash.combos[i].aerialAttack == playerMovement.inAir)
            {
                curSlash = slash.combos[i];

                if (slash.combos[i].launchAttack)
                {
                    nextSlash = slash.combos[i];
                }
                else
                {
                    slash.combos[i].NewAttack(this, slash.combos[i]);
                }
                return;
            }
        }

        for (int o = 0; o < slash.combos.Count; o++)
        {
            if (attackInput == slash.combos[o].attackInput && slash.combos[o].directionalInput == DirectionalInput.none && slash.combos[o].aerialAttack == playerMovement.inAir)
            {
                curSlash = slash.combos[o];

                if (slash.combos[o].launchAttack)
                {
                    nextSlash = slash.combos[o];
                }
                else
                {
                    slash.combos[o].NewAttack(this, slash.combos[o]);
                }
                return;
            }
        }

        NewAttack(baseSlash, attackInput, directionalInput, inAir);
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
        if (ableToAttack)
        {
            if (!inCombo)
            {
                if (Input.GetButtonDown(lightSlashInput))
                {
                    baseSlash.ContinueAttack(this, AttackInput.lightAttack, directionalInput, playerMovement.inAir);
                }

<<<<<<< HEAD
                else if (Input.GetButtonDown(heavySlashInput) && !Input.GetButtonDown(lightSlashInput))
                {
                    chargeTimer = 1;
                    NewAttack(baseSlash, AttackInput.heavyAttack, directionalInput, playerMovement.inAir);
=======
                else if (Input.GetButtonDown(heavySlashInput))
                {
                    chargeTimer = 1;
                    curSlash.ContinueAttack(this, AttackInput.heavyAttack, directionalInput, playerMovement.inAir);
>>>>>>> parent of 6f3afb4... Try break it plz
                }

                if (Input.GetButton(heavySlashInput))
                {
                    if(playerMovement.inAir == nextSlash.aerialAttack)
                    {
                        charging = true;
                        chargeTimer += Time.deltaTime;
                    }
                }
                else
                {
                    charging = false;
                }

                if (Input.GetButtonUp(heavySlashInput))
                {
                    if (playerMovement.inAir == nextSlash.aerialAttack)
                    {
                        curSlash.NewAttack(this, nextSlash);
                    }
                }
            }
            else
            {
                if (Input.GetButtonDown(lightSlashInput))
                {
                    curSlash.ContinueAttack(this, AttackInput.lightAttack, directionalInput, playerMovement.inAir);
                }
<<<<<<< HEAD
                else if (Input.GetButtonDown(heavySlashInput) && !Input.GetButtonDown(lightSlashInput))
=======
                else if (Input.GetButtonDown(heavySlashInput))
>>>>>>> parent of 6f3afb4... Try break it plz
                {
                    chargeTimer = 1;
<<<<<<< HEAD
                    NewAttack(curSlash, AttackInput.heavyAttack, directionalInput, playerMovement.inAir);
                }

                if (Input.GetButtonDown(heavySlashInput) && !Input.GetButtonDown(lightSlashInput) && nextSlash != null)
=======
                    curSlash.ContinueAttack(this, attack, directionalInput, playerMovement.inAir);
                }

                if (Input.GetButton(heavySlashInput))
>>>>>>> parent of 6f3afb4... Try break it plz
                {
                    charging = true;
                    chargeTimer += Time.deltaTime;
                }
                else
                {
                    charging = false;
                }

<<<<<<< HEAD
                if (Input.GetButtonUp(heavySlashInput) && !Input.GetButtonDown(lightSlashInput) && nextSlash != null)
=======
                if (Input.GetButtonUp(heavySlashInput))
>>>>>>> parent of 6f3afb4... Try break it plz
                {
                    if(playerMovement.inAir == nextSlash.aerialAttack)
                    {
                        curSlash.NewAttack(this, nextSlash);
                    }
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
