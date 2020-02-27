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

        public int turnIndex { get; set; } = 0;
        public float turnStartTime = 0;
        public bool bHandleThisTurn = false;
        public float timeCounter = 0;


        float MAX_TURN_PERIOD = 0;

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

            MAX_TURN_PERIOD = Time.fixedDeltaTime;
        }

        public void Close()
        {

        }

        public void FixedUpdate(float deltaTime)
        {
            if (_conn != null)
            {
                timeCounter += deltaTime;
                if (timeCounter - turnStartTime >= MAX_TURN_PERIOD && !bHandleThisTurn)
                {
                    ClientDoNothing();
                }
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

        public void ClientCtrlMove(MoveDir moveDir)
        {
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

            bHandleThisTurn = true;
        }

        public void ClientKeyPress(KeyCode keyCode)
        {
            GameMessage msg = new GameMessage();
            msg.msgType = "client_key_press";
            msg.content = ((int)keyCode).ToString();
            _conn.Send((int)CustomMsgType.Game_Client_Ctrl, msg);

            bHandleThisTurn = true;
        }

        public void ClientKeyRelease(KeyCode keyCode)
        {
            GameMessage msg = new GameMessage();
            msg.msgType = "client_key_release";
            msg.content = ((int)keyCode).ToString();
            _conn.Send((int)CustomMsgType.Game_Client_Ctrl, msg);

            bHandleThisTurn = true;
        }


        public void ClientDoNothing()
        {
            GameMessage msg = new GameMessage();
            msg.msgType = "game_client_empty";
            msg.content = "game_client_empty";
            _conn.Send((int)CustomMsgType.Game_Client_Ctrl, msg);

            bHandleThisTurn = true;
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
            bHandleThisTurn = false;

            _context.HandleMessage(msg);
        }
    }
}

