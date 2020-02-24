using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ayy
{
    public class AyyClient
    {
        private AyyNetwork _context;
        private NetworkClient _client = null;
        private int _sendCounter = 0;

        NetworkConnection _conn = null;

        public AyyClient(AyyNetwork context)
        {
            _context = context;
        }

        public void Start()
        {
            _client = new NetworkClient();
            _client.RegisterHandler(MsgType.Connect, OnConnectedServer);
            _client.RegisterHandler(MsgType.Disconnect, OnDisConnectedServer);
            _client.RegisterHandler(MsgType.Error, OnError);

            // lobby
            _client.RegisterHandler((int)CustomMsgType.Lobby_Server_Prepare, OnLobbyMsg);
            // gameplay
            _client.RegisterHandler((int)CustomMsgType.Game_Start, OnGameplayMsg);
            _client.RegisterHandler((int)CustomMsgType.Game_Player_Move, OnGameplayMsg);
            _client.RegisterHandler((int)CustomMsgType.Game_Player_Fire, OnGameplayMsg);
            // do start 
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

        public void ClientReady()
        {
            LobbyMessage msg = new LobbyMessage();
            msg.msgType = "client_ready";
            msg.content = "client_ready";
            _conn.Send((int)CustomMsgType.Lobby_Player_Ready,msg);
        }

        private void OnLobbyMsg(NetworkMessage netMsg)
        {
            LobbyMessage msg = new LobbyMessage();
            msg.Deserialize(netMsg.reader);
            _context.HandleMessage(msg);
        }

        private void OnGameplayMsg(NetworkMessage netMsg)
        {
            GameMessage msg = new GameMessage();
            msg.Deserialize(netMsg.reader);
            _context.HandleMessage(msg);
        }
    }
}

