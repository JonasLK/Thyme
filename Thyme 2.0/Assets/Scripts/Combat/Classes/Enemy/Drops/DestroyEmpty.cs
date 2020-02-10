using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEmpty : MonoBehaviour
{
    public List<GameObject> cogs;//fill in by hand

    public void CheckIfEmpty(GameObject cog)
    {
        cogs.Remove(cog);
        if(cogs.Count == 0)
        {
            Destroy(gameObject);
        }
    }
}