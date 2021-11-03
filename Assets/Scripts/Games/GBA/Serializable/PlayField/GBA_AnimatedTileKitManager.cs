﻿using BinarySerializer;

namespace Ray1Map.GBA
{
    public class GBA_AnimatedTileKitManager : GBA_BaseBlock {
        public byte Length { get; set; }
        public byte[] TileKitBlocks { get; set; }

        public override void SerializeBlock(SerializerObject s) {
            Length = s.Serialize<byte>(Length, name: nameof(Length));
            TileKitBlocks = s.SerializeArray<byte>(TileKitBlocks, Length, name: nameof(TileKitBlocks));
        }
    }
}