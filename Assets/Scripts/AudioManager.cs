using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioSource effects;
    public AudioSource music;
    public AudioSource menuMusic;
    public Sound[] sounds;
    private void Awake()
    {
        for(int i = 0; i < sounds.Length;i++)
        {
            Sound s = sounds[i];
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.clip = s.clip;
        }
    }
    public void PlayMusic()
    {
        music.Play();
    }
    public void StopMusic()
    {
        music.Stop();
    }
    public void PlayMenuMusic()
    {
        menuMusic.Play();
    }
    public void StopMenuMusic()
    {
        menuMusic.Stop();
    }
    public void Play(string soundName) 
    {
        
        for(int i = 0; i < sounds.Length;i++)
        {
            Sound s = sounds[i];
            if(s.soundName == soundName) 
            {
                //s.source.Play();
                effects.clip = s.clip;
                effects.Play();
                i = sounds.Length;
            }
        }
    }
    public void Stop(string soundName)
    {
        foreach (Sound s in sounds)
        {
            if (s.soundName == soundName)
            {
                s.source.Stop();
            }
        }
    }
}
[System.Serializable]
public class Sound
{
    public bool loop;
    public string soundName;
    public AudioClip clip;
    [Range(0f,1f)]
    public float volume;
    [Range(1f, 3f)]
    public float pitch;
    [HideInInspector]
    public AudioSource source;
}