using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewUiManager : MonoBehaviour
{
    public AbilityBase playerAbilities;

    public TextMeshProUGUI playerStatus;
    public TextMeshProUGUI playerHp;

    public Image orbDrainCD;
    public Image timeStopCD;
    public Image playerEnhanceCD;
    public Image playerHealCD;

    public TextMeshProUGUI orbDrainCDTimer;
    public TextMeshProUGUI timeStopCDTimer;
    public TextMeshProUGUI playerEnhanceCDTimer;
    public TextMeshProUGUI playerHealCDTimer;

    public Image orbDrainDur;
    public Image timeStopDur;
    public Image playerEnhanceDur;
    public Image playerHealDur;

    public TextMeshProUGUI orbDrainDurTimer;
    public TextMeshProUGUI timeStopDurTimer;
    public TextMeshProUGUI playerEnhanceDurTimer;
    public TextMeshProUGUI playerHealDurTimer;

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
        orbDrainCD.color = playerAbilities.orb;
        timeStopCD.color = playerAbilities.timeStop;
        playerEnhanceCD.color = playerAbilities.enhance;
        playerHealCD.color = playerAbilities.heal;
    }

    public void AbilityDurColor()
    {
        orbDrainDur.color = playerAbilities.orb;
        timeStopDur.color = playerAbilities.timeStop;
        playerEnhanceDur.color = playerAbilities.enhance;
        playerHealDur.color = playerAbilities.heal;
    }

    public void AbilityCDText()
    {
        orbDrainCDTimer.text = Mathf.RoundToInt(playerAbilities.curOrbCooldown).ToString();
        timeStopCDTimer.text = Mathf.RoundToInt(playerAbilities.curTimeStopCooldown).ToString();
        playerEnhanceCDTimer.text = Mathf.RoundToInt(playerAbilities.curPlayerEnchanceCooldown).ToString();
        playerHealCDTimer.text = Mathf.RoundToInt(playerAbilities.curHealCooldown).ToString();
    }

    public void AbilityDurText()
    {
        orbDrainDurTimer.text = Mathf.RoundToInt(playerAbilities.curOrbDur).ToString();
        timeStopDurTimer.text = Mathf.RoundToInt(playerAbilities.curTimeStopDur).ToString();
        playerEnhanceDurTimer.text = Mathf.RoundToInt(playerAbilities.curPlayerEnhanceDur).ToString();
        playerHealDurTimer.text = Mathf.RoundToInt(playerAbilities.curHealDur).ToString();
    }

}
