using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NewStats
{
    public Stats stat;
    
    [Range(0, 100)]
    public float increaseBy;
}

public enum Stats
{
    Health,
    Strength,
    Speed
}
