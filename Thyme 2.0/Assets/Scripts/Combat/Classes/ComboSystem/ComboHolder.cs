using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboHolder : MonoBehaviour
{
    public float time;
    public float chargeTimer;

    public bool inCombo;
    public bool ableToAttack = true;
    public bool inAir,charging;

    public DoesAttack doesAttack;
    public PlayerMovement playerMovement;

    [HideInInspector]
    public GameObject player;

    public string lightSlashInput;
    public string heavySlashInput;

    public Slash curSlash;
    public Slash baseSlash;
    public Slash nextSlash;
    private AttackInput attack;
    public DirectionalInput directionalInput;

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void NewAttack(Slash slash, AttackInput attackInput, DirectionalInput directionalInput, bool aerial)
    {
        if (slash != null)
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

        else if (Input.GetAxis("Horizontal") > 0.1)
        {
            directionalInput = DirectionalInput.right;
        }

        else if (Input.GetAxis("Horizontal") < -0.1)
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
                    NewAttack(baseSlash, AttackInput.lightAttack, directionalInput, playerMovement.inAir);
                }

                else if (Input.GetButtonDown(heavySlashInput) && !Input.GetButtonDown(lightSlashInput))
                {
                    chargeTimer = 1;
                    NewAttack(baseSlash, AttackInput.heavyAttack, directionalInput, playerMovement.inAir);
                }

                if (Input.GetButton(heavySlashInput) && !Input.GetButton(lightSlashInput) && nextSlash != null)
                {
                    playerMovement.curState = PlayerMovement.PlayerState.Charge;
                    if (Input.GetButtonDown(lightSlashInput))
                    {
                        nextSlash = null;
                    }

                    if (playerMovement.inAir == nextSlash.aerialAttack)
                    {
                        charging = true;
                        chargeTimer += Time.deltaTime;
                    }
                    else
                    {
                        charging = false;
                    }
                }
                else
                {
                    charging = false;
                }

                if (Input.GetButtonUp(heavySlashInput) && !Input.GetButton(lightSlashInput) && nextSlash != null)
                {

                    if (playerMovement.inAir == nextSlash.aerialAttack)
                    {
                        curSlash.NewAttack(this, nextSlash);
                    }
                }
                else if (Input.GetButtonUp(heavySlashInput))
                {
                    Debug.Log("ok");
                }
            }
            else
            {
                if (Input.GetButtonDown(lightSlashInput))
                {
                    NewAttack(curSlash, AttackInput.lightAttack, directionalInput, playerMovement.inAir);
                }
                else if (Input.GetButtonDown(heavySlashInput) && !Input.GetButtonDown(lightSlashInput))
                {
                    chargeTimer = 1;
                    NewAttack(curSlash, AttackInput.heavyAttack, directionalInput, playerMovement.inAir);
                }

                if (Input.GetButton(heavySlashInput) && !Input.GetButton(lightSlashInput) && nextSlash != null)
                {
                    playerMovement.curState = PlayerMovement.PlayerState.Charge;
                    if (playerMovement.inAir == nextSlash.aerialAttack)
                    {
                        charging = true;
                        chargeTimer += Time.deltaTime;
                    }
                    else
                    {
                        charging = false;
                    }
                }

                if (Input.GetButtonUp(heavySlashInput) && !Input.GetButtonDown(lightSlashInput) && nextSlash != null)
                {
                    if (playerMovement.inAir == nextSlash.aerialAttack)
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
        if (time < maxTimer)
        {
            playerMovement.curState = PlayerMovement.PlayerState.Attack;

            ableToAttack = false;

            time += Time.deltaTime;

            //GameManager.instance.particleMan.swordSlash.gameObject.SetActive(true);

            if (time > animTimer && !charging)
            {
                if (doesAttack.didAttack == false)
                {
                    //doesAttack.DoDamage(curSlash);
                }

                ableToAttack = true;

                //GameManager.instance.particleMan.swordSlash.gameObject.SetActive(false);
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

