using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = ("Slash"), menuName = "Slash/New Slash")]
public class Slash : ScriptableObject
{
    public List<Slash> combos;

    public Vector3 launchForce;

    public string audio;
    public string animation;

    public float animTimer, maxTimer;
    public float chargeTimer, chargeMax;

    public int damage;

    public bool launchAttack;
    public bool aerialAttack;

    public AttackInput attackInput;

    public DirectionalInput directionalInput;

    public HitArea hitArea;

    public virtual void NewAttack(ComboHolder comboHolder, Slash slash)
    {
        comboHolder.curSlash = slash;

        comboHolder.inCombo = true;

        comboHolder.doesAttack.didAttack = false;

        comboHolder.time = 0;

        comboHolder.playerMovement.PlayAnime(slash.animation);

        GameManager.instance.soundMan.Play(slash.audio);

        Debug.Log(comboHolder.curSlash.damage);
    }

    public virtual void ContinueAttack(ComboHolder combo, AttackInput attack, DirectionalInput dirInput, bool aerial)
    {
        combo.NewAttack(combo.curSlash, attack, dirInput, aerial);
    }
}
