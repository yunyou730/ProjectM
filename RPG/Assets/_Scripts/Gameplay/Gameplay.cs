using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEngine.SceneManagement;

namespace ayy
{
    public class Gameplay : MonoBehaviour
    {
        public static Gameplay instance = null;
        
        private AyyNetwork network = null;

        public GameObject playerPrefab = null;
        public Vector3[] spawnPoints = null;

        Dictionary<int, Player> playerMap = new Dictionary<int, Player>();


        Dictionary<int, Dictionary<KeyCode, bool>> clientKeyStateMap = new Dictionary<int, Dictionary<KeyCode, bool>>();
        //Dictionary<KeyCode, bool> careKeyMap = new Dictionary<KeyCode, bool>();

        GameObject root = null;
        ayy.MapMonoBehaviour map = null;

        private Camera mainCamera = null;
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            
            network = GetComponent<ayy.AyyNetwork>();
            //map = GetComponent<ayy.MapMonoBehaviour>();

            //careKeyMap.Add(KeyCode.W,true);
            //careKeyMap.Add(KeyCode.S, true);
            //careKeyMap.Add(KeyCode.A, true);
            //careKeyMap.Add(KeyCode.D, true);

            mainCamera = Camera.main;

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

        public static Gameplay GetInstance()
        {
            return instance;
        }

        public int GetMySessionId()
        {
            return network._client.sessionId;
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

        /*
            开始加载游戏，并在游戏加载完毕后，通知服务器加载完毕 
        */
        private void OnStartLoadGame()
        {
            SceneManager.LoadScene("Gameplay");
            StartCoroutine(DoStartLoadGame());
        }


        private IEnumerator DoStartLoadGame()
        {
            // load scene
            AsyncOperation opLoadGameScene = SceneManager.LoadSceneAsync("Gameplay");
            while (!opLoadGameScene.isDone)
            {
                yield return null;
            }

            // Create Root
            root = new GameObject();
            root.name = "root";
            yield return null;

            // Create Map
            GameObject mapPrefab = Resources.Load<GameObject>("Gameplay/MapHolder");
            GameObject mapHolder = GameObject.Instantiate(mapPrefab);
            map = mapHolder.GetComponent<ayy.MapMonoBehaviour>();
            map.transform.SetParent(root.transform);
            yield return null;

            map.CreateMap();
            yield return null;
            
            OnLoadGameDone();
        }

        private void OnLoadGameDone()
        {
            // Notify server load ready
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
            // Create player
            Vector3 pos = spawnPoints[spawnPosIndex];
            GameObject go = GameObject.Instantiate(playerPrefab, pos, Quaternion.identity);
            Player player = new Player(go);
            playerMap.Add(clientId, player);
            go.transform.SetParent(root.transform);
            
            // Init camera
            if (clientId == GetMySessionId())
            {
                InitCamera();
            }
        }

        private void InitCamera()
        {
            
        }
    }
}
