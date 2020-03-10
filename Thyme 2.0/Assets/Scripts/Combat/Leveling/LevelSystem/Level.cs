using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level/New Level")]
public class Level : ScriptableObject
{
    public int expNeeded;

    public List<NewStats> statIncrease = new List<NewStats>();

    public virtual float IncreaseStats(NewStats newStats)
    {
        return newStats.increaseBy;
    } 
}
