using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SignalSender : MonoBehaviour
{
    [Header("API")]
    public string BaseUrl = "kfma.iottalk.tw/api/v1";
    public string PaperAPI = "/epaper/{0}/image";
    public string RobotAPI = "/robot/{0}/angles";

    [Header("Component")]
    public ResourceManager resourceManager;
    public RobotValues robotValues1;
    public RobotValues robotValues2;


    [Header("Testing")]
    public RobotAngles testAngles;
    [EasyButtons.Button]
    public void TestSendRobotSignal()
    {
        SendRobotSignal(testAngles, 1);
    }

    void Awake()
    {   
        if (KRGameManager.instance.useInternet)
        {
            resourceManager.OnFileLoaded += OnFileLoaded;
            robotValues1.OnSendSignal += SendRobotSignal;
            robotValues2.OnSendSignal += SendRobotSignal;
        }
    }

        void OnFileLoaded(FileData fileData){
        if (fileData.extension == ".jpg"){
            SendPaperSignal(fileData.data,  $"{fileData.fileName}.{fileData.extension}", fileData.index + 1);   //紙張ID從1開始
        }
    }

    void SendPaperSignal(byte[] imageData, string fileName, int paperId)
    {
        string url = "https://" + BaseUrl + string.Format(PaperAPI, paperId);
        //Debug.Log(url);
        
        StartCoroutine(SendImageRequest(url, imageData, fileName));
    }

    private IEnumerator SendImageRequest(string url, byte[] imageData, string fileName)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", imageData, fileName, "multipart/form-data");

        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            request.SetRequestHeader("Authorization", "Bearer da31b188fa2949d2b507d520b60e0d14");
            
            yield return request.SendWebRequest();
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"上傳失敗! 錯誤: {request.error}");
                Debug.LogError($"錯誤信息: {request.downloadHandler.text}");
                KRGameManager.instance.uiManager.AddLog($"[Paper] 上傳失敗! 錯誤: {request.error}");
            }
            else
            {
                Debug.Log($"上傳成功!回應內容: {request.downloadHandler.text}");
            }
        }
    }

    public void SendRobotSignal(RobotAngles angles, int robotId)
    {
        string url = "https://" + BaseUrl + string.Format(RobotAPI, robotId);
        //Debug.Log(url);
        string json = JsonUtility.ToJson(angles);
        //Debug.Log(json);
        StartCoroutine(SendJsonRequest(url, json));
    }

    private IEnumerator SendJsonRequest(string url, string json)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", "Bearer da31b188fa2949d2b507d520b60e0d14");
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to send request: " + request.error);
                KRGameManager.instance.uiManager.AddLog($"[Robot] Failed to send request: {request.error}");
            }
            else
            {
                Debug.Log("Request sent successfully: " + url);
            }
        }
    }
}

