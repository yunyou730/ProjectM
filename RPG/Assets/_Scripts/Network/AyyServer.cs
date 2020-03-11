using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;

namespace ayy
{
    public class ClientRecord
    {
        public bool bReady = false;
        public NetworkConnection conn = null;
    }

    public class LockStepTurn
    {
        public int turnIndex = 0;
        public float period = 0;            // 记录 turn 总共经历的时间 
        public Dictionary<int, GameMessage> messageMap = new Dictionary<int, GameMessage>();


        public bool CheckCollection(int mapSize)
        {
            return messageMap.Count >= mapSize ;
        }

        public void OnMessage(NetworkMessage msg)
        {
            NetworkConnection conn = msg.conn;
            int connId = conn.connectionId;
            if (!messageMap.ContainsKey(connId))
            {
                GameMessage gameMsg = new GameMessage();
                gameMsg.Deserialize(msg.reader);
                messageMap.Add(connId,gameMsg);
            }
        }

        public void AddMessage(int connId,GameMessage msg)
        {
            if (!messageMap.ContainsKey(connId))
            {
                messageMap.Add(connId, msg);
            }
        }

        public void TimeElapse(float deltaTime)
        {
            period += deltaTime;
        }
    }

    public class AyyServer
    {
        AyyNetwork _context = null;
        Dictionary<int, ClientRecord> _clientMap = new Dictionary<int, ClientRecord>();

        int _readyCount = 0;
        int _lobbyMsgCounter = 0;

        int _lockstepTurnIndexCounter = 0;
        LockStepTurn _currentTurn = null;

        bool bGameStarted = false;
        float elapsedTime = 0;

        public AyyServer(AyyNetwork context)
        {
            _context = context;
        }

        public bool Start(int port)
        {
            NetworkServer.RegisterHandler(MsgType.Connect, OnClientConnected);
            NetworkServer.RegisterHandler(MsgType.Disconnect, OnClientDisconnected);
            NetworkServer.RegisterHandler(MsgType.Error, OnError);

            NetworkServer.RegisterHandler((short)CustomMsgType.Lobby_Player_Ready,OnPlayerReady);
            NetworkServer.RegisterHandler((short)CustomMsgType.Game_Client_Ctrl, OnPlayerCtrl);
            

            bool success = NetworkServer.Listen(port);
            if (success)
            {
                Debug.Log("Server start success.");
            }
            else
            {
                Debug.Log("Server start failed.");
            }

            _readyCount = 0;
            _lobbyMsgCounter = 0;
            return success;
        }

        public void Close()
        {
            NetworkServer.UnregisterHandler(MsgType.Connect);
            NetworkServer.UnregisterHandler(MsgType.Disconnect);
            NetworkServer.UnregisterHandler(MsgType.Error);
            NetworkServer.UnregisterHandler((short)CustomMsgType.Lobby_Player_Ready);
            NetworkServer.UnregisterHandler((short)CustomMsgType.Game_Client_Ctrl);
            NetworkServer.DisconnectAll();
            NetworkServer.Reset();
        }

        public void Update(float deltaTime)
        {
            if (!bGameStarted)
            {
                return;
            }
            elapsedTime += deltaTime;
            while (elapsedTime >= AyyNetwork.TURNS_PER_SECOND)
            {
                float overTime = elapsedTime - AyyNetwork.TURNS_PER_SECOND;
                elapsedTime = overTime;
                OnLockStepTurn();
            }
        }
        
        private void OnLockStepTurn()
        {
            if (_currentTurn != null && _currentTurn.CheckCollection(_clientMap.Count))
            {
                BroadCastTurn();
                NextTurn();
            }
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
            bGameStarted = true;
            elapsedTime = 0;
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
                
                BroadCastPlayerJoin(netMsg.conn.connectionId);
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
        
        // Lobby 阶段 ,广播 player 加入
        private void BroadCastPlayerJoin(int joinPlayerConnId)
        {
            LobbyMessage msg = new LobbyMessage();
            msg.messageId = ++_lobbyMsgCounter;
            msg.msgType = "player_join";
            
            JsonWriter writer = new JsonWriter();
            writer.WriteObjectStart();
            writer.WritePropertyName("player_id");
            writer.Write(joinPlayerConnId);
            writer.WriteObjectEnd();
            
            msg.content = writer.ToString();
            
            foreach (int connId in _clientMap.Keys)
            {
                ClientRecord clientRecord = _clientMap[connId];
                clientRecord.conn.Send((int)CustomMsgType.Lobby_Server_Player_Join, msg);
            }
        }

        // Lobby 阶段,广播 player 离开
        private void BroadCastPlayerLeft()
        {
            
        }
        
        // Lobby 阶段,单独告诉某个 Player, 当前所有 Player 的状态  
        private void TellPlayerState(int toConnId)
        {
            LobbyMessage msg = new LobbyMessage();
            msg.messageId = ++_lobbyMsgCounter;
            msg.msgType = "player_join";
            
            JsonWriter writer = new JsonWriter();
            writer.WriteArrayStart();
            foreach (int connId in _clientMap.Keys)
            {
                writer.WriteObjectStart();
                writer.WritePropertyName("player_id");
                writer.Write(connId);
                writer.WriteObjectEnd();
            }
            writer.WriteArrayEnd();
            
            msg.content = writer.ToString();
            
            
            ClientRecord clientRecord = _clientMap[toConnId];
            clientRecord.conn.Send((int)CustomMsgType.Lobby_Server_Player_List, msg);
            
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

        private void OnPlayerCtrl(NetworkMessage netMsg)
        {
            int connId = netMsg.conn.connectionId;
            if (_clientMap.ContainsKey(connId))
            {
                if (_currentTurn != null)
                {
                    _currentTurn.OnMessage(netMsg);
                }
            }
        }

        private void HandleAllReady()
        {
            _lockstepTurnIndexCounter = 0;
            NextTurn();
            FillSpawnMessageInitialTurn();
        }


        private void NextTurn()
        {
            _lockstepTurnIndexCounter++;
            
            LockStepTurn turn = new LockStepTurn();
            turn.turnIndex = _lockstepTurnIndexCounter;
            turn.period = 0;

            _currentTurn = turn;
        }

        private void FillSpawnMessageInitialTurn()
        {
            int spawnPointIndex = 0;
            foreach (int connId in _clientMap.Keys)
            {
                ClientRecord clientRecord = _clientMap[connId];
                    
                GameMessage gameMsg = new GameMessage();
                gameMsg.lockstepTurn = _currentTurn.turnIndex;
                gameMsg.msgType = "game_spawn";

                JsonWriter writer = new JsonWriter();
                writer.WriteObjectStart();
                writer.WritePropertyName("spawn_point");
                writer.Write(spawnPointIndex);
                writer.WriteObjectEnd();

                gameMsg.content = writer.ToString();
                //Debug.Log("send msg content:" + gameMsg.content);

                _currentTurn.AddMessage(connId,gameMsg);

                spawnPointIndex++;
            }
        }
        
        private void BroadCastTurn()
        {
            // Build message content
            GameMessage sendMsg = new GameMessage();
            sendMsg.lockstepTurn = _currentTurn.turnIndex;
            sendMsg.msgType = "game_turn";

            JsonWriter writer = new JsonWriter();
            writer.WriteObjectStart();
            foreach (int connId in _currentTurn.messageMap.Keys)
            {
                GameMessage gameMsg = _currentTurn.messageMap[connId];

                writer.WritePropertyName(connId.ToString());
                writer.WriteObjectStart();
                /*
                writer.WritePropertyName("conn_id");
                writer.Write(connId);
                */
                writer.WritePropertyName("msg_type");
                writer.Write(gameMsg.msgType);
                
                writer.WritePropertyName("msg_content");
                writer.Write(gameMsg.content);

                writer.WriteObjectEnd();
            }
            writer.WriteObjectEnd();
            sendMsg.content = writer.ToString();
            //Debug.Log("send msg content:" + sendMsg.content);

            // Send message to each client
            foreach (int connId in _clientMap.Keys)
            {
                ClientRecord clientRecord = _clientMap[connId];
                clientRecord.conn.Send((int)CustomMsgType.Game_LockStep_Turn, sendMsg);
            }
        }
    }
}
