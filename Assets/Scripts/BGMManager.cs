using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager instance;
    public AudioSource bgm;
    public float fadeSpeed = 1f;  // 淡入淡出的速度
    private float targetVolume = 1f;
    private bool isFading = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        if (bgm == null)
        {
            bgm = GetComponent<AudioSource>();
        }
        bgm.volume = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFading)
        {
            bgm.volume = Mathf.MoveTowards(bgm.volume, targetVolume, fadeSpeed * Time.deltaTime);
            
            if (Mathf.Approximately(bgm.volume, targetVolume))
            {
                isFading = false;
                if (targetVolume == 0f)
                {
                    bgm.Stop();
                }
            }
        }
    }

    [ContextMenu("FadeIn")]
    public void FadeIn()
    {
        targetVolume = 1f;
        isFading = true;
        if (!bgm.isPlaying)
        {
            bgm.Play();
        }
    }

    [ContextMenu("FadeOut")]
    public void FadeOut()
    {
        targetVolume = 0f;
        isFading = true;
    }
}
