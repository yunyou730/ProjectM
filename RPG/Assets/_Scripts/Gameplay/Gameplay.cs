using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace ayy
{
    public class PrepareSendControl
    { 
        
    }

    public class Gameplay : MonoBehaviour
    {
        private MapMonoBehaviour    map = null;
        private AyyNetwork          network = null;
        
        public GameObject playerPrefab = null;
        public Vector3[] spawnPoints = null;

        Dictionary<int, Player> playerMap = new Dictionary<int, Player>();
        

        Dictionary<int, Dictionary<KeyCode, bool>> clientKeyStateMap = new Dictionary<int, Dictionary<KeyCode, bool>>();
        //Dictionary<KeyCode, bool> careKeyMap = new Dictionary<KeyCode, bool>();

        private void Awake()
        {
            /*
            if (map == null)
            {
                map = GameObject.Find("Map").GetComponent<MapMonoBehaviour>();
            }
            
            if (network == null)
            {
                network = GameObject.Find("NetworkManager").GetComponent<AyyNetwork>();
            }
            
            if (network == null)
            {
                network = GameObject.Find("gameplay_network").GetComponent<AyyNetwork>();
            }            
            */
            network = GetComponent<ayy.AyyNetwork>();
            map = GetComponent<ayy.MapMonoBehaviour>();
            
            //careKeyMap.Add(KeyCode.W,true);
            //careKeyMap.Add(KeyCode.S, true);
            //careKeyMap.Add(KeyCode.A, true);
            //careKeyMap.Add(KeyCode.D, true);

            network.GamePrepareEvent += OnStartLoadGame;
            network.GameTurnEvent += OnGameTurnMessage;
        }

        void Start()
        {

        }
        
        void Update()
        {
            float dt = Time.deltaTime;
            //UpdateForSendCtrl();
            foreach (Player player in playerMap.Values)
            {
                player.Update(dt);
            }
        }

        /*
        void UpdateForSendCtrl()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                network.ClientCtrlMove(MoveDir.Up);
                Debug.Log("up");
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                network.ClientCtrlMove(MoveDir.Down);
                Debug.Log("down");
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                network.ClientCtrlMove(MoveDir.Left);
                Debug.Log("left");
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                network.ClientCtrlMove(MoveDir.Right);
                Debug.Log("right");
            }

            foreach (KeyCode keyCode in careKeyMap.Keys)
            {
                if (Input.GetKeyDown(keyCode))
                {
                    network.ClientKeyPress(keyCode);
                }
                else if (Input.GetKeyUp(keyCode))
                {
                    network.ClientKeyRelease(keyCode);
                }
            }
        }
        */
        void OnStartLoadGame()
        {
            map.CreateMap();
            network.ClientReady();
        }

        void OnGameTurnMessage(int turnIndex,string turnJson)
        {
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
                    {
                        Player p = playerMap[clientId];
                        p.HandleMoveControl(msgContent);
                    }
                    break;
                case "game_client_empty":
                    //Debug.Log("client:" + clientId + " do nothing");
                    break;
                case "client_key_press":
                    {
                        Player p = playerMap[clientId];
                        KeyCode keyCode = (KeyCode)int.Parse(msgContent);
                        p.HandleKeyPress(keyCode);
                    }
                    break;
                case "client_key_release":
                    {
                        Player p = playerMap[clientId];
                        KeyCode keyCode = (KeyCode)int.Parse(msgContent);
                        p.HandleKeyRelease(keyCode);
                    }
                    break;
            }
        }
        
        private void OnPlayerSpawn(int clientId,int spawnPosIndex)
        {
            Vector3 pos = spawnPoints[spawnPosIndex];
            GameObject go = GameObject.Instantiate(playerPrefab, pos, Quaternion.identity);
            Player player = new Player(go);
            playerMap.Add(clientId, player); 
        }
    }
}
