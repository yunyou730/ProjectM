using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;



public class AyyHostListener
{
    public class Message
    {
        public string content;
    }


    UdpClient udp = null;
    Thread thread = null;
    int port = 4321;
    IPEndPoint ipEndPoint = null;

    public delegate void RecvHostDelegate(string content);
    public List<Message> messageList = new List<Message>();
    
    public void Start()
    {
        udp = new UdpClient(port);
        ipEndPoint = new IPEndPoint(IPAddress.Any, port);

        ThreadStart ts = new ThreadStart(RecvLoop);
        thread = new Thread(ts);
        thread.Start();
    }

    public void Stop()
    {
        if (thread != null && thread.IsAlive)
        {
            thread.Abort();
        }

        udp.Close();
        udp.Dispose();
        udp = null;
    }

    private void RecvLoop()
    {
        try
        {
            while (true)
            {
                byte[] bytes = udp.Receive(ref ipEndPoint);
                string content = System.Text.Encoding.UTF8.GetString(bytes);

                lock (messageList)
                {
                    Message msg = new Message();
                    msg.content = content;
                    messageList.Add(msg);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("[RecvLoop] " + ex.ToString());
        }
    }
}
