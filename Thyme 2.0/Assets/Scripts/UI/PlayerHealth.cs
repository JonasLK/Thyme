using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public GameObject emptyToScale;
    public Hit_Effect hitEffect;

    //for testing
    public float maxHealth;
    public float currentHealth;
    // player health script here

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void Update()
    {
        if (Input.GetKeyDown("m"))
        {
            PlayerTakesDamage(5);
        }
    }
    
    public void PlayerTakesDamage(float damageTaken)
    {
        currentHealth -= damageTaken;
        emptyToScale.transform.localScale = new Vector3(currentHealth / maxHealth, emptyToScale.transform.localScale.y, emptyToScale.transform.localScale.z);
        hitEffect.DisplayBloodScreenImage();
         

        if(emptyToScale.transform.localScale.x < 0)
        {
            emptyToScale.transform.localScale = new Vector3(0, emptyToScale.transform.localScale.y, emptyToScale.transform.localScale.z);
            //death code
        }
    }
}