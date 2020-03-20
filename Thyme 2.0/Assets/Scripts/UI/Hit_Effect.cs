using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hit_Effect : MonoBehaviour
{
    public GameObject bloodScreenImage;
    public float camShakeDuration, camShakeMagnitude;
    public CamShake shake;

    public void DisplayBloodScreenImage()
    {
        bloodScreenImage.SetActive(true);
        StartCoroutine(shake.CustomScreenShake(camShakeDuration, camShakeMagnitude));
    }

    public void TurnOffDisplay()
    {
        bloodScreenImage.SetActive(false);
        StopCoroutine(shake.CustomScreenShake(camShakeDuration, camShakeMagnitude));
    }
}
/*active zetten
 * alpha geleidelijk naar beneden
 * dan uit
 */