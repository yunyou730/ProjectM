﻿using System.Collections;
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

        private void FixedUpdate()
        {
            if (_server != null)
            {
                _server.FixedUpdate(Time.fixedDeltaTime);
            }
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

        // ---------- Send to Server --------------
        public void ClientReady()
        {
            _client.ClientReady();
        }



        // ---------- Gameplay Code -------------- 
        public delegate void GamePrepare();
        public event GamePrepare GamePrepareEvent;

        public delegate void GameStart();
        public event GameStart GameStartEvent;

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
            Debug.Log("[HandleMessage(GameMessage)] " + msg.msgType);
            switch (msg.msgType)
            {
                case "start_game":
                    GameStartEvent?.Invoke();
                    break;
            }
        }
    }
}

