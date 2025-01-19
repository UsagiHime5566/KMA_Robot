using System.Collections;
using System.Collections.Generic;
using EasyButtons;
using UnityEngine;

public class KRGameManager : HimeLib.SingletonMono<KRGameManager>
{
    public bool useInternet = false;

    [Header("Managers")]
    public UIManager uiManager;


    private void OnEnable()
    {
        ShowEvents.OnShowSegmentStart += LoadSingleFileAndShow;
    }

    private void OnDisable()
    {
        ShowEvents.OnShowSegmentStart -= LoadSingleFileAndShow;
    }

    void Start()
    {
        uiManager.CanvasTurn(1);
    }

    public void LoadSingleFileAndShow(string _fileName){
        uiManager.LoadShow(_fileName);
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.F7))
        {
            uiManager.CanvasTurn(0);
        }
        if (Input.GetKeyDown(KeyCode.F8))
        {
            uiManager.CanvasTurn(1);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}

