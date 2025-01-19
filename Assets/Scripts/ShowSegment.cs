using UnityEngine;

[System.Serializable]
public class ShowSegment
{
    public string loadFileName;
    private int segmentIndex;
    

    public ShowSegment(int index, string _fileName)
    {
        segmentIndex = index;
        loadFileName = _fileName;
    }

    public void StartSegment()
    {
        Debug.Log($"開始第 {segmentIndex + 1} 個橋段");
        // 使用事件系統替代直接呼叫
        ShowEvents.TriggerShowSegmentStart(loadFileName);
    }

    public void UpdateSegment(float currentTime)
    {
        // 在這裡添加橋段進行中的更新邏輯
    }

    public void EndSegment()
    {
        Debug.Log($"結束第 {segmentIndex + 1} 個橋段");
        // 在這裡添加橋段結束時的清理工作
    }
} 