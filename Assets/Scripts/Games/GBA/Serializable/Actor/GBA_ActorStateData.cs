﻿using BinarySerializer;

namespace Ray1Map.GBA
{
    public class GBA_ActorStateData : GBA_BaseBlock
    {
        public byte[] Data { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            Data = s.SerializeArray<byte>(Data, BlockSize, name: nameof(Data));
        }
    }
}