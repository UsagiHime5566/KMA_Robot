using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public ResourceManager resourceManager;
    public AudioSource AUD_Sound;
    public System.Action OnSoundEnd;
    private bool hasTriggeredEnd = false;

    void Awake()
    {
        resourceManager.OnSoundLoaded += OnSoundLoaded;
    }

    void Start()
    {
        StartCoroutine(CheckAudioEnd());
    }

    void OnSoundLoaded(AudioClip _clip){
        AUD_Sound.clip = _clip;
        if (_clip != null){
            AUD_Sound.Play();
            hasTriggeredEnd = false;
        } else {
            AUD_Sound.Stop();
        }
    }

    IEnumerator CheckAudioEnd()
    {
        while (true)
        {
            if (AUD_Sound.clip != null && !AUD_Sound.isPlaying && !hasTriggeredEnd)
            {
                OnSoundEnd?.Invoke();
                hasTriggeredEnd = true;
            }
            else if (AUD_Sound.isPlaying)
            {
                hasTriggeredEnd = false;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
