using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ayy
{
    public class Player
    {
        int _connId = 0;

        GameObject _go = null;

        private static float moveSpeed = 10.0f;

        public PlayerInput input = new PlayerInput();
        public CtrlCmdQueue cmdQueue = new CtrlCmdQueue();

        public Player(GameObject go)
        {
            _go = go;
        }

        public void Update(float deltaTime)
        {
            UpdateExecuteCmd(deltaTime);
        }

        public void TickByNetwork(float deltaTime)
        {
            TickForCtrl();
        }

        public GameObject GetGameObject()
        {
            return _go;
        }

        private void UpdateExecuteCmd(float deltaTime)
        {
            CtrlCommand cmd = cmdQueue.Peak();
            if (cmd != null)
            {
                if (!cmd.HasStart())
                {
                    cmd.Start(this);
                }
                cmd.Update(this, deltaTime);
                if (cmd.IsDone())
                {
                    cmdQueue.OnCmdDone();
                }
            }
        }

        private void TickForCtrl()
        {
            if (input.CheckHasMoveCtrl())
            {
                CtrlCommand ctrlCmd = new CtrlCommand(input);
                cmdQueue.Push(ctrlCmd);
            }
        }
    }
}

