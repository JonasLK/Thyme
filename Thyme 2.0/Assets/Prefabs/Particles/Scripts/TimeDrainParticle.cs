using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeDrainParticle : MonoBehaviour
{
    public GameObject player;
    public bool timeDrainActive;
    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }
    
    void Update()
    {
        if (timeDrainActive)
        {
            transform.LookAt(player.transform);
            var main = ps.main;
            main.startLifetime = Vector3.Distance(transform.position, player.transform.position) / ps.main.startSpeed.constant;
        }

        if (Input.GetKeyDown("i"))
        {
            timeDrainActive = true;
        }
    }
}
