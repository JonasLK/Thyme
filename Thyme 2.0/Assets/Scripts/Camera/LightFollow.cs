using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFollow : MonoBehaviour
{
    public Transform target;

    void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
    }
    void Update()
    {
        transform.LookAt(target);
    }
}
