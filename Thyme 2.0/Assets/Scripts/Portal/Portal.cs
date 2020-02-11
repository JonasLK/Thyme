using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform portalSpawn;
    public Transform firstPatrolPoint;

    public void Start()
    {
        StartSpawn();
    }

    public void StartSpawn()
    {
        StartCoroutine(SpawnEnemy(GameManager.instance.waveMan.wave[GameManager.instance.waveMan.curWave].enemy));
    }

    public IEnumerator SpawnEnemy(GameObject entity)
    {
        while (true)
        {
            GameObject curEnemy = Instantiate(entity, portalSpawn.position, portalSpawn.rotation);
            curEnemy.GetComponent<Chase>().point = firstPatrolPoint;
            yield return new WaitForSeconds(GameManager.instance.waveMan.spawndelay);
        }
    }

    public void Close()
    {
        //Play Closing Animation
        //Destroy Portal
        StopAllCoroutines();
        Debug.Log("Closed");
    }
}


