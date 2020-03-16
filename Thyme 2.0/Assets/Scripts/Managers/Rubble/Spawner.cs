using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> gameObjects;

    public void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Spawn();
        }
    }

    public void Spawn()
    {
        foreach (GameObject gameObject in gameObjects)
        {
            Instantiate(gameObject);
        }
    }
}
