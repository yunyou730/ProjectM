using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace ayy
{
    public class AyyServer
    {
        Dictionary<int, NetworkConnection> _connMap = new Dictionary<int, NetworkConnection>();

        public bool Start()
        {
            NetworkServer.RegisterHandler(MsgType.Connect, OnClientConnected);
            NetworkServer.RegisterHandler(MsgType.Disconnect, OnClientDisconnected);
            NetworkServer.RegisterHandler(MsgType.Error, OnError);
            NetworkServer.RegisterHandler(CustomMsgTypes.InGameMsg, OnInGameMsg);
            bool success = NetworkServer.Listen(20086);
            if (success)
            {
                Debug.Log("Server start success.");
            }
            else
            {
                Debug.Log("Server start failed.");
            }
            Debug.Log("Fix Update dt:" + Time.fixedDeltaTime);
            return success;
        }

        public void Close()
        {

        }
        
        private void OnClientConnected(NetworkMessage netMsg)
        {
            Debug.Log("OnClientConnected.[client]" + netMsg.conn.address);
            _connMap.Add(netMsg.conn.connectionId,netMsg.conn);
        }

        private void OnClientDisconnected(NetworkMessage netMsg)
        {
            Debug.Log(netMsg);
        }

        private void OnError(NetworkMessage netMsg)
        {
            Debug.Log(netMsg);
        }

        private void OnInGameMsg(NetworkMessage netMsg)
        {
            CustomMessage msg = new CustomMessage();
            msg.Deserialize(netMsg.reader);
            Debug.Log("OnInGameMsg:" + msg.ToString());
        }
    }
}
