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

        // Update is called once per frame
        void Update()
        {

        }


        void OnStartLoadGame()
        {
            map.CreateMap();
            network.ClientReady();
        }

        void OnGameTurnMessage(string turnJson)
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
            }
        }
        
        private void OnPlayerSpawn(int clientId,int spawnPosIndex)
        {
            Vector3 pos = spawnPoints[spawnPosIndex];
            GameObject playerObject = GameObject.Instantiate(playerPrefab, pos, Quaternion.identity);
            playerMap.Add(clientId,playerObject); 
        }
    }
}
