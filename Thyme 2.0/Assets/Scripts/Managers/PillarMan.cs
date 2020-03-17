using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarMan : MonoBehaviour
{
    public List<Pillar> pylons = new List<Pillar>();
    public int curPylon;

    private void Update()
    {
        if (!pylons[curPylon].GetComponent<Pillar>().donePillar)
        {
            pylons[curPylon].GetComponent<Pillar>().enabledPillar = true;
        }
        else if (pylons[curPylon].GetComponent<Pillar>().drained)
        {
            if(curPylon < pylons.Count-1)
            {
                curPylon++;
            }
        }
    }
}
