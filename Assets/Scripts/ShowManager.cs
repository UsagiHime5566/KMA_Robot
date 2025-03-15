using UnityEngine;
using System.Collections.Generic;
using EasyButtons;
using UnityEngine.UI;
using System.Collections;

public class ShowManager : MonoBehaviour
{
    [Header("Runtime")]
    public int currentSegmentIndex = 0;  // 當前橋段索引
    public float currentSegmentTimer = 0f;
    private bool isShowRunning = false;

    [Header("Show Settings")]
    public float atleastTotalTime = 150f;
    public float waitToNextTime = 40f;
    public float timeWhenSoundEnd = -1;
    public float waitTimeForRobotReset = 40;
    public float waitTimeParams = 1.5f;

    [Header("Control UI")]
    public InputField INP_Segment;
    public Button BTN_Start;


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

        INP_Segment.onEndEdit.AddListener( x => {
            if(int.TryParse(x, out int index)){
                index = index - 1;
                if(index >= 0 && index < segments.Count){
                    isShowRunning = true;
                    currentSegmentIndex = index;
                    currentSegmentTimer = 0f;
                    timeWhenSoundEnd = 9999;
                    UpdateUIInfo();
                    segments[currentSegmentIndex].StartSegment();
                }
            }
            INP_Segment.text = "";
        });

        BTN_Start.onClick.AddListener(() => {
            Debug.Log("StartShow Manual");
            StartShow();
        });
        TimeArrange.instance.OnTimeFlagChanged += (x, y) => {
            Debug.Log($"TimeFlagChanged: {x.playMode} - {x.timeRange.startTimeString} - {x.timeRange.endTimeString}");
            KRGameManager.instance.uiManager.AddLog($"TimeFlagChanged: {x.playMode} - {x.timeRange.startTimeString} - {x.timeRange.endTimeString}");
            if(x.playMode == TimeArrange.PlayMode.OnlyGame){
                StartShow();
            }
        };

        //StartCoroutine(DelayedStart());
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
            waitTimeForRobotReset = (currentSegmentTimer - timeWhenSoundEnd) * waitTimeParams;
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
        if(isShowRunning) return;

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
            KRGameManager.instance.animManager.animRobot = false;
            EndShow();
            return;
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
        BGMManager.instance.FadeOut();
        Debug.Log("演出結束！");
    }

    private void OnSoundEnd()
    {
        KRGameManager.instance.animManager.animRobot = true;

        int nextFileIndex = (currentSegmentIndex + 1) % segments.Count;
        KRGameManager.instance.uiManager.LoadSequenceImage(segments[nextFileIndex].loadFileName);

        timeWhenSoundEnd = currentSegmentTimer;
    }
} 