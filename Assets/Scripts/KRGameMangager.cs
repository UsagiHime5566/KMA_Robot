using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KRGameMangager : HimeLib.SingletonMono<KRGameMangager>
{
    [Header("Managers")]
    public UIManager uiManager;

    void Start()
    {
        
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.F8))
        {
            uiManager.CanvasTurn();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}

