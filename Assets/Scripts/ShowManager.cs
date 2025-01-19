using UnityEngine;
using System.Collections.Generic;
using EasyButtons;

public class ShowManager : MonoBehaviour
{
    public float segmentDuration = 150f; // 每個橋段的持續時間
    private int currentSegmentIndex = 0;  // 當前橋段索引
    private float currentSegmentTimer = 0f;
    private bool isShowRunning = false;

    public List<ShowSegment> segments = new List<ShowSegment>();

    void Start()
    {
        if(!InitializeSegments()){
            return;
        }
    }

    void Update()
    {
        if (!isShowRunning) return;

        currentSegmentTimer += Time.deltaTime;
        
        // 發送時間更新事件
        ShowEvents.TriggerSegmentTimeUpdate(currentSegmentTimer);

        if (currentSegmentTimer >= segmentDuration)
        {
            NextSegment();
        }

        if (segments[currentSegmentIndex] != null)
        {
            segments[currentSegmentIndex].UpdateSegment(currentSegmentTimer);
        }
    }

    private bool InitializeSegments()
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
        UpdateUIInfo();
        segments[currentSegmentIndex].StartSegment();
    }

    private void NextSegment()
    {
        segments[currentSegmentIndex].EndSegment();
        currentSegmentIndex++;
        currentSegmentTimer = 0f;

        if (currentSegmentIndex >= segments.Count)
        {
            EndShow();
            return;
        }

        UpdateUIInfo();
        segments[currentSegmentIndex].StartSegment();
    }

    // 新增用於更新 UI 資訊的方法
    private void UpdateUIInfo()
    {
        ShowEvents.TriggerSegmentIndexUpdate(currentSegmentIndex, segments.Count);
        ShowEvents.TriggerSegmentFileNameUpdate(segments[currentSegmentIndex].loadFileName);
    }

    private void EndShow()
    {
        isShowRunning = false;
        Debug.Log("演出結束！");
    }
} 