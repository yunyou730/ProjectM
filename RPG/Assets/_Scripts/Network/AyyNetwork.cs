using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public static float TURNS_PER_SECOND = 1.0f/30.0f;
        //public static float TURNS_PER_SECOND = 0.1f;

        public AyyServer _server = null;
        public AyyClient _client = null;
        public bool IsServer { set; get; } = false;

        public GameState gameState { set; get; } = GameState.Lobby;

        public bool IsWorking { set; get; } = false;

        public string serverIP { set; get; } = "127.0.0.1";
        public int serverPort { set; get; } = 20086;


        float elapsedTime = 0;


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


            float dt = Time.deltaTime;
            elapsedTime += dt;

            while (elapsedTime >= TURNS_PER_SECOND)
            {
                float overTime = elapsedTime - TURNS_PER_SECOND;
                elapsedTime = overTime;
                OnLockStepTurn();
            }

        }
        
        private void OnLockStepTurn()
        {
            //Debug.Log("OnLockStepTurn ----- ");
            if (_client != null)
            {
                //UpdateForSendCtrl();
                _client.OnLockStepTurn();
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

        public void StartAsClient(string ip,int port)
        {
            if (IsWorking) return;

            _client = new AyyClient(this);
            _client.Start(ip, port);
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

        // ---------- Gameplay Code -------------- 
        public delegate void GamePrepare();
        public event GamePrepare GamePrepareEvent;

        public delegate void GameStart();
        public event GameStart GameStartEvent;

        public delegate void GameTurn(int turnIndex,string json);
        public event GameTurn GameTurnEvent;

        public void HandleMessage(LobbyMessage msg)
        {
            Debug.Log("[HandleMessage(LobbyMessage)] " + msg.msgType);
            switch (msg.msgType)
            {
                case "game_prepare":
                    GamePrepareEvent?.Invoke();
                    break;
            }
        }

        public void HandleMessage(GameMessage msg)
        {
            //Debug.Log("[HandleMessage(GameMessage)] " + msg.msgType);
            switch (msg.msgType)
            {
                case "start_game":
                    GameStartEvent?.Invoke();
                    break;
                case "game_turn":
                    _client.turnIndex = msg.lockstepTurn;
                    GameTurnEvent?.Invoke(msg.lockstepTurn,msg.content);
                    break;
            }
        }
    }
}

