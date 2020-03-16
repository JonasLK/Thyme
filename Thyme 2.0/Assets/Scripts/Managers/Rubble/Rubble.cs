using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rubble : MonoBehaviour
{
    public Vector3 movement,rotation;

    public float fallingSpeed, rotationSpeed;

    public bool falling;

    public void Start()
    {
        falling = true;
    }

    public void Update()
    {
        if (falling)
        {
            Falling(fallingSpeed, rotationSpeed);
        }
    }

    public void Falling(float fSpeed, float rSpeed)
    {
        Vector3 rot = rotation * Time.deltaTime * rSpeed;
        Vector3 mov = movement * Time.deltaTime * fSpeed;

        transform.position += mov;
        transform.Rotate(rot);
    }
}
