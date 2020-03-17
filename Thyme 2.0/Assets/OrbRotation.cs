using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbRotation : MonoBehaviour
{
    public float rotateSpeed;
    public bool invert;
   
    void Update()
    {
        if (invert)
        {
            transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed);
        }
        else
        {
            transform.Rotate(Vector3.down * Time.deltaTime * rotateSpeed);
        }
    }
}
