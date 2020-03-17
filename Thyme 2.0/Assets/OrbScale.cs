using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbScale : MonoBehaviour
{
    public float maxScale;
    public float scaleSpeed;
    public float decreaseSpeed;
    
    // Update is called once per frame
    public IEnumerator SizeIncrease()
    {
        ParticleSystem.ShapeModule shape = GetComponentInChildren<ParticleSystem>().shape;
        ParticleSystem.MainModule main = GetComponentInChildren<ParticleSystem>().main;
        ParticleSystem.EmissionModule emisson = GetComponentInChildren<ParticleSystem>().emission;
        while (transform.localScale.x < maxScale)
        {
            if(GameManager.gameTime > 0)
            {
                GameManager.gameTime -= Time.deltaTime;
            }
            if (GetComponentInChildren<ParticleSystem>())
            {
                main.startLifetimeMultiplier += 1 * scaleSpeed * Time.deltaTime;
                emisson.rateOverTimeMultiplier += 1 * scaleSpeed * Time.deltaTime;
                shape.radius += 1 * scaleSpeed * Time.deltaTime;
            }
            transform.localScale += new Vector3(1, 1, 1) * scaleSpeed * Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public IEnumerator SizeDecrease()
    {
        ParticleSystem.ShapeModule shape = GetComponentInChildren<ParticleSystem>().shape;
        ParticleSystem.MainModule main = GetComponentInChildren<ParticleSystem>().main;
        ParticleSystem.EmissionModule emisson = GetComponentInChildren<ParticleSystem>().emission;
        GetComponentInChildren<OrbRotation>().invert = true;
        while (transform.localScale.x > 0)
        {
            if (GameManager.gameTime < 1)
            {
                GameManager.gameTime = 1;
            }
            if (GetComponentInChildren<ParticleSystem>())
            {
                main.startLifetimeMultiplier -= 1 * decreaseSpeed * Time.deltaTime;
                emisson.rateOverTimeMultiplier -= 1 * decreaseSpeed * Time.deltaTime;
                shape.radius -= 1 * decreaseSpeed * Time.deltaTime;
            }
            transform.localScale -= new Vector3(1, 1, 1) * decreaseSpeed * Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        gameObject.SetActive(false);
    }
}
