using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewUiManager : MonoBehaviour
{
    [Header("Functional UI")]
    public AbilityBase playerAbilities;

    public TextMeshProUGUI playerStatus;
    public TextMeshProUGUI playerHp;

    public Image playerEnhanceDashCD;
    public Image playerEnhanceCD;
    public Image playerHealCD;

    public TextMeshProUGUI playerEnhanceDashCDTimer;
    public TextMeshProUGUI playerEnhanceCDTimer;
    public TextMeshProUGUI playerHealCDTimer;
   
    public Image playerEnhanceDur;
    public Image playerHealDur;

    public TextMeshProUGUI playerEnhanceDurTimer;
    public TextMeshProUGUI playerHealDurTimer;

    [Header("HitScreen")]
    public GameObject BloodScreen;
    public float dur, mag;

    // Update is called once per frame
    void Update()
    {
        PlayerStatusCheck();
        AbilityCDColor();
        AbilityCDText();
        AbilityDurColor();
        AbilityDurText();
    }

    public void PlayerStatusCheck()
    {
        if(playerAbilities.GetComponent<PlayerMovement>().curState != PlayerMovement.PlayerState.Death)
        {
            playerStatus.text = "Current Status: Alive";
            playerHp.text = "Current Hp: " + Mathf.Round(playerAbilities.GetComponent<PlayerMovement>().curplayerHp);
        }
        else
        {
            playerStatus.text = "Current Status: Dead";
            playerHp.text = "Current Hp: 0";
        }
    }

    public void AbilityCDColor()
    {
        playerEnhanceDashCD.color = playerAbilities.orb;
        playerEnhanceCD.color = playerAbilities.enhance;
        playerHealCD.color = playerAbilities.heal;
    }

    public void AbilityDurColor()
    {
        playerEnhanceDur.color = playerAbilities.enhance;
        playerHealDur.color = playerAbilities.heal;
    }

    public void AbilityCDText()
    {
        playerEnhanceDashCDTimer.text = Mathf.RoundToInt(playerAbilities.curEnhancedDashCooldown).ToString();
        playerEnhanceCDTimer.text = Mathf.RoundToInt(playerAbilities.curPlayerEnchanceCooldown).ToString();
        playerHealCDTimer.text = Mathf.RoundToInt(playerAbilities.curHealCooldown).ToString();
    }

    public void AbilityDurText()
    {
        playerEnhanceDurTimer.text = Mathf.RoundToInt(playerAbilities.curPlayerEnhanceDur).ToString();
        playerHealDurTimer.text = Mathf.RoundToInt(playerAbilities.curHealDur).ToString();
    }

    public void SetBloodScreen()
    {
        BloodScreen.SetActive(true);
        GameManager.instance.shake.StartCoroutine(GameManager.instance.shake.CustomScreenShake(dur,mag));
    }

    public void ResetBloodScreen()
    {
        BloodScreen.SetActive(false);
    }
}
