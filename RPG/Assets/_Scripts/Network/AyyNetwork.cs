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

    public class AyyNetwork : MonoBehaviour
    {
        public AyyServer _server = null;
        public AyyClient _client = null;
        public bool IsServer { set; get; } = false;

        public GameState gameState { set; get; } = GameState.Lobby;

        public bool IsWorking { set; get; } = false;
        
        void Start()
        {
            Time.fixedDeltaTime = 1 / 60.0f;
        }

        void Update()
        {
            
        }

        public void StartAsServer()
        {
            if (IsWorking) return;

            _server = new AyyServer(this);
            if (_server.Start())
            {
                IsServer = true;
                IsWorking = true;

                _client = new AyyClient(this);
                _client.Start();
            }
        }

        public void StartAsClient()
        {
            if (IsWorking) return;

            _client = new AyyClient(this);
            _client.Start();
            IsServer = false;
            IsWorking = true;
        }

        public void ServerStartGame()
        {
            if (!IsServer) return;
            _server.StartGame();
        }

        public void ClientReady()
        {
            _client.ClientReady();
        }


        // ---------- Send to Server --------------
        public void LoadGameDone()
        {
            
        }

        // ---------- Gameplay Code -------------- 
        public delegate void StartGame();
        public event StartGame StartGameEvent;

        public void HandleMessage(LobbyMessage msg)
        {
            switch (msg.msgType)
            {
                case "game_prepare":
                    StartGameEvent?.Invoke();
                    Debug.Log("[event]start_game");
                    break;
            }
        }

        public void HandleMessage(GameMessage msg)
        {
            //switch (msg.msgType)
            //{
            //    case "start_game":
            //        StartGameEvent?.Invoke();
            //        Debug.Log("[event]start_game");
            //        break;
            //}
        }
    }
}

