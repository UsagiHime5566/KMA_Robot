using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TriLibCore.SFB;

public class UIManager : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup CG_System;
    public InputField INP_FileName;
    public InputField INP_ImageExtension;
    public InputField INP_SoundExtension;
    public List<InputField> listDirPathInput;   //共6個輸入處
    public List<Button> listDirButton;          //共6個輸入處
    public List<Image> listOutputImage;         //共5個輸出圖片
    public Button BTN_Update;
    [SerializeField] private Text timeText;
    [SerializeField] private Text segmentIndexText;
    [SerializeField] private Text fileNameText;
    public Text TXT_DebugLog;
    public Toggle TOG_AutoStart;
    public Button BTN_Manual;


    [Header("Materials")]
    public Material mat_ScreenA;
    public Material mat_ScreenB;
    public Material mat_ScreenC;
    public Material mat_ScreenD;
    public Material mat_ScreenE;

    [Header("Component")]
    public ResourceManager resourceManager;
    public Animator animatorA;
    public Animator animatorB;
    public RobotValues robotValuesA;
    public RobotValues robotValuesB;

    [Header("Debug")]
    public int maxLog = 3;

    Queue<string> logQueue = new Queue<string>();

    private void OnEnable()
    {
        ShowEvents.OnSegmentTimeUpdate += UpdateTimeUI;
        ShowEvents.OnSegmentIndexUpdate += UpdateSegmentIndexUI;
        ShowEvents.OnSegmentFileNameUpdate += UpdateFileNameUI;
    }

    private void OnDisable()
    {
        ShowEvents.OnSegmentTimeUpdate -= UpdateTimeUI;
        ShowEvents.OnSegmentIndexUpdate -= UpdateSegmentIndexUI;
        ShowEvents.OnSegmentFileNameUpdate -= UpdateFileNameUI;
    }

    private void UpdateTimeUI(float currentTime)
    {
        if (timeText != null)
            timeText.text = $"時間: {currentTime:F1} 秒";
    }

    private void UpdateSegmentIndexUI(int currentIndex, int totalSegments)
    {
        if (segmentIndexText != null)
            segmentIndexText.text = $"橋段: {currentIndex + 1}/{totalSegments}";
    }

    private void UpdateFileNameUI(string fileName)
    {
        if (fileNameText != null)
            fileNameText.text = $"檔案: {fileName}";
    }

    void Start()
    {
        resourceManager.ListDirPath = new List<string>();
        for (int i = 0; i < listDirPathInput.Count; i++)
        {
            int index = i;
            listDirPathInput[i].onValueChanged.AddListener( x => {
                SystemConfig.Instance.SaveData($"dir{index}", x);
                resourceManager.ListDirPath[index] = x;
            });

            resourceManager.ListDirPath.Add(SystemConfig.Instance.GetData<string>($"dir{i}", "Null"));
            listDirPathInput[i].text = resourceManager.ListDirPath[i];
        }

        INP_ImageExtension.onValueChanged.AddListener(OnChangeImageExtension);  
        INP_ImageExtension.text = SystemConfig.Instance.GetData<string>("imgExt", "Null");
        INP_SoundExtension.onValueChanged.AddListener(OnChangeSoundExtension);
        INP_SoundExtension.text = SystemConfig.Instance.GetData<string>("sndExt", "Null");

        //先載入副檔名, 再載入檔名, 預防副檔名錯誤
        INP_FileName.onValueChanged.AddListener(OnChangeFileName);
        INP_FileName.text = SystemConfig.Instance.GetData<string>("FileName", "Null");
        

        BTN_Update.onClick.AddListener(() => {
            resourceManager.LoalAll(listOutputImage);
        });

        TOG_AutoStart.onValueChanged.AddListener(x => {
            SystemConfig.Instance.SaveData("AutoStart", x);
            KRGameManager.instance.autoStart = x;
        });
        TOG_AutoStart.isOn = SystemConfig.Instance.GetData<bool>("AutoStart", true);

        BTN_Manual.onClick.AddListener(() => {
            animatorA.enabled = false;
            animatorB.enabled = false;
            robotValuesA.enableSendSignal = true;
            robotValuesB.enableSendSignal = true;
        });


        for (int i = 0; i < listDirButton.Count; i++)
        {
            int index = i;
            listDirButton[i].onClick.AddListener(() => SetupDictionary(index));
        }
    }

    public void LoadShow(string _fileName){
        INP_FileName.text = _fileName;
        resourceManager.LoalAll(listOutputImage);
        if(listOutputImage[0].sprite && listOutputImage[1].sprite && listOutputImage[2].sprite && listOutputImage[3].sprite && listOutputImage[4].sprite)
        {
            mat_ScreenA.SetTexture("_BaseMap", listOutputImage[0].sprite.texture);
            mat_ScreenB.SetTexture("_BaseMap", listOutputImage[1].sprite.texture);
            mat_ScreenC.SetTexture("_BaseMap", listOutputImage[2].sprite.texture);
            mat_ScreenD.SetTexture("_BaseMap", listOutputImage[3].sprite.texture);
            mat_ScreenE.SetTexture("_BaseMap", listOutputImage[4].sprite.texture);
        }
    }

    void OnChangeFileName(string _fileName){
        SystemConfig.Instance.SaveData("FileName", _fileName);
        resourceManager.FileName = _fileName;
    }

    void OnChangeImageExtension(string _imageExtension){
        SystemConfig.Instance.SaveData("imgExt", _imageExtension);
        resourceManager.ImageExtension = _imageExtension;
    }

    void OnChangeSoundExtension(string _soundExtension){
        SystemConfig.Instance.SaveData("sndExt", _soundExtension);
        resourceManager.SoundExtension = _soundExtension;
    }

    void SetupDictionary(int index)
    {
        var result = StandaloneFileBrowser.OpenFolderPanel("Open Path", Application.dataPath, false);
        if (result != null)
        {
            var hasFiles = result.Count > 0 && result[0].HasData;

            if (hasFiles)
            {
                string dirPath = result[0].Name;

                if (listDirPathInput[index]) listDirPathInput[index].text = dirPath;
                SystemConfig.Instance.SaveData($"dir{index}", dirPath);
                resourceManager.ListDirPath[index] = dirPath;
                Debug.Log("Success");
            }
        }
    }


    public void CanvasTurn(int index){
        if(index == 0){
            CG_System.blocksRaycasts = false;
            CG_System.alpha = 0;
        }
        if(index == 1){
            CG_System.blocksRaycasts = true;
            CG_System.alpha = 1;
        }
    }

    public void AddLog(string log){
        logQueue.Enqueue(log);
        if(logQueue.Count > maxLog){
            logQueue.Dequeue();
        }
        TXT_DebugLog.text = string.Join("\n", logQueue);
    }
}
