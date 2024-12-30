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

    [Header("Component")]
    public ResourceManager resourceManager;

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

        for (int i = 0; i < listDirButton.Count; i++)
        {
            int index = i;
            listDirButton[i].onClick.AddListener(() => SetupDictionary(index));
        }
    }

    void OnChangeFileName(string _fileName){
        SystemConfig.Instance.SaveData("FileName", _fileName);
        resourceManager.FileName = _fileName;

        resourceManager.LoalAll(listOutputImage);
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


    public void CanvasTurn(){
        CG_System.blocksRaycasts = !CG_System.blocksRaycasts;
        CG_System.alpha = CG_System.alpha == 1 ? 0 : 1;
    }
}
