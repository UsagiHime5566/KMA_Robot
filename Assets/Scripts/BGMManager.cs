using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BGMManager : MonoBehaviour
{
    public static BGMManager instance;
    public AudioSource bgm;
    public float fadeSpeed = 1f;  // 淡入淡出的速度
    public float endTimeThreshold = 1f;
    private float targetVolume = 1f;
    private bool isFading = false;

    // 添加 BGM 播放完畢事件
    public event Action OnBGMComplete;

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
        
        // 添加 BGM 播放完畢的監聽
        StartCoroutine(CheckBGMComplete());
    }

    // 檢查 BGM 是否播放完畢的協程
    private IEnumerator CheckBGMComplete()
    {
        while (true)
        {
            if (bgm.isPlaying && !bgm.loop && bgm.time >= bgm.clip.length - endTimeThreshold)
            {
                OnBGMComplete?.Invoke();
                yield break;
            }
            yield return new WaitForSeconds(0.1f);
        }
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
