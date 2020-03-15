using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ayy
{
    public class PlayerInput 
    {
        public enum Usage
        {
            LocalControl,
            Communication,
        }

        public Usage usage { set; get; } = Usage.LocalControl;


        private Dictionary<KeyCode, bool> keyState = new Dictionary<KeyCode, bool>();
        private List<KeyCode> keySequence = new List<KeyCode>();

        public PlayerInput(Usage _usage)
        {
            usage = _usage;

            keySequence.Add(KeyCode.W);
            keySequence.Add(KeyCode.A);
            keySequence.Add(KeyCode.S);
            keySequence.Add(KeyCode.D);
            keySequence.Add(KeyCode.UpArrow);
            keySequence.Add(KeyCode.DownArrow);
            keySequence.Add(KeyCode.LeftArrow);
            keySequence.Add(KeyCode.RightArrow);
            keySequence.Add(KeyCode.Space);
            
            for (int i = 0;i < keySequence.Count;i++)
            {
                KeyCode keyCode = keySequence[i];
                keyState.Add(keyCode,false);    
            }
        }
        
        public int Marshal()
        {
            int result = 0;
            for (int i = 0;i < keySequence.Count;i++)
            {
                KeyCode keyCode = keySequence[i];
                if (keyState[keyCode])
                {
                    result += (1 << i);   
                }
            }
            return result;
        }

        public void UnMarshal(int mask)
        {
            //Debug.LogWarning("------- unmarshal -------------");
            //Debug.Log(Convert.ToString(mask,2));
            
            Clear();
            
            int bit = 0;
            while (mask > 0)
            {
                int bitValue = mask % 2;
                mask = (mask >> 1);
                KeyCode keyCode = keySequence[bit];
                keyState[keyCode] = bitValue > 0;
                bit++;
            }

            //Dump();
        }
        
        public void CollectSample()
        {
            KeyCode[] keyArray = keyState.Keys.ToArray();
            for (int i = 0;i < keyArray.Length;i++)
            {
                KeyCode keyCode = keyArray[i];
                if (Input.GetKeyDown(keyCode))
                {
                    SetKeyState(keyCode,true);
                }
                if (Input.GetKeyUp(keyCode))
                {
                    SetKeyState(keyCode,false);
                }   
            }
        }

        public bool IsKeyPressing(KeyCode keyCode)
        {
            if (keyState.ContainsKey(keyCode))
            {
                return keyState[keyCode];
            }
            return false;
        }

        public void Clear()
        {
            KeyCode[] keyArray = keyState.Keys.ToArray();
            for (int i = 0;i < keyArray.Length;i++)
            {
                KeyCode keyCode = keyArray[i];
                SetKeyState(keyCode,false);
            }
        }
        
        public void SetKeyState(KeyCode keyCode,bool bPress)
        {
            if (keyState.ContainsKey(keyCode))
            {
                keyState[keyCode] = bPress;
            }            
        }

        public void Dump()
        {
            foreach (KeyCode keyCode in keyState.Keys)
            {
                Debug.Log("[key] " + keyCode + " [state] " + keyState[keyCode]);
            }
        }


        public bool CheckHasMoveCtrl()
        {
            return IsKeyPressing(KeyCode.W) || IsKeyPressing(KeyCode.A) || IsKeyPressing(KeyCode.S) || IsKeyPressing(KeyCode.D);
        }

    }
}

