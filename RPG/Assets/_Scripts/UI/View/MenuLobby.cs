using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ayy;

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

    private void Awake()
    {
        Debug.Log("awake..");    
    }

    void Start()
    {
        // enterArg has be set
        bSelfHost = enterArg.ContainsKey("self_host");

        if (bSelfHost)
        {
            broadCaster = new AyyHostBroadCaster();
            broadCaster.Prepare();
            broadCaster.Start();
        }
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
