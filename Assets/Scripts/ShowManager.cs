using UnityEngine;
using System.Collections.Generic;
using EasyButtons;
using UnityEngine.UI;
using System.Collections;

public class ShowManager : MonoBehaviour
{
    private int currentSegmentIndex = 0;  // 當前橋段索引
    private float currentSegmentTimer = 0f;
    private bool isShowRunning = false;

    public float atleastTotalTime = 150f;
    public float waitToNextTime = 40f;
    public float timeWhenSoundEnd = -1;


    [Header("Debug UI")]
    [SerializeField] Text timeText;
    [SerializeField] Text segmentIndexText;
    [SerializeField] Text fileNameText;

    public List<ShowSegment> segments = new List<ShowSegment>();


    void Start()
    {
        if(!InitializeSegments()){
            return;
        }

        KRGameManager.instance.audioManager.OnSoundEnd += OnSoundEnd;

        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(KRGameManager.instance.autoStartDelay);
        if(KRGameManager.instance.autoStart){
            StartShow();
        }
    }

    void Update()
    {
        if (!isShowRunning) return;

        currentSegmentTimer += Time.deltaTime;
        
        // 發送時間更新事件
        if (timeText != null)
            timeText.text = $"時間: {currentSegmentTimer:F1} 秒";

        if (segments[currentSegmentIndex] != null)
        {
            segments[currentSegmentIndex].UpdateSegment(currentSegmentTimer);
        }

        if(currentSegmentTimer > atleastTotalTime &&
            currentSegmentTimer - timeWhenSoundEnd > waitToNextTime){
            NextSegment();
        }
    }

    bool InitializeSegments()
    {
        if(segments.Count < 12){
            Debug.LogError("Segments count is less than 12");
            return false;
        }

        // 自動設置每個段落的 index
        for (int i = 0; i < segments.Count; i++)
        {
            segments[i] = new ShowSegment(i, segments[i].loadFileName);
        }
        return true;
    }

    [Button]
    public void StartShow()
    {
        isShowRunning = true;
        currentSegmentIndex = 0;
        currentSegmentTimer = 0f;
        timeWhenSoundEnd = 9999;
        UpdateUIInfo();
        segments[currentSegmentIndex].StartSegment();
    }

    private void NextSegment()
    {
        segments[currentSegmentIndex].EndSegment(currentSegmentTimer);
        currentSegmentIndex++;
        currentSegmentTimer = 0f;
        timeWhenSoundEnd = 9999;

        if (currentSegmentIndex >= segments.Count)
        {
            currentSegmentIndex = 0;
        }

        UpdateUIInfo();
        segments[currentSegmentIndex].StartSegment();
    }

    // 新增用於更新 UI 資訊的方法
    private void UpdateUIInfo()
    {
        if (segmentIndexText != null)
            segmentIndexText.text = $"橋段: {currentSegmentIndex + 1}/{segments.Count}";

        if (fileNameText != null)
            fileNameText.text = $"檔案: {segments[currentSegmentIndex].loadFileName}";
    }

    private void EndShow()
    {
        isShowRunning = false;
        Debug.Log("演出結束！");
    }

    private void OnSoundEnd()
    {
        KRGameManager.instance.animManager.animRobot = true;
        KRGameManager.instance.uiManager.LoadSequenceImage();

        timeWhenSoundEnd = currentSegmentTimer;
    }
} 