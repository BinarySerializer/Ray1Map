﻿using BinarySerializer;

namespace Ray1Map.GBA
{
    public class GBA_ShanghaiLocalOffsetTable : BinarySerializable
    {
        public long Length { get; set; } // Set before serializing

        public uint[] Offsets { get; set; }

        public Pointer GetPointer(long index) => Offset + Offsets[index];

        public override void SerializeImpl(SerializerObject s)
        {
            Offsets = s.SerializeArray<uint>(Offsets, Length, name: nameof(Offsets));
        }
    }
}