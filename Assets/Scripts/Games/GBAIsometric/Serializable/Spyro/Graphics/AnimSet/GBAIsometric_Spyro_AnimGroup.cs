﻿using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro_AnimGroup : BinarySerializable
    {
        public ushort AnimCount { get; set; }
        public ushort AnimIndex { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            AnimCount = s.Serialize<ushort>(AnimCount, name: nameof(AnimCount));
            AnimIndex = s.Serialize<ushort>(AnimIndex, name: nameof(AnimIndex));
        }
    }
}