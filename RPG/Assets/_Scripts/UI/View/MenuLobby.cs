using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ayy;
using UnityEngine.UI;

public class ClientItem
{
    string ip;
    string displayName;
}


public class MenuLobby : MenuBase
{
    bool bSelfHost = false;
    AyyHostBroadCaster broadCaster = null;

    Dictionary<string, ClientItem> clients = new Dictionary<string, ClientItem>();

    Button btnStart = null;

    private void Awake()
    {
        btnStart = transform.Find("BtnStart").GetComponent<Button>();
    }

    void Start()
    {
        // enterArg has be set
        bSelfHost = enterArg.ContainsKey("self_host");

        // start network
        if (bSelfHost)
        {
            CmdCenter.GetInstance().RunCmd(new CmdInitNetwork(null));

            // start broad cast
            broadCaster = new AyyHostBroadCaster();
            broadCaster.Prepare(Home.GetInstance().network);
            broadCaster.Start();
        }

        // toggle ui for host/client mode
        btnStart.gameObject.SetActive(bSelfHost);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnDestroy()
    {
        // server close notify host, disconnect all network session
        // client disconnect network session

        if (bSelfHost)
        {
            broadCaster.Stop();
            broadCaster = null;
        }
    }

    public void OnClickStart()
    {
        Debug.Log("Start");
    }

    public void OnClickQuit()
    {
        // close
        CmdCenter.GetInstance().RunCmd(new CmdCloseMenu(gameObject));
        // open parent
        Dictionary<string, object> arg = new Dictionary<string, object>();
        arg.Add("menu_path","Menu/MenuLanGame");
        CmdCenter.GetInstance().RunCmd(new CmdOpenMenu(arg));
    }
}
