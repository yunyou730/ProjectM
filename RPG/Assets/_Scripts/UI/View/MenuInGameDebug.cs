using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class MenuInGameDebug : MenuBase
{
    private ayy.AyyNetwork network = null;
    private Text lockframeStepLabel = null;
    
    void Start()
    {
        lockframeStepLabel = transform.Find("Text_LockFrameTurn").GetComponent<Text>();
        
        network = Home.GetInstance().network;
        network.GameTurnEvent += OnLockStepTurn;
    }

    private void OnDestroy()
    {
        network.GameTurnEvent -= OnLockStepTurn;
    }

    void OnLockStepTurn(int turnIndex,string turnJson)
    {
        lockframeStepLabel.text = "[lockstep turn] " + turnIndex.ToString();
    }
}
