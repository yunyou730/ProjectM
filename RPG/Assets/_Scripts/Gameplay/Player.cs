using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ayy
{
    public class Player
    {
        int _connId = 0;
        GameObject _go = null;
        Dictionary<KeyCode, bool> _keyStateMap = new Dictionary<KeyCode, bool>();

        private static float moveSpeed = 10.0f;


        public Player(GameObject go)
        {
            _go = go;
        }

        public void Update(float deltaTime)
        {
            UpdateForCtrl(deltaTime);
        }


        private void UpdateForCtrl(float deltaTime)
        {
            Vector3 offset = new Vector3();
            if (IsKeyPressing(KeyCode.W))
            {
                offset.z += Time.deltaTime * moveSpeed;
            }
            if (IsKeyPressing(KeyCode.S))
            {
                offset.z -= Time.deltaTime * moveSpeed;
            }
            if (IsKeyPressing(KeyCode.A))
            {
                offset.x -= Time.deltaTime * moveSpeed;
            }
            if (IsKeyPressing(KeyCode.D))
            {
                offset.x += Time.deltaTime * moveSpeed;
            }
            _go.transform.Translate(offset);
        }

        public void HandleMoveControl(string strDir)
        {
            switch (strDir)
            {
                case "up":
                    _go.transform.Translate(new Vector3(0, 0, 1));
                    break;
                case "down":
                    _go.transform.Translate(new Vector3(0, 0, -1));
                    break;
                case "left":
                    _go.transform.Translate(new Vector3(-1, 0, 0));
                    break;
                case "right":
                    _go.transform.Translate(new Vector3(1, 0, 0));
                    break;
            }
        }

        public void HandleKeyPress(KeyCode keyCode)
        {
            if (_keyStateMap.ContainsKey(keyCode))
            {
                _keyStateMap[keyCode] = true;
            }
            else
            {
                _keyStateMap.Add(keyCode, true);
            }
        }

        public void HandleKeyRelease(KeyCode keyCode)
        {
            if (_keyStateMap.ContainsKey(keyCode))
            {
                _keyStateMap[keyCode] = false;
            }
            else
            {
                _keyStateMap.Add(keyCode, false);
            }
        }

        private bool IsKeyPressing(KeyCode keyCode)
        {
            if (!_keyStateMap.ContainsKey(keyCode))
            {
                return false;
            }
            return _keyStateMap[keyCode];
        }
    }
}

