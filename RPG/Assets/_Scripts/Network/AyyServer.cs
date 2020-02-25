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
        public float startTime = 0;
        public float period = 0;
        public float shouldKeepPeriod = 0;
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
        public bool CheckPeriod()
        {
            return period >= shouldKeepPeriod;
        }
    }

    public class AyyServer
    {
        AyyNetwork _context = null;
        Dictionary<int, ClientRecord> _clientMap = new Dictionary<int, ClientRecord>();

        int _readyCount = 0;
        int _lobbyMsgCounter = 0;

        int _lockstepTurnIndexCounter = 0;

        float MAX_TURN_PERIOD = 0;
        LockStepTurn _currentTurn = null;

        
        // 用于记录时间 
        float _timeCounter = 0; 

        

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
            NetworkServer.RegisterHandler((short)CustomMsgType.Game_Client_Ctrl, OnPlayerCtrl);
            

            bool success = NetworkServer.Listen(20086);
            if (success)
            {
                Debug.Log("Server start success.");
            }
            else
            {
                Debug.Log("Server start failed.");
            }
            
            MAX_TURN_PERIOD = Time.fixedDeltaTime;

            _readyCount = 0;
            _lobbyMsgCounter = 0;
            _timeCounter = 0;
            return success;
        }

        public void Close()
        {

        }

        public void FixedUpdate(float deltaTime)
        {
            _timeCounter += deltaTime;
            if (_currentTurn == null)
            {
                return;
            }
            _currentTurn.TimeElapse(deltaTime);

            if (_currentTurn.CheckPeriod())
            {
                if (_currentTurn.CheckCollection(_clientMap.Count))
                {
                    BroadCastTurn();
                    NextTurn();
                }
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

        private void OnPlayerCtrl(NetworkMessage netMsg)
        {
            int connId = netMsg.conn.connectionId;
            if (_clientMap.ContainsKey(connId))
            { 
                
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
            turn.startTime = _timeCounter;
            turn.shouldKeepPeriod = MAX_TURN_PERIOD;
            turn.period = 0;

            _currentTurn = turn;
        }

        private void FillSpawnMessageInitialTurn()
        {
            int index = 0;
            foreach (int connId in _clientMap.Keys)
            {
                ClientRecord clientRecord = _clientMap[connId];
                    
                GameMessage gameMsg = new GameMessage();
                gameMsg.lockstepTurn = _currentTurn.turnIndex;
                gameMsg.msgType = "game_spawn";

                JsonWriter writer = new JsonWriter();
                writer.WriteObjectStart();
                writer.WritePropertyName("spawn_point");
                writer.Write(index);
                writer.WriteObjectEnd();

                gameMsg.content = writer.ToString();
                //Debug.Log("send msg content:" + gameMsg.content);

                _currentTurn.AddMessage(connId,gameMsg);

                index++;
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
            Debug.Log("send msg content:" + sendMsg.content);

            // Send message to each client
            foreach (int connId in _clientMap.Keys)
            {
                ClientRecord clientRecord = _clientMap[connId];
                clientRecord.conn.Send((int)CustomMsgType.Game_LockStep_Turn, sendMsg);
            }
        }
    }
}
