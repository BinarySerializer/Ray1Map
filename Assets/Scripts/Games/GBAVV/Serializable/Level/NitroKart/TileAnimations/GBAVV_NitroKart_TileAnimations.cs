﻿using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_NitroKart_TileAnimations : BinarySerializable
    {
        public Pointer[] AnimationPointers { get; set; }

        // Serialized from pointers
        public GBAVV_NitroKart_TileAnimation[] Animations { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            AnimationPointers = s.SerializePointerArrayUntil(AnimationPointers, x => x == null, getLastObjFunc: () => null, name: nameof(AnimationPointers));

            if (Animations == null)
                Animations = new GBAVV_NitroKart_TileAnimation[AnimationPointers.Length];

            for (int i = 0; i < Animations.Length; i++)
                Animations[i] = s.DoAt(AnimationPointers[i], () => s.SerializeObject<GBAVV_NitroKart_TileAnimation>(Animations[i], name: $"{nameof(Animations)}[{i}]"));
        }
    }
}