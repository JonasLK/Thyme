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
    public Image playerEnhanceCD;
    public Image timeStopCD;

    public TextMeshProUGUI orbDrainCDTimer;
    public TextMeshProUGUI playerEnhanceCDTimer;
    public TextMeshProUGUI timeStopCDTimer;

    public Image orbDrainDur;
    public Image playerEnhanceDur;
    public Image timeStopDur;

    public TextMeshProUGUI orbDrainDurTimer;
    public TextMeshProUGUI playerEnhanceDurTimer;
    public TextMeshProUGUI timeStopDurTimer;

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
        playerEnhanceCD.color = playerAbilities.enhance;
        timeStopCD.color = playerAbilities.timeStop;
    }

    public void AbilityDurColor()
    {
        orbDrainDur.color = playerAbilities.orb;
        playerEnhanceDur.color = playerAbilities.enhance;
        timeStopDur.color = playerAbilities.timeStop;
    }

    public void AbilityCDText()
    {
        orbDrainCDTimer.text = Mathf.RoundToInt(playerAbilities.curOrbCooldown).ToString();
        playerEnhanceCDTimer.text = Mathf.RoundToInt(playerAbilities.curPlayerEnchanceCooldown).ToString();
        timeStopCDTimer.text = Mathf.RoundToInt(playerAbilities.curTimeStopCooldown).ToString();
    }

    public void AbilityDurText()
    {
        orbDrainDurTimer.text = Mathf.RoundToInt(playerAbilities.curOrbDur).ToString();
        playerEnhanceDurTimer.text = Mathf.RoundToInt(playerAbilities.curPlayerEnhanceDur).ToString();
        timeStopDurTimer.text = Mathf.RoundToInt(playerAbilities.curTimeStopDur).ToString();
    }

}
