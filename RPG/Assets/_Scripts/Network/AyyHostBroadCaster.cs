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
        UdpClient udp = null;
        int port = 4321;
        Thread broadcastThread = null;

        string content = "{\'empty\':true}";
        IPEndPoint endPoint;
        

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
            udp.Close();
            udp.Dispose();
            udp = null;

            if (broadcastThread != null && broadcastThread.IsAlive)
            {
                broadcastThread.Abort();
            }
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
                    byte[] binaryData = System.Text.Encoding.UTF8.GetBytes(content);
                    int len = binaryData.Length;
                    if (udp != null)
                    {
                        udp.Send(binaryData, len, endPoint);
                    }
                    Thread.Sleep(1000);
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log("[BroadCastLoop] " + ex.ToString());
            }
        }
    }
}

