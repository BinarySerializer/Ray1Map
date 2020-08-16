using System;

namespace R1Engine
{
    public class GBA_ActorState : R1Serializable
    {
        public byte Byte_00 { get; set; }
        public byte Byte_01 { get; set; }
        public byte Byte_02 { get; set; }
        public byte Byte_03 { get; set; }
        public byte AnimationIndex { get; set; }
        public ActorStateFlags Flags { get; set; }

        // Related to the struct Byte_07 points to
        public sbyte Byte_06 { get; set; }

        // Some offset index
        public byte Byte_07 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
            Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
            AnimationIndex = s.Serialize<byte>(AnimationIndex, name: nameof(AnimationIndex));
            Flags = s.Serialize<ActorStateFlags>(Flags, name: nameof(Flags));
            Byte_06 = s.Serialize<sbyte>(Byte_06, name: nameof(Byte_06));
            Byte_07 = s.Serialize<byte>(Byte_07, name: nameof(Byte_07));
        }

        [Flags]
        public enum ActorStateFlags : byte
        {
            None = 0,

            IsFlipped = 1 << 0,
            UnkFlag_1 = 1 << 1,
            UnkFlag_2 = 1 << 2,
            UnkFlag_3 = 1 << 3,
            UnkFlag_4 = 1 << 4,
            UnkFlag_5 = 1 << 5,
            UnkFlag_6 = 1 << 6,
            UnkFlag_7 = 1 << 7,
        }
    }
}