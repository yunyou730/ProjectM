using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ayy
{
    public class CtrlCommand
    {
        bool bStart = false;
        bool bDone = false;


        Vector3 moveDir = new Vector3();
        public float moveSpeed = 5;
        private Vector3 destPos;
        public float moveDistance = 0.5f;
        
        public CtrlCommand(PlayerInput input)
        {
            InitMove(input);
        }

        private void InitMove(PlayerInput input)
        {
            bool bHorizonMove = input.IsKeyPressing(KeyCode.A) || input.IsKeyPressing(KeyCode.D);
            if (bHorizonMove)
            {
                if (input.IsKeyPressing(KeyCode.A))
                {
                    moveDir.x -= 1;
                }
                if (input.IsKeyPressing(KeyCode.D))
                {
                    moveDir.x += 1;
                }
            }
            else
            {
                if (input.IsKeyPressing(KeyCode.W))
                {
                    moveDir.z += 1;
                }
                if (input.IsKeyPressing(KeyCode.S))
                {
                    moveDir.z -= 1;
                }
            }

            



            moveDir.Normalize();
        }

        public bool CheckSame(CtrlCommand other)
        {
            return other.GetMoveDir() == GetMoveDir();
        }

        public bool HasStart()
        {
            return bStart;
        }


        public Vector3 GetMoveDir()
        {
            return moveDir;
        }

        public void Start(Player player)
        {
            bStart = true;
            bDone = false;
            destPos = player.GetGameObject().transform.position + moveDir * moveDistance;
        }

        public bool IsDone()
        {
            return bDone;
        }

        public void Update(Player player,float deltaTime)
        {
            UpdateForMove(player,deltaTime);
        }

        private void UpdateForMove(Player player,float deltaTime)
        {
            Vector3 curPos = player.GetGameObject().transform.position;
            float moveDis = deltaTime * moveSpeed;
            float toDestDis = (destPos - curPos).magnitude;
            moveDis = moveDis <= toDestDis ? moveDis : toDestDis;
            player.GetGameObject().transform.position += moveDir * moveDis;

            if (toDestDis <= moveDis)
            {
                bDone = true;
            }
        }
    }
}

