﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShake : MonoBehaviour
{
    [Header("LowShake")]
    [SerializeField] float lowDuration;
    [SerializeField] float lowMagnitude;

    [Header("MedShake")]
    [SerializeField] float medDuration;
    [SerializeField] float medMagnitude;
    
    [Header("HighShake")]
    [SerializeField] float highDuration;
    [SerializeField] float highMagnitude;

    public IEnumerator CustomScreenShake(float dur,float mag)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < dur)
        {
            float x = Random.Range(-1f, 1f) * mag;
            float y = Random.Range(-1f, 1f) * mag;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }
        transform.localPosition = originalPos;
    }

    public IEnumerator LowScreenShake()
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < lowDuration)
        {
            float x = Random.Range(-1f, 1f) * lowMagnitude;
            float y = Random.Range(-1f, 1f) * lowMagnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }
        transform.localPosition = originalPos;
    }

    public IEnumerator MedScreenShake()
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < medDuration)
        {
            float x = Random.Range(-1f, 1f) * medMagnitude;
            float y = Random.Range(-1f, 1f) * medMagnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }
        transform.localPosition = originalPos;
    }

    public IEnumerator HighScreenShake()
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < highDuration)
        {
            float x = Random.Range(-1f, 1f) * highMagnitude;
            float y = Random.Range(-1f, 1f) * highMagnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }
        transform.localPosition = originalPos;
    }
}
