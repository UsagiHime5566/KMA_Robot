using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System;

[Serializable]
public class FileData {
    public byte[] data;
    public string fileName;
    public string extension;
    public int index;
}

public class ResourceManager : MonoBehaviour
{
    [Header("Runtime Data")]
    public string FileName;
    public string ImageExtension;
    public string SoundExtension;
    public List<string> ListDirPath;
    [SerializeField] List<Texture2D> createdTexture;
    [SerializeField] List<Sprite> createdSprite;
    [SerializeField] List<AudioClip> createdAudioClips;

    private readonly string[] VALID_IMAGE_EXTENSIONS = { ".png", ".jpg", ".jpeg", ".bmp", ".tga" };
    private readonly Dictionary<string, AudioType> AUDIO_TYPE_MAP = new Dictionary<string, AudioType>
    {
        {".wav", AudioType.WAV},
        {".mp3", AudioType.MPEG},
        {".ogg", AudioType.OGGVORBIS}
    };

    public System.Action<FileData> OnFileLoaded;
    public System.Action<AudioClip> OnSoundLoaded;


    FileData tempFileData;

    void Awake()
    {
        createdTexture = new List<Texture2D>();
        createdSprite = new List<Sprite>();
        createdAudioClips = new List<AudioClip>();
    }

    public void LoalAll(List<Image> listOutputImage){
        ClearPreviousResources();

        LoadImages(FileName, listOutputImage);
        LoadSound(FileName);
    }

    public void LoadImages(string _fileName, List<Image> listOutputImage){
        for (int i = 0; i < listOutputImage.Count; i++)
        {
            tempFileData = new FileData();
            tempFileData.fileName = _fileName;
            tempFileData.index = i;

            try {
                string dirPath = ListDirPath[i];
                string file = Path.Combine(dirPath, $"{_fileName}.{ImageExtension}");
                
                if (File.Exists(file)) {
                    listOutputImage[i].sprite = LoadSprite(file);
                } else {
                    Debug.LogWarning($"File not found: {file}");
                    listOutputImage[i].sprite = null;
                }
            }
            catch (System.Exception e) {
                Debug.LogError($"Error loading image {i}: {e.Message}");
                listOutputImage[i].sprite = null;
            }
        }
    }

    Sprite LoadSprite(string filePath){
        try {
            string extension = Path.GetExtension(filePath).ToLower();
            if (!VALID_IMAGE_EXTENSIONS.Contains(extension))
            {
                Debug.LogError($"Invalid image format: {extension}. Supported formats: {string.Join(", ", VALID_IMAGE_EXTENSIONS)}");
                return null;
            }

            byte[] fileData = File.ReadAllBytes(filePath);
            tempFileData.data = fileData;   
            tempFileData.extension = extension;
            OnFileLoaded?.Invoke(tempFileData);

            Texture2D texture = new Texture2D(2, 2);
            if (!texture.LoadImage(fileData))
            {
                Debug.LogError($"Failed to load image: {filePath}");
                Destroy(texture);
                return null;
            }
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            createdSprite.Add(sprite);
            createdTexture.Add(texture);
            return sprite;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in LoadSprite: {e.Message}");
            return null;
        }
    }

    public async void LoadSound(string _fileName){
        string dirPath = ListDirPath.Last();        //共6個輸入處, 第6個是聲音
        if (dirPath == "Null")
        {
            Debug.LogWarning("Sound directory path not set");
            OnSoundLoaded?.Invoke(null);
            return;
        }

        string file = Path.Combine(dirPath, $"{_fileName}.{SoundExtension}");
        var clip = File.Exists(file) ? await LoadAudioClipAsync(file) : null;
        if (clip != null)
        {
            createdAudioClips.Add(clip);
        }
        OnSoundLoaded?.Invoke(clip);
    }


    private async Task<AudioClip> LoadAudioClipAsync(string filePath)
    {
        try {
            string extension = Path.GetExtension(filePath).ToLower();
            if (!AUDIO_TYPE_MAP.ContainsKey(extension))
            {
                Debug.LogError($"Unsupported audio format: {extension}. Supported formats: {string.Join(", ", AUDIO_TYPE_MAP.Keys)}");
                return null;
            }

            string url = "file://" + filePath;
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AUDIO_TYPE_MAP[extension]))
            {
                var operation = www.SendWebRequest();
                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (www.result == UnityWebRequest.Result.Success)
                {
                    return DownloadHandlerAudioClip.GetContent(www);
                }
                else
                {
                    Debug.LogError($"Failed to load audio: {www.error}");
                    return null;
                }
            }
        }
        catch (System.Exception e) {
            Debug.LogError($"Error in LoadAudioClipAsync: {e.Message}");
            return null;
        }
    }

    private void ClearPreviousResources()
    {
        foreach (var texture in createdTexture)
        {
            if (texture != null)
            {
                Destroy(texture);
            }
        }
        foreach (var sprite in createdSprite)
        {
            if (sprite != null)
            {
                Destroy(sprite);
            }
        }
        foreach (var clip in createdAudioClips)
        {
            if (clip != null)
            {
                Destroy(clip);
            }
        }
        createdTexture.Clear();
        createdSprite.Clear();
        createdAudioClips.Clear();
    }

    void OnDestroy()
    {
        ClearPreviousResources();
    }
}
