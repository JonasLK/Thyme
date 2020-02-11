using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPortals : MonoBehaviour
{
    public List<GameObject> locations;

    public float closeTime;

    public void Start()
    {
        Randomizer(closeTime);
    }

    public void Randomizer(float waitTime)
    {      
        StartCoroutine(NewSpawn(waitTime, Random.Range(0, locations.Count - 1)));
    }

    public IEnumerator NewSpawn(float waitTime, int i)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);

            Debug.Log("Time");
            //locations[i].GetComponent<Portal>().StartSpawn();

            yield return StartCoroutine(closePortal(waitTime, i));
        }
    }

    public IEnumerator closePortal(float waitTime, int i)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);

            Debug.Log("Closing");
            //locations[i].GetComponent<Portal>().Close();           

            yield return NewSpawn(waitTime, i);
        }
    }
}
