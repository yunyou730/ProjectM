using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ayy
{
    public class Player
    {
        int _connId = 0;

        GameObject _go = null;
        //Dictionary<KeyCode, bool> _keyStateMap = new Dictionary<KeyCode, bool>();

        private static float moveSpeed = 10.0f;

        public PlayerInput input = new PlayerInput();

        public Player(GameObject go)
        {
            _go = go;
        }

        public void Update(float deltaTime)
        {
            //UpdateForCtrl(deltaTime);
        }

        public void TickByNetwork(float deltaTime)
        {
            TickForCtrl();
        }

        public GameObject GetGameObject()
        {
            return _go;
        }
        
        private void UpdateForCtrl(float deltaTime)
        {
            Vector3 offset = new Vector3();
            if (input.IsKeyPressing(KeyCode.W))
            {
                offset.z += deltaTime * moveSpeed;
            }
            if (input.IsKeyPressing(KeyCode.S))
            {
                offset.z -= deltaTime * moveSpeed;
            }
            if (input.IsKeyPressing(KeyCode.A))
            {
                offset.x -= deltaTime * moveSpeed;
            }
            if (input.IsKeyPressing(KeyCode.D))
            {
                offset.x += deltaTime * moveSpeed;
            }
            if (offset.magnitude > 0)
            {
                _go.transform.Translate(offset);    
            }
        }

        private void TickForCtrl()
        {
            Vector3 offset = new Vector3();
            if (input.IsKeyPressing(KeyCode.W))
            {
                offset.z += 1;
            }
            if (input.IsKeyPressing(KeyCode.S))
            {
                offset.z -= 1;
            }
            if (input.IsKeyPressing(KeyCode.A))
            {
                offset.x -= 1;
            }
            if (input.IsKeyPressing(KeyCode.D))
            {
                offset.x += 1;
            }
            if (offset.magnitude > 0)
            {
                _go.transform.Translate(offset);    
            }
        }

    }
}

