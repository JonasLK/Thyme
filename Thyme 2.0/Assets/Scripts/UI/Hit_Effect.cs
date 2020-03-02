using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hit_Effect : MonoBehaviour
{
    private GameObject canvas;
    public GameObject bloodScreenImage;
    public bool depleteAlpha;
    private float timer;
    public float maxTimer;
    public float camShakeDuration, camShakeMagnitude;
    public CamShake cameraWithCamShake;

    void Start()
    {
        canvas = gameObject;
        timer = maxTimer;
    }

    public void Update()
    {
        if (depleteAlpha == true)
        {
            timer -= Time.deltaTime;
        }

        if(timer < 0)
        {
            depleteAlpha = false;
            timer = maxTimer;
            bloodScreenImage.SetActive(false);
        }

        if (Input.GetKeyDown("l"))// testing purposes
        {
            DisplayBloodScreenImage();
        }
    }

    public void DisplayBloodScreenImage()
    {
        bloodScreenImage.SetActive(true);
        StartCoroutine(cameraWithCamShake.CustomScreenShake(camShakeDuration, camShakeMagnitude));
        depleteAlpha = true;
    }
}
/*active zetten
 * alpha geleidelijk naar beneden
 * dan uit
 */