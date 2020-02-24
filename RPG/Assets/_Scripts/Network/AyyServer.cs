using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ayy
{

    public class ClientRecord
    {
        public bool bReady = false;
        public NetworkConnection conn = null;
    }

    public class AyyServer
    {
        AyyNetwork _context = null;
        Dictionary<int, ClientRecord> _clientMap = new Dictionary<int, ClientRecord>();

        int _readyCount = 0;
        int _lobbyMsgCounter = 0;
        int _lockstepTurnIndex = 0;

        public AyyServer(AyyNetwork context)
        {
            _context = context;
        }

        public bool Start()
        {
            NetworkServer.RegisterHandler(MsgType.Connect, OnClientConnected);
            NetworkServer.RegisterHandler(MsgType.Disconnect, OnClientDisconnected);
            NetworkServer.RegisterHandler(MsgType.Error, OnError);
            NetworkServer.RegisterHandler(CustomMsgTypes.InGameMsg, OnInGameMsg);

            NetworkServer.RegisterHandler((short)CustomMsgType.Lobby_Player_Ready,OnPlayerReady);

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

        public void StartGame()
        {
            LobbyMessage msg = new LobbyMessage();
            msg.messageId = ++_lobbyMsgCounter;
            msg.msgType = "game_prepare";
            msg.content = "game_prepare";
            foreach (int connId in _clientMap.Keys)
            {
                ClientRecord clientRecord = _clientMap[connId];
                clientRecord.conn.Send((int)CustomMsgType.Lobby_Server_Prepare, msg);
            }
        }

        private void OnClientConnected(NetworkMessage netMsg)
        {
            OnClientJoinLobby(netMsg);
        }

        private void OnClientDisconnected(NetworkMessage netMsg)
        {
            if (_clientMap.ContainsKey(netMsg.conn.connectionId))
            {
                OnClientLeave(netMsg.conn.connectionId);
            }
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

        private void OnClientJoinLobby(NetworkMessage netMsg)
        {
            Debug.Log("OnClientConnected.[client]" + netMsg.conn.address);
            if (_context.gameState == GameState.Lobby)
            {
                ClientRecord clientRecord = new ClientRecord();
                clientRecord.bReady = false;
                clientRecord.conn = netMsg.conn;
                _clientMap.Add(netMsg.conn.connectionId, clientRecord);
                Debug.Log("[OnClientJoinLobby] conn id:" + netMsg.conn.connectionId);
            }
            else
            {
                netMsg.conn.Disconnect();
            }
        }

        private void OnClientLeave(int connectionId)
        {
            _clientMap.Remove(connectionId);
        }

        private void OnPlayerReady(NetworkMessage netMsg)
        {
            int connId = netMsg.conn.connectionId;
            ClientRecord clientRecord = _clientMap[connId];
            clientRecord.bReady = true;

            // try to handle client ready 
            _readyCount++;
            if (_readyCount == _clientMap.Count)
            {
                HandleAllReady();
            }
        }

        private void HandleAllReady()
        {
            LobbyMessage msg = new LobbyMessage();
            msg.messageId = ++_lobbyMsgCounter;
            msg.msgType = "game_started";
            msg.content = "game_started";
            foreach (int connId in _clientMap.Keys)
            {
                ClientRecord clientRecord = _clientMap[connId];
                clientRecord.conn.Send((int)CustomMsgType.Game_Start, msg);
            }
        }
    }
}
