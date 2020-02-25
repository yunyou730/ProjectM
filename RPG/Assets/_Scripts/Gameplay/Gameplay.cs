using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    public class GameplayEvent
    {
        public delegate void StartLoad();
        public static event StartLoad StartLoadEvent;
    }

}
