using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public bool b;
    public float rotation;
    public Vector3 v;
    public float reverseTime;
    public float rotateSpeed;
    // Update is called once per frame
    void Update()
    {
        if (rotation >= reverseTime)
        {
            b = true;
        }
        else if (rotation <= -reverseTime)
        {
            b = false;
        }
        if (!b)
        {
            transform.Rotate(v * Time.deltaTime);
            rotation += (v.x * Time.deltaTime);
        }
        else
        {
            transform.Rotate(-v * Time.deltaTime);
            rotation -= (v.x * Time.deltaTime);
        }
    }
}
