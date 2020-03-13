using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ControlVolume : MonoBehaviour
{
    public AudioMixer audiomixer;

    public void SetMasterVolume(float volume)
    {
        audiomixer.SetFloat("MasterVolume", volume);
    }
    
    public void SetMusicVolume(float volume)
    {
        audiomixer.SetFloat("MusicVolume", volume);
    }

    public void SetSfxVolume(float volume)
    {
        audiomixer.SetFloat("SFXVolume", volume);
    }
}