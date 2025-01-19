using UnityEngine;
using System;

public static class ShowEvents
{
    public static event Action<string> OnShowSegmentStart;
    // 新增用於 UI 更新的事件
    public static event Action<float> OnSegmentTimeUpdate;
    public static event Action<int, int> OnSegmentIndexUpdate; // 當前索引, 總段落數
    public static event Action<string> OnSegmentFileNameUpdate;
    
    public static void TriggerShowSegmentStart(string fileName)
    {
        OnShowSegmentStart?.Invoke(fileName);
    }

    public static void TriggerSegmentTimeUpdate(float currentTime)
    {
        OnSegmentTimeUpdate?.Invoke(currentTime);
    }

    public static void TriggerSegmentIndexUpdate(int currentIndex, int totalSegments)
    {
        OnSegmentIndexUpdate?.Invoke(currentIndex, totalSegments);
    }

    public static void TriggerSegmentFileNameUpdate(string fileName)
    {
        OnSegmentFileNameUpdate?.Invoke(fileName);
    }
} 