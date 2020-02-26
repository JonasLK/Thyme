using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = ("Slash"), menuName = "Slash/New Slash")]
public class Slash : ScriptableObject
{
    public List<Slash> combos;

    public string audio;
    public string animation;

    public float animTimer, maxTimer;
    public float chargeTimer, chargeTimeMax;

    public int damage;

    public Vector3 launchForce;

    public bool launchAttack;
    public bool aerialAttack;

    public AttackInput attackInput;

    public DirectionalInput directionalInput;

    public HitArea hitArea;

    public virtual void NewAttack(ComboHolder comboHolder, Slash slash, float charge)
    {
        if(charge > chargeTimeMax)
        {
            chargeTimer = chargeTimeMax; 
        }
        else
        {
            chargeTimer = charge;
        }

        comboHolder.curSlash = slash;

        comboHolder.inCombo = true;

        comboHolder.doesAttack.didAttack = false;

        comboHolder.time = 0;

        AttackAnim(comboHolder, slash);

        GameManager.instance.soundMan.Play(slash.audio);

        Debug.Log(comboHolder.curSlash.damage);
    }

    public virtual void AttackAnim(ComboHolder comboHolder, Slash slash)
    {
        comboHolder.playerMovement.PlayAnime(slash.animation);
    }

    public virtual void NewHeavyAttack(ComboHolder combo, AttackInput attack, DirectionalInput dirInput, bool aerial, float charge)
    {
        for (int i = 0; i < combos.Count; i++)
        {
            if (attack == combos[i].attackInput && dirInput == combos[i].directionalInput && aerial == aerialAttack)
            {
                combo.nextSlash = combos[i];
                return;
            }
        }

        for (int o = 0; o < combos.Count; o++)
        {
            if (attack == combos[o].attackInput && combos[o].directionalInput == DirectionalInput.none && aerial == aerialAttack)
            {
                combo.nextSlash = combos[o];
                return;
            }
        }
    }

    public virtual void ContinueAttack(ComboHolder combo, AttackInput attack, DirectionalInput dirInput, bool aerial, float charge)
    {
        for(int i = 0; i < combos.Count; i++)
        {
            if(attack == combos[i].attackInput && dirInput == combos[i].directionalInput && aerial == aerialAttack)
            {
                NewAttack(combo, combos[i] ,charge);
                return;
            }
        }

        for(int o = 0; o < combos.Count; o++)
        {
            if(attack == combos[o].attackInput && combos[o].directionalInput == DirectionalInput.none && aerial == aerialAttack)
            {
                NewAttack(combo, combos[o], charge);
                return;
            }
        }

        combo.NewAttack(attack, dirInput, aerial);
    }
}
