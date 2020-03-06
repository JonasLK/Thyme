using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    public Transform nextPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Enemy")
        {
            if (nextPoint != null)
            {
                other.transform.GetComponent<Chase>().point = nextPoint;
            }
        }
    }
}
