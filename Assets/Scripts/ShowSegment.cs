using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
        float waitTime = KRGameManager.instance.showManager.waitTimeForRobotReset;
        Debug.Log($"開始第 {segmentIndex + 1} 個橋段, 等候{waitTime}秒等待手臂歸位...");

        KRGameManager.instance.animManager.animRobot = false;
        KRGameManager.instance.showManager.StartCoroutine(WaitForRobot(waitTime));
    }

    IEnumerator WaitForRobot(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
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