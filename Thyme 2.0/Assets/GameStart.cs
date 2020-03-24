using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        GameManager.instance.gameStart = true;
        GameManager.instance.pillarMan.StartPylon();
        GetComponent<Collider>().isTrigger = false;
    }
}
