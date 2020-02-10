using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waves : MonoBehaviour
{
    public int curWave;
    public float spawndelay;
    public List<Spawns> wave = new List<Spawns>();

    [System.Serializable]
    public struct Spawns
    {
        [SerializeField] public GameObject enemy;
    }
}

