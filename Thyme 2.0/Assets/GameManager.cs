﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static float gameTime = 1f;

    [Header("Managers")]
    public SoundMan soundMan;
    public Waves waveMan;
    public PointManager pointMan;
    public NewUiManager uiMan;
    public ParticleMan particleMan;
    public PillarMan pillarMan;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
