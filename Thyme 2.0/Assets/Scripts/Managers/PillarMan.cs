using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarMan : MonoBehaviour
{
    public List<Pillar> pylons = new List<Pillar>();
    public List<Pillar> donePylon = new List<Pillar>();
    public int curPylon;
    public bool finalPillarDone;

    public void StartPylon()
    {
        if (!pylons[curPylon].GetComponent<Pillar>().enabledPillar)
        {
            pylons[curPylon].GetComponent<Pillar>().enabledPillar = true;
        }
    }

    public void NextPylon()
    {
        if (curPylon < pylons.Count - 1)
        {
            curPylon++;
            StartPylon();
        }
        else
        {
            finalPillarDone = true;
        }
    }
}
