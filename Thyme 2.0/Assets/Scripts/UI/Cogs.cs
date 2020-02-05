using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cogs : MonoBehaviour
{
    public int cogs;
    public Text cogValueText;

    public void Start()
    {
        UpdateCogText();
    }

    public void UpdateCogValueMinus(int cogsToRetract)
    {
        cogs -= cogsToRetract;
    }
    
    public void UpdateCogValuePlus(int cogsToAdd)
    {
        cogs += cogsToAdd;
    }

    public void UpdateCogText()
    {
        cogValueText.text = cogs.ToString();
    }
}