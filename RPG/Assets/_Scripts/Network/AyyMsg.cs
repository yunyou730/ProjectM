using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ayy
{
    public class CustomMsgTypes
    {
        public const short InGameMsg = 1003;
    }


    public enum CustomMsgType
    {
        Lobby_CreateRoom = 1234,
        Lobby_Server_Prepare,       // server 通知 client  开始加载游戏 
        Lobby_Player_Ready,         // client 通知 server  加载完毕，可以开始游戏

        Game_Start,                 // server 通知 client  游戏正式开始，开启 LockStep 

        Game_Player_Move,           // 玩家操作  client 通知 server，server 做广播 
        Game_Player_Fire,           // 玩家操作  client 通知 server，server 做广播 
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
