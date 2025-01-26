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

        KRGameManager.instance.animManager.animRobot = false;
        KRGameManager.instance.uiManager.LoadSequenceShow(loadFileName);
    }

    public void UpdateSegment(float currentTime)
    {
        
    }

    public void EndSegment(float currentTime)
    {
        Debug.Log($"結束第 {segmentIndex + 1} 個橋段於 {currentTime} 秒");
    }
} 