using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public ResourceManager resourceManager;
    public AudioSource AUD_Sound;
    void Awake()
    {
        resourceManager.OnSoundLoaded += OnSoundLoaded;
    }

    void OnSoundLoaded(AudioClip _clip){
        AUD_Sound.clip = _clip;
        if (_clip != null){
            AUD_Sound.Play();
        } else {
            AUD_Sound.Stop();
        }
    }
}
