using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ayy;
using LitJson;

public class HostRecord
{
    public string ip;
    public int port;
    public string displayName;
    public int joinedClientNum;
    public int maxClientNum;
}

public class MenuLanGame : MenuBase
{
    AyyHostListener hostListener = new AyyHostListener();

    RectTransform hostListView = null;
    
    Dictionary<string, HostRecord> aliveHostDict = new Dictionary<string, HostRecord>();
    Dictionary<string, GameObject> cellDict = new Dictionary<string, GameObject>();


    GameObject cellPrefab = null;

    void Start()
    {
        hostListView = transform.Find("Scroll View Host").Find("Viewport").Find("Content").GetComponent<RectTransform>();
        hostListener.Start();
        cellPrefab = Resources.Load<GameObject>("Menu/Panel_HostItem");
    }

    
    void Update()
    {
        if (hostListener != null)
        {
            lock (hostListener.messageList)
            {
                foreach (AyyHostListener.Message msg in hostListener.messageList)
                {
                    OnRecvData(msg.content);
                }
                hostListener.messageList.Clear();
            }
        }
    }


    private void OnDestroy()
    {
        // close listen host
        hostListener.Stop();
        hostListener = null;
    }

    public void OnClickCreateGame()
    {
        CmdCenter.GetInstance().RunCmd(new CmdCloseMenu(gameObject));

        Dictionary<string, object> arg = new Dictionary<string, object>();
        Dictionary<string, object> enterArg = new Dictionary<string, object>();
        arg.Add("menu_path", "Menu/MenuLobby");
        arg.Add("enter_arg", enterArg);
        enterArg.Add("self_host", true);
        CmdCenter.GetInstance().RunCmd(new CmdOpenMenu(arg));
    }

    public void OnClickBack()
    {
        CmdCenter.GetInstance().RunCmd(new CmdCloseMenu(gameObject));

        Dictionary<string, object> arg = new Dictionary<string, object>();
        arg.Add("menu_path", "Menu/MenuHome");
        CmdCenter.GetInstance().RunCmd(new CmdOpenMenu(arg));
    }


    private void OnRecvData(string content)
    {
        Debug.Log("[recv data] " + content);

        JsonData jd = JsonMapper.ToObject(content);
        string type = (string)jd["type"];
        switch (type)
        {
            case "alive":
                {
                    AyyHostBroadCaster.AliveMessage msg = JsonMapper.ToObject<AyyHostBroadCaster.AliveMessage>(content);
                    OnRecvHostAlive(msg);
                }
                break;
            case "cancel":
                {
                    AyyHostBroadCaster.CancelMessage msg = JsonMapper.ToObject<AyyHostBroadCaster.CancelMessage>(content);
                    OnRecvHostCancel(msg);
                }
                break;
        }
    }


    private void OnRecvHostAlive(AyyHostBroadCaster.AliveMessage msg)
    {
        string key = BuildHostKey(msg.ip,msg.port);

        HostRecord record = null;
        if (aliveHostDict.ContainsKey(key))
        {
            record = aliveHostDict[key];
            record.joinedClientNum = msg.playerNum;
            record.maxClientNum = msg.maxPlayerNum;
            UpdateCell(key,record);
        }
        else
        {
            record = new HostRecord();
            aliveHostDict.Add(key, record);

            record.ip = msg.ip;
            record.port = msg.port;
            record.joinedClientNum = msg.playerNum;
            record.maxClientNum = msg.maxPlayerNum;
            AddCell(key, record);
        }
    }


    private void OnRecvHostCancel(AyyHostBroadCaster.CancelMessage msg)
    {
        string key = BuildHostKey(msg.ip, msg.port);
        if (aliveHostDict.ContainsKey(key))
        {
            aliveHostDict.Remove(key);
            RemoveCell(key);
        }
    }

    private string BuildHostKey(string ip,int port)
    {
        return ip + port;
    }


    private void AddCell(string key,HostRecord record)
    {
        GameObject cell = GameObject.Instantiate(cellPrefab);
        cell.transform.SetParent(hostListView);
        cellDict.Add(key, cell);
        RefreshCell(cell,record);
    }

    private void UpdateCell(string key,HostRecord record)
    {
        GameObject cell = cellDict[key];
        RefreshCell(cell,record);
    }

    private void RemoveCell(string key)
    {
        GameObject cell = cellDict[key];
        cellDict.Remove(key);
        Destroy(cell);
    }

    private void RefreshCell(GameObject cell,HostRecord record)
    {

    }
}
