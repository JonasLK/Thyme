using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbScale : MonoBehaviour
{
    public float maxScale;
    public float scaleSpeed;
    public float decreaseSpeed;

    public PlayerMovement playerM;
    private float tempDam;
    private Vector3 tempDir;
    private Vector3 tempUpDir;
    private float tempAmount;
    private float tempUpAmount;
    private void Awake()
    {
        playerM = FindObjectOfType<PlayerMovement>();
    }
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
            else
            {
                GameManager.instance.bInfo.drainChase = true;
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
            if (GetComponentInChildren<ParticleSystem>())
            {
                main.startLifetimeMultiplier -= 1 * decreaseSpeed * Time.deltaTime;
                emisson.rateOverTimeMultiplier -= 1 * decreaseSpeed * Time.deltaTime;
                shape.radius -= 1 * decreaseSpeed * Time.deltaTime;
            }
            transform.localScale -= new Vector3(1, 1, 1) * decreaseSpeed * Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        playerM.DoDamage(tempDam);
        playerM.AddForceToPlayer(tempUpDir, tempUpAmount);
        playerM.AddForceToPlayer(tempDir, tempAmount);
        GameManager.gameTime = 1;
        GameManager.instance.pillarMan.NextPylon();
        Destroy(gameObject);
    }
    public void HoldDamage(float dam)
    {
        tempDam = dam;
    }
    public void HoldForce(Vector3 dir, float amount)
    {
        tempDir = dir;
        tempAmount = amount;
    }
    public void HoldUpforce(Vector3 dir,float amount)
    {
        tempUpDir = dir;
        tempUpAmount = amount;
    }
}
