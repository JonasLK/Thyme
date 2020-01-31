using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = ("Slash"), menuName = "Slash/New Slash")]
public class Slash : ScriptableObject
{
    public List<Slash> combos;

    public Animation animation;

    public float animTimer, maxTimer;

    public int damage;

    public bool launchAttack;

    public AttackInput attackInput;

    public DirectionalInput directionalInput;

    public HitArea hitArea;

    public virtual void NewAttack(ComboHolder comboHolder, Slash slash)
    {
        comboHolder.curSlash = slash;

        comboHolder.inCombo = true;

        comboHolder.doesAttack.didAttack = false;

        comboHolder.time = 0;

        Debug.Log(comboHolder.curSlash.damage);
    }

    public virtual void ContinueAttack(ComboHolder combo, AttackInput attack, DirectionalInput dirInput)
    {
        for(int i = 0; i < combos.Count; i++)
        {
            if(attack == combos[i].attackInput && dirInput == combos[i].directionalInput)
            {
                NewAttack(combo, combos[i]);
                return;
            }
        }

        for(int o = 0; o < combos.Count; o++)
        {
            if(attack == combos[o].attackInput && combos[o].directionalInput == DirectionalInput.none)
            {
                NewAttack(combo, combos[o]);
                return;
            }
        }

        combo.NewAttack(attack, dirInput);
    }
}
