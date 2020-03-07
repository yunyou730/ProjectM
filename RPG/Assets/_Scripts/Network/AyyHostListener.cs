using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;



public class AyyHostListener
{
    UdpClient udp = null;
    Thread thread = null;
    int port = 4321;
    IPEndPoint ipEndPoint = null;

    public void Start()
    {
        udp = new UdpClient(new IPEndPoint(IPAddress.Any, port));
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
        try {
            while (true)
            {
                if (udp != null)
                {
                    byte[] bytes = udp.Receive(ref ipEndPoint);
                    string str = System.Text.Encoding.UTF8.GetString(bytes);
                    Debug.Log("[RecvLoop] " + str);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("[RecvLoop] " + ex.ToString());
        }
    }
}
