﻿using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro_AnimationBlock : BinarySerializable
    {
        public GBAIsometric_Spyro_Animation[] Animations { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            var animLength = Animations?.Length ?? -1;

            if (animLength == -1)
            {
                var firstAnim = s.DoAt(s.CurrentPointer, () => s.SerializeObject<GBAIsometric_Spyro_Animation>(default, name: $"{nameof(Animations)}[0]"));
                animLength = firstAnim.FrameOffset;
            }

            Animations = s.SerializeObjectArray<GBAIsometric_Spyro_Animation>(Animations, animLength, name: nameof(Animations));
        }
    }
}