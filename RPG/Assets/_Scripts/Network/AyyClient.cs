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

        NetworkConnection _conn = null;

        public int turnIndex { get; set; } = 0;
        public float turnStartTime = 0;
        public float timeCounter = 0;

        Dictionary<int, bool> handledTurnMap = new Dictionary<int, bool>();


        Dictionary<KeyCode, bool> careKeyMap = new Dictionary<KeyCode, bool>();
        Dictionary<KeyCode, bool> keyPressState = new Dictionary<KeyCode, bool>();

        public delegate void DelegateConnectOK();
        DelegateConnectOK connectOKCallback = null;
        
        public AyyClient(AyyNetwork context)
        {
            _context = context;
        }

        public void Start(string serverIP,int serverPort, DelegateConnectOK connOKCallback = null)
        {
            connectOKCallback = connOKCallback;

            _client = new NetworkClient();
            _client.RegisterHandler(MsgType.Connect, OnConnectedServer);
            _client.RegisterHandler(MsgType.Disconnect, OnDisConnectedServer);
            _client.RegisterHandler(MsgType.Error, OnError);

            // lobby
            _client.RegisterHandler((int)CustomMsgType.Lobby_Server_Prepare, OnLobbyMsg);
            _client.RegisterHandler((int)CustomMsgType.Lobby_Server_Player_List, OnLobbyMsg);
            _client.RegisterHandler((int)CustomMsgType.Lobby_Server_Player_Join, OnLobbyMsg);
            _client.RegisterHandler((int)CustomMsgType.Lobby_Server_Player_Left, OnLobbyMsg);
            
            // gameplay
            _client.RegisterHandler((int)CustomMsgType.Game_LockStep_Turn, OnGameplayMsg);
            // do start 
            _client.Connect(serverIP, serverPort);

            // Key State
            careKeyMap.Add(KeyCode.UpArrow, true);
            careKeyMap.Add(KeyCode.DownArrow, true);
            careKeyMap.Add(KeyCode.LeftArrow, true);
            careKeyMap.Add(KeyCode.RightArrow, true);
            foreach (KeyCode key in careKeyMap.Keys)
            {
                keyPressState[key] = false;
            }
        }

        public void Close()
        {
            _client.Disconnect();
            _client = null;
        }

        public void Update(float deltaTime)
        {
            timeCounter += deltaTime;
            UpdateCollectCtrl();
            if (timeCounter - turnStartTime >= AyyNetwork.TURNS_PER_SECOND)
            {
                OnLockStepTurn();
            }
        }


        private void UpdateCollectCtrl()
        {
            foreach (KeyCode keyCode in careKeyMap.Keys)
            {
                if (Input.GetKeyDown(keyCode))
                {
                    keyPressState[keyCode] = true;
                }
                else if (Input.GetKeyUp(keyCode))
                {
                    keyPressState[keyCode] = false;
                }
            }
        }


        private void UpdateForSendCtrl()
        {
            //Debug.Log("UpdateForSendCtrl **");
            foreach (KeyCode key in careKeyMap.Keys)
            {
                if (keyPressState[key])
                {
                    MoveDir dir = MoveDir.Up;
                    switch (key)
                    {
                        case KeyCode.UpArrow:
                            dir = MoveDir.Up;
                            break;
                        case KeyCode.DownArrow:
                            dir = MoveDir.Down;
                            break;
                        case KeyCode.LeftArrow:
                            dir = MoveDir.Left;
                            break;
                        case KeyCode.RightArrow:
                            dir = MoveDir.Right;
                            break;
                    }
                    ClientCtrlMove(dir);
                }
            }
        }

        public void OnLockStepTurn()
        {
            UpdateForSendCtrl();

            //Debug.Log("OnLockStepTurn **");
            if (!HasHandledTurn(turnIndex))
            {
                ClientDoNothing();
            }
        }

        private void OnConnectedServer(NetworkMessage netMsg)
        {
            Debug.Log("OnConnctedServer.[server]" + netMsg.conn.address);
            _conn = netMsg.conn;
            connectOKCallback?.Invoke();
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
            {
                Debug.LogWarning("has handled turn:" + turnIndex);
                return;
            }
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
            if (_conn == null)
            {
                return;
            }
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


            Debug.Log("OnGameplayMsg --- turnIndex:" + turnIndex);

            _context.HandleMessage(msg);
        }

        private void MarkTurnHandled(int theTurnIndex)
        {
            Debug.Log("mark turn handled:" + theTurnIndex);
            handledTurnMap.Add(theTurnIndex, true);

            if (handledTurnMap.ContainsKey(theTurnIndex - 2))
            {
                handledTurnMap.Remove(theTurnIndex - 2);
            }
        }

        private bool HasHandledTurn(int theTurnIndex)
        {
            return handledTurnMap.ContainsKey(theTurnIndex);
        }
    }
}

