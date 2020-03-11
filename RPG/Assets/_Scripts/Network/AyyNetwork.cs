using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace ayy
{
    public enum GameState
    {
        Lobby,
        Playing,
    }

    public enum MoveDir
    {
        Up,
        Down,
        Left,
        Right,
    }



    public class AyyNetwork : MonoBehaviour
    {
        public static float TURNS_PER_SECOND = 1.0f / 30.0f;
        //public static float TURNS_PER_SECOND = 1.0f/30.0f;
        //public static float TURNS_PER_SECOND = 0.1f;

        public AyyServer _server = null;
        public AyyClient _client = null;
        public bool IsServer { set; get; } = false;

        public GameState gameState { set; get; } = GameState.Lobby;

        public bool IsWorking { set; get; } = false;

        public string serverIP { set; get; } = "127.0.0.1";
        public int serverPort { set; get; } = 20086;


        // ------ lobby event ------
        public delegate void LobbyPlayerListDelegate(List<int> playerIdArray);
        public event LobbyPlayerListDelegate PlayerListEvent;

        public delegate void LobbyPlayerJoinDelegate(int playerId);
        public event LobbyPlayerJoinDelegate PlayerJoinEvent;

        public delegate void LobbyPlayerLeftDelegate(int playerId);
        public event LobbyPlayerLeftDelegate PlayerLeftEvent;

        // ------ gameplay event ------
        public delegate void GamePrepare();
        public event GamePrepare GamePrepareEvent;

        public delegate void GameStart();
        public event GameStart GameStartEvent;

        public delegate void GameTurn(int turnIndex, string json);
        public event GameTurn GameTurnEvent;

        void Start()
        {

        }

        void Update()
        {
            if (_server == null && _client == null)
            {
                return;
            }
            if (_client != null)
            {
                _client.Update(Time.deltaTime);
            }
            if (_server != null)
            {
                _server.Update(Time.deltaTime);
            }
        }
        
        private void OnDestroy()
        {
            if (_client != null)
            {
                _client.Close();
            }
            if (_server != null)
            {
                _server.Close();
            }
        }

        public void StartAsServer()
        {
            if (IsWorking) return;

            _server = new AyyServer(this);
            if (_server.Start(serverPort))
            {
                IsServer = true;
                IsWorking = true;

                _client = new AyyClient(this);
                _client.Start("127.0.0.1",serverPort);
            }
        }

        public void StartAsClient(string ip,int port,AyyClient.DelegateConnectOK okCallback = null)
        {
            if (IsWorking) return;

            _client = new AyyClient(this);
            _client.Start(ip, port, okCallback);
            IsServer = false;
            IsWorking = true;
        }

        public void ServerStartGame()
        {
            if (!IsServer) return;
            _server.StartGame();
        }

        // ---------- Send to Server --------------
        public void ClientReady()
        {
            _client.ClientReady();
        }

        public void ClientCtrlMove(MoveDir moveDir)
        {
            _client.ClientCtrlMove(moveDir);
        }


        public void ClientKeyPress(KeyCode keyCode)
        {
            _client.ClientKeyPress(keyCode);
        }

        public void ClientKeyRelease(KeyCode keyCode)
        {
            _client.ClientKeyRelease(keyCode);
        }

        // ---------- Lobby Code -------------- 
        public void HandleMessage(LobbyMessage msg)
        {
            Debug.Log("[HandleMessage(LobbyMessage)] " + msg.msgType);
            switch (msg.msgType)
            {
                case "game_prepare":
                    GamePrepareEvent?.Invoke();
                    break;
                case "player_list":
                    {
                        string strJson = msg.content;
                        JsonData jd = JsonMapper.ToObject(strJson);
                        List<int> playerIdArray = new List<int>();
                        for (int i = 0;i < jd.Count;i++)
                        {
                            JsonData playerInfo = jd[i];
                            int playerId = (int)playerInfo["player_id"];
                            playerIdArray.Add(playerId);
                        }
                        PlayerListEvent?.Invoke(playerIdArray);
                    }
                    break;
                case "player_join":
                    {
                        string strJson = msg.content;
                        JsonData jd = JsonMapper.ToObject(strJson);
                        int playerId = (int)jd["player_id"];
                        PlayerJoinEvent?.Invoke(playerId);
                    }
                    break;
                case "player_left":
                    {
                        string strJson = msg.content;
                        JsonData jd = JsonMapper.ToObject(strJson);
                        int playerId = (int)jd["player_id"];
                        PlayerLeftEvent?.Invoke(playerId);
                    }
                    break;
            }
        }

        // ---------- Gameplay Code -------------- 
        public void HandleMessage(GameMessage msg)
        {
            //Debug.Log("[HandleMessage(GameMessage)] " + msg.msgType);
            switch (msg.msgType)
            {
                //case "start_game":
                //    GameStartEvent?.Invoke();
                //    break;
                case "game_turn":
                    _client.turnIndex = msg.lockstepTurn;
                    if (_client.turnIndex == 1)
                    {
                        GameStartEvent?.Invoke();
                    }
                    GameTurnEvent?.Invoke(msg.lockstepTurn,msg.content);
                    break;
            }
        }
    }
}

