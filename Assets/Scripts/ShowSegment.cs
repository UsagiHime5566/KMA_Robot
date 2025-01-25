using UnityEngine;

[System.Serializable]
public class ShowSegment
{
    public string loadFileName;
    private int segmentIndex;
    private float lastTriggerTime = -60f;
    private const float TRIGGER_INTERVAL = 60f;
    

    public ShowSegment(int index, string _fileName)
    {
        segmentIndex = index;
        loadFileName = _fileName;
    }

    public void StartSegment()
    {
        Debug.Log($"開始第 {segmentIndex + 1} 個橋段");
        lastTriggerTime = -60f;
        ShowEvents.TriggerShowSegmentStart(loadFileName);
    }

    public void UpdateSegment(float currentTime)
    {
        // 檢查是否經過30秒
        if (currentTime - lastTriggerTime >= TRIGGER_INTERVAL)
        {
            // 生成隨機觸發值 (0-10)
            int randomTrigger = Random.Range(0, 11);
            // 發送動畫觸發事件
            ShowEvents.TriggerAnimationUpdate(randomTrigger);
            // 更新最後觸發時間
            lastTriggerTime = currentTime;
            Debug.Log($"章節 {segmentIndex + 1}: Triggered at {currentTime:F1}s");
        }
    }

    public void EndSegment()
    {
        Debug.Log($"結束第 {segmentIndex + 1} 個橋段");
        // 在這裡添加橋段結束時的清理工作
    }
} 