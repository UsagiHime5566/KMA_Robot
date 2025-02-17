using System.Collections;
using System.Collections.Generic;
using EasyButtons;
using UnityEngine;

public class KRGameManager : HimeLib.SingletonMono<KRGameManager>
{
    public bool useInternet = false;

    [Header("Managers")]
    public UIManager uiManager;
    public AudioManager audioManager;
    public AnimManager animManager;
    public ShowManager showManager;
  

    public bool autoStart = false;
    public float autoStartDelay = 5f;

    void Start()
    {
        uiManager.CanvasTurn(1);
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
            //Application.Quit();
        }
    }

    public void QuitApp(){
        Application.Quit();
    }
}

