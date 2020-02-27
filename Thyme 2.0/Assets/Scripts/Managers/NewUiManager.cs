using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewUiManager : MonoBehaviour
{
    public AbilityBase playerAbilities;

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
        orbDrainCD.color = playerAbilities.orb;
        playerEnhanceCD.color = playerAbilities.enhance;
        timeStopCD.color = playerAbilities.timeStop;

        orbDrainDur.color = playerAbilities.orb;
        playerEnhanceDur.color = playerAbilities.enhance;
        timeStopDur.color = playerAbilities.timeStop;

        orbDrainCDTimer.text = Mathf.RoundToInt(playerAbilities.curOrbCooldown).ToString();
        playerEnhanceCDTimer.text = Mathf.RoundToInt(playerAbilities.curPlayerEnchanceCooldown).ToString();
        timeStopCDTimer.text = Mathf.RoundToInt(playerAbilities.curTimeStopCooldown).ToString();

        orbDrainDurTimer.text = Mathf.RoundToInt(playerAbilities.curOrbDur).ToString();
        playerEnhanceDurTimer.text = Mathf.RoundToInt(playerAbilities.curPlayerEnhanceDur).ToString();
        timeStopDurTimer.text = Mathf.RoundToInt(playerAbilities.curTimeStopDur).ToString();
    }
}
