using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public bool b;
    public Vector3 v;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(b == false)
        {
            transform.Rotate(v * Time.deltaTime);
        }
        else
        {
            transform.Rotate(-v * Time.deltaTime);
        }

        if(transform.rotation.x > 90)
        {
            b = true;
        }
        else if(transform.rotation.x < -90)
        {
            b = false;
        }
    }
}
