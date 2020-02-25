using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ayy;
using UnityEngine.UI;

public class MenuNetwork : MonoBehaviour
{
    public AyyNetwork _network = null;
    Text lockstepTurnIndexLabel = null;

    // Start is called before the first frame update
    void Start()
    {
        _network.GameTurnEvent += OnLockstepTurn;
        lockstepTurnIndexLabel = transform.Find("Label_TurnIndex").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnClickStartServer()
    {
        _network.StartAsServer();
    }

    public void OnClickServerStartGame()
    {
        _network.ServerStartGame();
    }

    public void OnClickStartClient()
    {
        _network.StartAsClient();
    }

    public void OnClickClientSend()
    {
        _network._client.Send();
    }

    private void OnLockstepTurn(int turnIndex,string json)
    {
        lockstepTurnIndexLabel.text = "lockstep turn:" + turnIndex;
    }
}
