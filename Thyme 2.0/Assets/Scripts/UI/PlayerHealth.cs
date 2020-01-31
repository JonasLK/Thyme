using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public GameObject h;
    public Image health;
    public Sprite healthBar;
    // player health script here

    void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKey("a"))
        {
            h.transform.localScale = new Vector3(-1, 1, 1);
            PlayerTakesDamage();
        }
    }
    
    public void PlayerTakesDamage()
    {
        /* healthBar.scale.x = current player health : max player health;
         */
        if(h.transform.localScale.x < 0)
        {
            h.transform.localScale = new Vector3(0,1,1);
            //death code
        }
    }
}