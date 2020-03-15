using System;
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
        
        GameObject root = null;
        ayy.MapMonoBehaviour map = null;

        private GameObject mainCamera = null;
        private CameraController mainCameraCtrl = null;
        
        private float prevTurnTime = 0;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            
            network = GetComponent<ayy.AyyNetwork>();
            network.GamePrepareEvent += OnStartLoadGame;
            network.GameTurnEvent += OnGameTurnMessage;
        }

        void Start()
        {

        }

        void Update()
        {
            float dt = Time.deltaTime;
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

            InitCamera();

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
            network.ClientReady();
            
            // debug menu
            Dictionary<string, object> arg = new Dictionary<string, object>();
            arg.Add("menu_path","Menu/MenuInGameDebug");
            CmdCenter.GetInstance().RunCmd(new CmdOpenMenu(arg));

            // virtual pad
            arg = new Dictionary<string, object>();
            arg.Add("menu_path", "Menu/VirtualPad");
            CmdCenter.GetInstance().RunCmd(new CmdOpenMenu(arg));
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
            TickByNetwork(turnIndex);
        }


        private void TickByNetwork(int turnIndex)
        {
            if (turnIndex == 1)
            {
                prevTurnTime = Time.timeSinceLevelLoad;
                return;
            }

            float now = Time.timeSinceLevelLoad;
            float deltaTime = now - prevTurnTime;
            prevTurnTime = now;
        
            // do tick
            foreach (Player player in playerMap.Values)
            {
                player.TickByNetwork(deltaTime);
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
                case "client_ctrl_keymask":
                    {
                        int keyMask = Int32.Parse(msgContent);
                        //Debug.Log("[send key mask] " + Convert.ToString(keyMask,2));
                        Player p = playerMap[clientId];
                        p.input.UnMarshal(keyMask);
                    }
                    break;
                case "game_client_empty":
                    //Debug.Log("client:" + clientId + " do nothing");
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
                AssignCameraToMyPlayer();
            }
        }

        private void InitCamera()
        {
            mainCamera = Camera.main.gameObject;
            mainCameraCtrl = mainCamera.AddComponent<CameraController>();
        }

        private void AssignCameraToMyPlayer()
        {
            Player myPlayer = playerMap[GetMySessionId()];
            mainCameraCtrl.SetFollowTarget(myPlayer.GetGameObject());
        }
    }
}
