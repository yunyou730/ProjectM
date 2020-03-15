using System;
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

        public int sessionId { set; get; } = -1;

        public int turnIndex { get; set; } = 0;
        public float turnStartTime = 0;
        public float timeCounter = 0;

        Dictionary<int, bool> handledTurnMap = new Dictionary<int, bool>();
        
        public delegate void DelegateConnectOK();
        DelegateConnectOK connectOKCallback = null;
        
        
        public PlayerInput   input = new PlayerInput(PlayerInput.Usage.Communication);
        private List<int>    ctrlCodeList = new List<int>();
        
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
        }

        public void Close()
        {
            _client.Disconnect();
            _client = null;
        }

        public void Update(float deltaTime)
        {
            timeCounter += deltaTime;
            if (_conn != null)
            {
                input.CollectSample();
                ctrlCodeList.Add(input.Marshal());
                if (timeCounter - turnStartTime >= AyyNetwork.TURNS_PER_SECOND)
                {
                    OnLockStepTurn();
                }
            }
        }
        
        public void OnLockStepTurn()
        {
            if(!HasHandledTurn(turnIndex))
            {
                if (ctrlCodeList.Count > 0)
                {
                    // 把 所有 没有发出去的操作，合并成一个 keyMask 在一个 lockstep turn 里集中发出去
                    int keyMask = 0;
                    for (int i = 0;i < ctrlCodeList.Count;i++)
                    {
                        keyMask = keyMask | ctrlCodeList[i];
                    }
                    ClientCtrl(keyMask);
                    ctrlCodeList.Clear();
                }
                else
                {
                    ClientDoNothing();
                }
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
        
        public bool ClientCtrl(int keyMask)
        {
            if (HasHandledTurn(turnIndex))
            {
                //Debug.LogWarning("has handled turn:" + turnIndex);
                return false;
            }
            MarkTurnHandled(turnIndex);
            
            GameMessage msg = new GameMessage();
            msg.msgType = "client_ctrl_keymask";
            msg.content = keyMask.ToString();
            
            //Debug.Log("[send key mask] " + Convert.ToString(keyMask,2));
            
            _conn.Send((int)CustomMsgType.Game_Client_Ctrl,msg);
            return true;
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


            //Debug.Log("OnGameplayMsg --- turnIndex:" + turnIndex);

            _context.HandleMessage(msg);
        }

        private void MarkTurnHandled(int theTurnIndex)
        {
            //Debug.Log("mark turn handled:" + theTurnIndex);
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

