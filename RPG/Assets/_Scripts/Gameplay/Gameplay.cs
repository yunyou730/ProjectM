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
        
        private void Awake()
        {
            map = GameObject.Find("Map").GetComponent<MapMonoBehaviour>();
            network = GameObject.Find("NetworkManager").GetComponent<AyyNetwork>();

            network.GamePrepareEvent += OnStartLoadGame;
            network.GameTurnEvent += OnGameTurnMessage;
        }

        void Start()
        {

        }
        
        void Update()
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
            }
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
    }
}
