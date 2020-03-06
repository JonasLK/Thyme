using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepList : MonoBehaviour
{

    public List<AudioClip> leftFoot = new List<AudioClip>();
    public List<AudioClip> rightFoot = new List<AudioClip>();

    public int Randomizer()
    {
        int randomNumber = Random.Range(0,leftFoot.Count);
        return randomNumber;
    }
}
