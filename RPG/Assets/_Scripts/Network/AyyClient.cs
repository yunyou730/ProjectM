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
        //private int _sendCounter = 0;

        NetworkConnection _conn = null;

        public int turnIndex { get; set; } = 0;
        public float turnStartTime = 0;
        //public bool bHandleThisTurn = false;
        public float timeCounter = 0;

        Dictionary<int, bool> handledTurnMap = new Dictionary<int, bool>();


        //float MAX_TURN_PERIOD = 0;

        public AyyClient(AyyNetwork context)
        {
            _context = context;
        }

        public void Start(string serverIP,int serverPort)
        {
            _client = new NetworkClient();
            _client.RegisterHandler(MsgType.Connect, OnConnectedServer);
            _client.RegisterHandler(MsgType.Disconnect, OnDisConnectedServer);
            _client.RegisterHandler(MsgType.Error, OnError);

            // lobby
            _client.RegisterHandler((int)CustomMsgType.Lobby_Server_Prepare, OnLobbyMsg);
            // gameplay
            //_client.RegisterHandler((int)CustomMsgType.Game_Start, OnGameplayMsg);
            _client.RegisterHandler((int)CustomMsgType.Game_LockStep_Turn, OnGameplayMsg);
            // do start 
            _client.Connect(serverIP, serverPort);

            //MAX_TURN_PERIOD = Time.fixedDeltaTime;
        }

        public void Close()
        {

        }

        public void FixedUpdate(float deltaTime)
        {
            //if (_conn != null)
            //{
            //    timeCounter += deltaTime;
            //    if (timeCounter - turnStartTime >= MAX_TURN_PERIOD && !bHandleThisTurn)
            //    {
            //        ClientDoNothing();
            //    }
            //}
        }

        public void OnLockStepTurn()
        {
            if (!HasHandledTurn(turnIndex))
            {
                ClientDoNothing();
            }
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

        public void ClientReady()
        {
            LobbyMessage msg = new LobbyMessage();
            msg.msgType = "client_ready";
            msg.content = "client_ready";
            _conn.Send((int)CustomMsgType.Lobby_Player_Ready,msg);
        }

        public void ClientCtrlMove(MoveDir moveDir)
        {
            if (HasHandledTurn(turnIndex)) 
                return;
            MarkTurnHandled(turnIndex);

            GameMessage msg = new GameMessage();
            msg.msgType = "client_ctrl_move";
            string content = "";
            switch (moveDir)
            {
                case MoveDir.Up:
                    content = "up";
                    break;
                case MoveDir.Down:
                    content = "down";
                    break;
                case MoveDir.Left:
                    content = "left";
                    break;
                case MoveDir.Right:
                    content = "right";
                    break;
            }
            msg.content = content;
            _conn.Send((int)CustomMsgType.Game_Client_Ctrl,msg);

        }

        public void ClientKeyPress(KeyCode keyCode)
        {
            if (HasHandledTurn(turnIndex)) 
                return;
            MarkTurnHandled(turnIndex);

            GameMessage msg = new GameMessage();
            msg.msgType = "client_key_press";
            msg.content = ((int)keyCode).ToString();
            _conn.Send((int)CustomMsgType.Game_Client_Ctrl, msg);
        }

        public void ClientKeyRelease(KeyCode keyCode)
        {
            if (HasHandledTurn(turnIndex)) 
                return;
            MarkTurnHandled(turnIndex);

            GameMessage msg = new GameMessage();
            msg.msgType = "client_key_release";
            msg.content = ((int)keyCode).ToString();
            _conn.Send((int)CustomMsgType.Game_Client_Ctrl, msg);
        }


        public void ClientDoNothing()
        {
            if (HasHandledTurn(turnIndex))
                return;
            MarkTurnHandled(turnIndex);

            GameMessage msg = new GameMessage();
            msg.msgType = "game_client_empty";
            msg.content = "game_client_empty";
            _conn.Send((int)CustomMsgType.Game_Client_Ctrl, msg);
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


            turnIndex = msg.lockstepTurn;
            turnStartTime = timeCounter;
            //bHandleThisTurn = false;

            _context.HandleMessage(msg);
        }

        private void MarkTurnHandled(int theTurnIndex)
        {
            handledTurnMap.Add(theTurnIndex, true);
        }

        private bool HasHandledTurn(int theTurnIndex)
        {
            return handledTurnMap.ContainsKey(theTurnIndex);
        }
    }
}

