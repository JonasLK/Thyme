using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour
{
    public float multiplier = 2;
    public float chargeTimer, chargeTimerMax = 10;
    public bool enabledPillar, donePillar,drained;

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
