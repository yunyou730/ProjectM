using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ayy
{
    public class CtrlCmdQueue
    {
        List<CtrlCommand> queue = new List<CtrlCommand>();

        public void Push(CtrlCommand cmd)
        {
            CtrlCommand firstNotStartCmd = null;
            for (int i = 0; i < queue.Count; i++)
            {
                if (!queue[i].HasStart())
                {
                    firstNotStartCmd = queue[i];
                    break;
                }
            }

            if (firstNotStartCmd == null)
            {
                queue.Add(cmd);
            }
            else if(!firstNotStartCmd.CheckSame(cmd))
            {
                queue.Add(cmd);
            }
        }


        public void OnCmdDone()
        {
            if(queue.Count > 0)
            {
                queue.RemoveAt(0);
            }
        }

        public CtrlCommand Peak()
        {
            if (queue.Count > 0)
            {
                return queue[0];
            }
            return null;
        }

        public int GetSize()
        {
            return queue.Count;
        }
    }
}

