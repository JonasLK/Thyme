using UnityEngine;
using System;
using UnityEngine.Audio;

public class SoundMan : MonoBehaviour
{
    public Sound[] sounds;
    private void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void Start()
    {
        StopAllMusic();
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " Not Found");
            return;
        }
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " Not Found");
            return;
        }
        s.source.Stop();
    }

    public void ChangeClip(string name,AudioClip newClip)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " Not Found");
            return;
        }
        s.source.clip = newClip;
    }

    public bool IsPlaying(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null)
        {
            Debug.LogWarning("Sound: " + name + " Not Found");
            return false;
        }
        return s.source.isPlaying;
    }

    public void StopAllMusic()
    {
        foreach (Sound s in sounds)
        {
            s.source.Stop();
        }
    }

    [Serializable]
    public class Sound
    {
        public string name;

        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume;
        [Range(0.1f, 3f)]
        public float pitch;

        public bool loop;

        [HideInInspector]
        public AudioSource source;
    }
}
