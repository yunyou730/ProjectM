using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ayy
{
    public class AyyClient
    {

        private NetworkClient _client = null;
        private int _sendCounter = 0;

        NetworkConnection _conn = null;

        public void Start()
        {
            _client = new NetworkClient();
            _client.RegisterHandler(MsgType.Connect, OnConnectedServer);
            _client.RegisterHandler(MsgType.Disconnect, OnDisConnectedServer);
            _client.RegisterHandler(MsgType.Error, OnError);
            _client.Connect("127.0.0.1", 20086);
        }

        public void Close()
        {

        }

        private void OnConnectedServer(NetworkMessage netMsg)
        {
            Debug.Log("OnConnctedServer.[server]" + netMsg.conn.address);
            _conn = netMsg.conn;
        }

        private void OnDisConnectedServer(NetworkMessage netMsg)
        {
            Debug.Log(netMsg);
        }

        private void OnError(NetworkMessage netMsg)
        {
            Debug.Log(netMsg);
        }


        public void Send()
        {
            CustomMessage msg = new CustomMessage();
            msg._uuid = ++_sendCounter;
            msg._content = "ayyyyy";
            msg._vector = new Vector3(1,1,0);
            msg._bytes = new byte[100];
            _conn.Send(CustomMsgTypes.InGameMsg,msg);
        }
    }
}

