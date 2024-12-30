using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class ImageUploader : MonoBehaviour
{
    private const string API_URL = "https://kfma.iottalk.tw/api/v1/epaper/1/image";
    private const string AUTH_TOKEN = "da31b188fa2949d2b507d520b60e0d14";

    public string imagePath = "Assets/Images/test.jpg";

    [EasyButtons.Button]
    public void TestUploadImage()
    {
        StartCoroutine(UploadImage(imagePath));
    }

    public IEnumerator UploadImage(string imagePath)
    {
        // 檢查文件是否存在
        if (!File.Exists(imagePath))
        {
            Debug.LogError($"找不到圖片文件: {imagePath}");
            yield break;
        }

        // 讀取圖片文件
        byte[] imageData = File.ReadAllBytes(imagePath);

        // 創建表單數據
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", imageData, "image.jpg", "multipart/form-data");

        // 創建請求
        using (UnityWebRequest request = UnityWebRequest.Post(API_URL, form))
        {
            // 添加認證頭
            request.SetRequestHeader("Authorization", $"Bearer {AUTH_TOKEN}");

            // 發送請求
            yield return request.SendWebRequest();

            // 檢查響應
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("上傳成功!");
                Debug.Log($"響應內容: {request.downloadHandler.text}");
            }
            else
            {
                Debug.LogError($"上傳失敗! 錯誤: {request.error}");
                Debug.LogError($"錯誤信息: {request.downloadHandler.text}");
            }
        }
    }

    // 使用示例
    public void StartImageUpload(string imagePath)
    {
        StartCoroutine(UploadImage(imagePath));
    }
} 