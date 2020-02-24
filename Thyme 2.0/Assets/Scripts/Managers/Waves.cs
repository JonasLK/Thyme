using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waves : MonoBehaviour
{
    public GameObject enemy;
    public Vector3 spawnPoint;

    private void Update()
    {
        if (Input.GetButtonDown("SpawnDummy"))
        {
            Instantiate(enemy, spawnPoint, Quaternion.identity);
        }
    }
}

