using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using LitJson;


namespace ayy
{
    public class AyyHostBroadCaster
    {
        [SerializeField]
        public class AliveMessage
        {
            public string type;
            public string ip;
            public int port;
            public int playerNum;
            public int maxPlayerNum;
        }

        [SerializeField]
        public class CancelMessage
        {
            public string type;
            public string ip;
            public int port;
        }


        UdpClient udp = null;
        int port = 4321;
        Thread broadcastThread = null;

        string content = "{\"empty\":true}";
        IPEndPoint endPoint;

        // @temp
        int gamePort = 2333;
        int playerNum = 0;
        int maxPlayerNum = 5;
        
        public void Prepare()
        {
            udp = new UdpClient();
            endPoint = new IPEndPoint(IPAddress.Parse("255.255.255.255"), port);
        }

        public void Start()
        {
            ThreadStart ts = new ThreadStart(BroadCastLoop);
            broadcastThread = new Thread(ts);
            broadcastThread.Start();
        }

        public void Stop()
        {
            if (broadcastThread != null && broadcastThread.IsAlive)
            {
                broadcastThread.Abort();
            }

            // close message
            SetContent(BuildCancelMessage());
            DoSend();

            udp.Close();
            udp.Dispose();
            udp = null;
        }


        public void SetContent(string content)
        {
            this.content = content;
        }

        private void BroadCastLoop()
        {
            try
            {
                while (true)
                {
                    // alive message
                    SetContent(BuildAliveMessage());
                    DoSend();

                    Thread.Sleep(1000);
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log("[BroadCastLoop] " + ex.ToString());
            }
        }

        private void DoSend()
        {
            byte[] binaryData = System.Text.Encoding.UTF8.GetBytes(content);
            int len = binaryData.Length;
            udp.Send(binaryData, len, endPoint);
        }


        private string BuildAliveMessage()
        {

            AliveMessage msg = new AliveMessage();
            msg.type = "alive";
            msg.ip = GetIPAddress();
            msg.port = gamePort;
            msg.playerNum = playerNum;
            msg.maxPlayerNum = maxPlayerNum;
            string strJson = JsonMapper.ToJson(msg);
            return strJson;
        }

        private string BuildCancelMessage()
        {
            CancelMessage msg = new CancelMessage();
            msg.type = "cancel";
            msg.ip = GetIPAddress();
            msg.port = gamePort;
            string strJson = JsonMapper.ToJson(msg);
            return strJson;
        }


        private string GetIPAddress()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(hostName);
            for (int i = 0;i < ipEntry.AddressList.Length;i++)
            {
                IPAddress ipAddress = ipEntry.AddressList[i];
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ipAddress.ToString();
                }
            }
            return string.Empty;
        }
    }
}

