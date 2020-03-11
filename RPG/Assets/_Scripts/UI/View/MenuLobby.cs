using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ayy;
using UnityEngine.UI;

public class ClientItem
{
    public int playerId;
}


public class MenuLobby : MenuBase
{
    bool bSelfHost = false;
    AyyHostBroadCaster broadCaster = null;

    Dictionary<int, ClientItem> clientMap = new Dictionary<int, ClientItem>();
    Dictionary<int, GameObject> clientCellMap = new Dictionary<int, GameObject>();

    Button btnStart = null;
    RectTransform viewContent = null; 

    private void Awake()
    {
        btnStart = transform.Find("BtnStart").GetComponent<Button>();
        viewContent = transform.Find("Scroll View").Find("Viewport").Find("Content").GetComponent<RectTransform>();
    }

    void Start()
    {
        // enterArg has be set
        bSelfHost = enterArg.ContainsKey("self_host");

        // start network
        if (bSelfHost)
        {
            CmdCenter.GetInstance().RunCmd(new CmdInitNetwork(null));
            MapEvent();

            // start broad cast
            broadCaster = new AyyHostBroadCaster();
            broadCaster.Prepare(Home.GetInstance().network);
            broadCaster.Start();
        }
        else
        {
            MapEvent();
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
        UnMapEvent();
        if (bSelfHost)
        {
            broadCaster.Stop();
            broadCaster = null;
        }
    }


    private void MapEvent()
    {
        Home.GetInstance().network.GameStartEvent += OnGameStarted;
        Home.GetInstance().network.PlayerJoinEvent += OnPlayerJoin;
        Home.GetInstance().network.PlayerLeftEvent += OnPlayerLeft;
        Home.GetInstance().network.PlayerListEvent += OnPlayerList;
    }

    private void UnMapEvent()
    {
        if (Home.GetInstance().network != null)
        {
            Home.GetInstance().network.GameStartEvent -= OnGameStarted;
            Home.GetInstance().network.PlayerJoinEvent -= OnPlayerJoin;
            Home.GetInstance().network.PlayerLeftEvent -= OnPlayerLeft;
            Home.GetInstance().network.PlayerListEvent -= OnPlayerList;
        }
    }

    public void OnClickStart()
    {
        Home.GetInstance().network.ServerStartGame();
    }

    public void OnClickQuit()
    {
        // close
        CmdCenter.GetInstance().RunCmd(new CmdCloseMenu(gameObject));
        // disconnect network
        CmdCenter.GetInstance().RunCmd(new CmdCloseNetwork(null));
        // open parent
        Dictionary<string, object> arg = new Dictionary<string, object>();
        arg.Add("menu_path","Menu/MenuLanGame");
        CmdCenter.GetInstance().RunCmd(new CmdOpenMenu(arg));
    }

    private void OnGameStarted()
    {
        Destroy(gameObject);
    }


    private void OnPlayerJoin(int playerId)
    {
        Debug.Log("Player join id:" + playerId);
        if (!clientMap.ContainsKey(playerId))
        {
            ClientItem clientItem = new ClientItem();
            clientItem.playerId = playerId;
            clientMap[playerId] = clientItem;
            HandleClientExist(clientItem);
        }
    }

    private void OnPlayerLeft(int playerId)
    {
        Debug.Log("Player left id:" + playerId);
        if (clientMap.ContainsKey(playerId))
        {
            ClientItem clientItem = clientMap[playerId];
            HandleClientLeft(clientItem);
            clientMap.Remove(playerId);
        }
    }

    private void OnPlayerList(List<int> playerList)
    {
        for (int i = 0;i < playerList.Count;i++)
        {
            Debug.Log("player list [" + playerList[i] + "]");
            int playerId = playerList[i];
            if (!clientMap.ContainsKey(playerId))
            {
                ClientItem clientItem = new ClientItem();
                clientItem.playerId = playerId;
                clientMap[playerId] = clientItem;
                HandleClientExist(clientItem);
            }
        }
    }


    private void HandleClientExist(ClientItem clientItem)
    {
        if (clientCellMap.ContainsKey(clientItem.playerId))
        {
            return;
        }
        GameObject prefab = Resources.Load<GameObject>("Menu/Panel_Player");
        GameObject go = GameObject.Instantiate(prefab);
        go.transform.parent = viewContent;
        clientCellMap.Add(clientItem.playerId,go);

        // Set Cell View
        Text nameLabel = go.transform.Find("Text_Name").GetComponent<Text>();
        nameLabel.text = "[player id]" + clientItem.playerId; 
    }

    private void HandleClientLeft(ClientItem clientItem)
    {
        if (!clientCellMap.ContainsKey(clientItem.playerId))
        {
            return;
        }
        GameObject go = clientCellMap[clientItem.playerId];
        Destroy(go);
    }
}
