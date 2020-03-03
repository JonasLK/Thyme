using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public GameObject emptyToScale;
    public GameObject emptyToScaleMaxHp;
    public Hit_Effect hitEffect;
    public Text healthText;
    // player health script here

    //for testing
    public float maxHealth;
    public float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthText();
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
        UpdateHealthText();

        if(currentHealth < 0)
        {
            currentHealth = 0;
            emptyToScale.transform.localScale = new Vector3(0, emptyToScale.transform.localScale.y, emptyToScale.transform.localScale.z);
            UpdateHealthText();
            //death code
        }
    }

    public void UpdateHealthText()
    {
        healthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
    }
}