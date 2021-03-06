﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ayy
{
    public enum CustomMsgType
    {
        Lobby_CreateRoom = 1234,
        Lobby_Server_Prepare,       // server 通知 client  开始加载游戏 
        Lobby_Player_Ready,         // client 通知 server  加载完毕，可以开始游戏
        Lobby_Server_Player_Join,          // server 通知 client 加入了新的 player
        Lobby_Server_Player_Left,        // Server 通知 client 有 player 离开了 
        Lobby_Server_Player_List,        // Server 通知 client 所有在房间的 player 列表 
        
        Game_Start,                 // server 通知 client  游戏正式开始，开启 LockStep 

        Game_LockStep_Turn,         // 服务器发送给 客户端 的 lockstep 轮次 数据  

        Game_Client_Ctrl,           // client 发送给 server 的 操作数据 
    }


    public class CustomMessage : MessageBase
    {
        public int _uuid;
        public string _content;
        public Vector3 _vector;
        public byte[] _bytes;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(_uuid);
            writer.Write(_content);
            writer.Write(_vector);
            writer.WriteBytesAndSize(_bytes,_bytes.Length);
        }

        public override void Deserialize(NetworkReader reader)
        {
            _uuid = reader.ReadInt32();
            _content = reader.ReadString();
            _vector = reader.ReadVector3();
            ushort count = reader.ReadUInt16();
            _bytes = reader.ReadBytes(count);
        }

        public override string ToString()
        {
            string uuid = "[uuid]" + _uuid;
            string vector = "[vector]" + "(" + _vector.x + "," + _vector.y + "," + _vector.z + ")";
            string byteLen = "[byte_len]" + _bytes.Length.ToString();
            return uuid + vector + byteLen;
        }
    }

    public class LobbyMessage : MessageBase
    {
        public int messageId;
        public string msgType;
        public string content;


        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(messageId);
            writer.Write(msgType);
            writer.Write(content);
        }

        public override void Deserialize(NetworkReader reader)
        {
            messageId = reader.ReadInt32();
            msgType = reader.ReadString();
            content = reader.ReadString();
        }
    }

    public class GameMessage : MessageBase
    {
        public int lockstepTurn;
        public string msgType;
        public string content;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(lockstepTurn);
            writer.Write(msgType);
            writer.Write(content);
        }

        public override void Deserialize(NetworkReader reader)
        {
            lockstepTurn = reader.ReadInt32();
            msgType = reader.ReadString();
            content = reader.ReadString();
        }
    }
}
