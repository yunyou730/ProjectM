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


    public enum MsgType
    {
        Lobby_CreateRoom = 1234,
        Lobby_Join,
        Lobby_Start,

        Game_Player_Ready,
        Game_Start,
        Game_Player_Move,
        Game_Player_Fire,
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

    
}
