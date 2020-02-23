using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ayy
{
    public class AyyNetwork : MonoBehaviour
    {
        public AyyServer _server = null;
        public AyyClient _client = null;
        public bool IsServer { set; get; } = false;
        
        void Start()
        {
            Time.fixedDeltaTime = 1 / 60.0f;
        }

        void Update()
        {
            
        }


        public void StartAsServer()
        {
            _server = new AyyServer();
            _server.Start();
            IsServer = true;
        }

        public void StartAsClient()
        {
            _client = new AyyClient();
            _client.Start();
            IsServer = false;
        }

        private void FixedUpdate()
        {
            
        }

        public void Simulate()
        {

        }

        private void OnSimulateBefore()
        {


        }

        private void OnSimulateAfter()
        {

        }

        private void ExecuteCommand()
        {

        }
    }
}

