using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour
{
    public float chargeTimer, chargeTimerMax;
    public bool enabledPillar, donePillar;

    public void Update()
    {
        if(enabledPillar && !donePillar)
        {
            ChargingPillar();
        }
    }

    public void ChargingPillar()
    {
        if(chargeTimer < chargeTimerMax)
        {
            chargeTimer += Time.deltaTime;
        }
        else
        {
            donePillar = true;
        }
    }
}
