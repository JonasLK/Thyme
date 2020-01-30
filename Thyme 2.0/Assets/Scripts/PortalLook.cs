﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalLook : MonoBehaviour
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
