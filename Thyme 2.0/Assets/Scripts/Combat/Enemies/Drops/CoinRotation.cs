using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinRotation : MonoBehaviour
{
    public Vector3 coinRotation;

    void Update()
    {
        transform.Rotate(coinRotation * Time.deltaTime);
    }
}
