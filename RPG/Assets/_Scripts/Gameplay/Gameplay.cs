using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace ayy
{
    public class Gameplay : MonoBehaviour
    {
        MapMonoBehaviour    map = null;
        AyyNetwork          network = null;

        public GameObject playerPrefab = null;
        public Vector3[] spawnPoints = null;

        Dictionary<int, GameObject> playerMap = new Dictionary<int, GameObject>();


        bool bHasSendCtrlThisTurn = false;


        Dictionary<int, Dictionary<KeyCode, bool>> clientKeyStateMap = new Dictionary<int, Dictionary<KeyCode, bool>>();
        Dictionary<KeyCode, bool> careKeyMap = new Dictionary<KeyCode, bool>();

        private void Awake()
        {
            map = GameObject.Find("Map").GetComponent<MapMonoBehaviour>();
            network = GameObject.Find("NetworkManager").GetComponent<AyyNetwork>();

            careKeyMap.Add(KeyCode.W,true);
            careKeyMap.Add(KeyCode.S, true);
            careKeyMap.Add(KeyCode.A, true);
            careKeyMap.Add(KeyCode.D, true);

            network.GamePrepareEvent += OnStartLoadGame;
            network.GameTurnEvent += OnGameTurnMessage;
        }

        void Start()
        {

        }
        
        void Update()
        {
            UpdateForSendCtrl();
            UpdateForPlayerCtrl();
        }


        void UpdateForSendCtrl()
        {
            if (!bHasSendCtrlThisTurn)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    network.ClientCtrlMove(MoveDir.Up);
                    bHasSendCtrlThisTurn = true;
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    network.ClientCtrlMove(MoveDir.Down);
                    bHasSendCtrlThisTurn = true;
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    network.ClientCtrlMove(MoveDir.Left);
                    bHasSendCtrlThisTurn = true;
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    network.ClientCtrlMove(MoveDir.Right);
                    bHasSendCtrlThisTurn = true;
                }

                foreach (KeyCode keyCode in careKeyMap.Keys)
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        network.ClientKeyPress(keyCode);
                        bHasSendCtrlThisTurn = true;
                    }
                    else if (Input.GetKeyUp(keyCode))
                    {
                        network.ClientKeyRelease(keyCode);
                        bHasSendCtrlThisTurn = true;
                    }
                }

            }
        }


        void UpdateForPlayerCtrl()
        {
            foreach (int connId in playerMap.Keys)
            {
                UpdateOnePlayerCtrl(connId);
            }
        }

        void UpdateOnePlayerCtrl(int connId)
        {
            GameObject playerGo = playerMap[connId];

            float moveSpeed = 10;
            Vector3 offset = new Vector3();
            if (IsPlayerPressingKey(connId,KeyCode.W))
            {
                offset.z += Time.deltaTime * moveSpeed;
            }
            if (IsPlayerPressingKey(connId, KeyCode.S))
            {
                offset.z -= Time.deltaTime * moveSpeed;
            }
            if (IsPlayerPressingKey(connId, KeyCode.A))
            {
                offset.x -= Time.deltaTime * moveSpeed;
            }
            if (IsPlayerPressingKey(connId, KeyCode.D))
            {
                offset.x += Time.deltaTime * moveSpeed;
            }
            playerGo.transform.Translate(offset);
        }
    

        void OnStartLoadGame()
        {
            map.CreateMap();
            network.ClientReady();
        }

        void OnGameTurnMessage(int turnIndex,string turnJson)
        {
            bHasSendCtrlThisTurn = false;
            JsonData jd = JsonMapper.ToObject(turnJson);
            foreach (string strClientId in jd.Keys)
            {
                int clientId = int.Parse(strClientId);
                string msgType = (string)jd[strClientId]["msg_type"];
                string msgContent = (string)jd[strClientId]["msg_content"];
                HandleGameplayMessage(clientId,msgType,msgContent);
            }
        }

        private void HandleGameplayMessage(int clientId,string msgType,string msgContent)
        {
            JsonData jd = null;
            switch (msgType)
            {
                case "game_spawn":
                    jd = JsonMapper.ToObject(msgContent);
                    int spawnPointIndex = (int)jd["spawn_point"];
                    OnPlayerSpawn(clientId,spawnPointIndex);
                    break;
                case "client_ctrl_move":
                    OnPlayerCtrlMove(clientId, msgContent);
                    break;
                case "game_client_empty":
                    Debug.Log("client:" + clientId + " do nothing");
                    break;
                case "client_key_press":
                    OnPlayerKeyPress(clientId, msgContent);
                    break;
                case "client_key_release":
                    OnPlayerKeyRelease(clientId, msgContent);
                    break;
            }
        }
        
        private void OnPlayerSpawn(int clientId,int spawnPosIndex)
        {
            Vector3 pos = spawnPoints[spawnPosIndex];
            GameObject playerObject = GameObject.Instantiate(playerPrefab, pos, Quaternion.identity);
            playerMap.Add(clientId,playerObject); 
        }

        private void OnPlayerCtrlMove(int clientId,string strDir)
        {
            if (!playerMap.ContainsKey(clientId)) return;
            GameObject go = playerMap[clientId];
            switch (strDir)
            {
                case "up":
                    go.transform.Translate(new Vector3(0,0,1));
                    break;
                case "down":
                    go.transform.Translate(new Vector3(0, 0, -1));
                    break;
                case "left":
                    go.transform.Translate(new Vector3(-1, 0, 0));
                    break;
                case "right":
                    go.transform.Translate(new Vector3(1, 0, 0));
                    break;
            }
        }

        private void OnPlayerKeyPress(int clientId,string msg)
        {
            KeyCode keyCode = (KeyCode)int.Parse(msg);
            if (!clientKeyStateMap.ContainsKey(clientId))
            {
                clientKeyStateMap.Add(clientId, new Dictionary<KeyCode, bool>());
            }
            if (clientKeyStateMap[clientId].ContainsKey(keyCode))
            {
                clientKeyStateMap[clientId][keyCode] = true;
            }
            else
            {
                clientKeyStateMap[clientId].Add(keyCode, true);
            }
        }

        private void OnPlayerKeyRelease(int clientId,string msg)
        {
            KeyCode keyCode = (KeyCode)int.Parse(msg);
            if (!clientKeyStateMap.ContainsKey(clientId))
            {
                clientKeyStateMap.Add(clientId, new Dictionary<KeyCode, bool>());
            }
            if (clientKeyStateMap[clientId].ContainsKey(keyCode))
            {
                clientKeyStateMap[clientId][keyCode] = false;
            }
            else
            {
                clientKeyStateMap[clientId].Add(keyCode, false);
            }
        }


        private bool IsPlayerPressingKey(int connId,KeyCode keyCode)
        {
            if (!clientKeyStateMap.ContainsKey(connId))
            {
                return false;
            }
            if (!clientKeyStateMap[connId].ContainsKey(keyCode))
            {
                return false;
            }
            return clientKeyStateMap[connId][keyCode];
        }
    }
}
