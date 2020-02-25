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
            /*
                string jsonStr = @"{
                'name':'miao',
                'gender':'male',
                'age':31,
                'girlfriend':'shenyizhi',
                'array':[1,'bbb','432'],
                'map':{
                    'key1':true,
                    'key2':2333
                }
            }";

            JsonData jd = JsonMapper.ToObject(jsonStr);
            JsonData array = jd["array"];
        
            for (int i = 0;i < array.Count;i++)
            {
                Debug.Log(array[i]);
            }

            JsonData map = jd["map"];
            ICollection<string> keys = map.Keys;
            foreach (var key in keys)
            {
                Debug.Log(map[key]);
            }
             */
            JsonData jd = JsonMapper.ToObject(turnJson);
            ICollection<string> keys = jd.Keys;
            foreach (var key in keys)
            {                 
                string clientId = (string)jd[key];
                //string msgType = jd[key]["msg_type"];
                //Debug.Log("msgType:" + msgType);
                Debug.Log("clientId:" + clientId);
            }
        }
    }

    public class GameplayEvent
    {
        public delegate void StartLoad();
        public static event StartLoad StartLoadEvent;
    }

}
