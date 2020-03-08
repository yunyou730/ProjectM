using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdNetworkConnectToServer : BaseCommand
{
    string serverIP = string.Empty;
    int serverPort = 0;
    ayy.AyyClient.DelegateConnectOK okcallback = null;

    public CmdNetworkConnectToServer(object arg) : base(arg)
    {
        HostRecord hostRecord = (HostRecord)arg;
        serverIP = hostRecord.ip;
        serverPort = hostRecord.port;
    }

    public void SetOKCallback(ayy.AyyClient.DelegateConnectOK okCb)
    {
        okcallback = okCb;
    }

    public override void Execute()
    {
        if (Home.GetInstance().network == null)
        {
            CmdCenter.GetInstance().RunCmd(new CmdInitNetwork(null));
        }
        Home.GetInstance().network.StartAsClient(serverIP, serverPort, okcallback);
    }
}
