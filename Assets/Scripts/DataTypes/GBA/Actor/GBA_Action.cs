using System;

namespace R1Engine
{
    public class GBA_Action : R1Serializable
    {
        public byte Byte_00 { get; set; }
        public byte Byte_01 { get; set; }
        public byte Byte_02 { get; set; }
        public byte Byte_03 { get; set; }

        // Batman
        public short Short_00 { get; set; }
        public short Short_02 { get; set; }
        public short Short_04 { get; set; }
        public short Short_06 { get; set; }

        public byte AnimationIndex { get; set; }
        public ActorStateFlags Flags { get; set; }

        // Related to the struct Byte_07 points to
        public sbyte StateDataType { get; set; }
        public byte StateDataOffsetIndex { get; set; }

        // Parsed
        public GBA_ActorStateData StateData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Are these x/y min/max values for the hitbox?
            if (s.GameSettings.EngineVersion <= EngineVersion.GBA_BatmanVengeance) {
                Short_00 = s.Serialize<short>(Short_00, name: nameof(Short_00));
                Short_02 = s.Serialize<short>(Short_02, name: nameof(Short_02));
                Short_04 = s.Serialize<short>(Short_04, name: nameof(Short_04));
                Short_06 = s.Serialize<short>(Short_06, name: nameof(Short_06));
            } else {
                Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
                Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
                Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
                Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
            }
            AnimationIndex = s.Serialize<byte>(AnimationIndex, name: nameof(AnimationIndex));
            Flags = s.Serialize<ActorStateFlags>(Flags, name: nameof(Flags));
            StateDataType = s.Serialize<sbyte>(StateDataType, name: nameof(StateDataType));
            StateDataOffsetIndex = s.Serialize<byte>(StateDataOffsetIndex, name: nameof(StateDataOffsetIndex));
        }

        [Flags]
        public enum ActorStateFlags : byte
        {
            None = 0,

            HorizontalFlip = 1 << 0,
            VerticalFlip = 1 << 1,
            UnkFlag_2 = 1 << 2,
            UnkFlag_3 = 1 << 3,
            UnkFlag_4 = 1 << 4,
            UnkFlag_5 = 1 << 5,
            UnkFlag_6 = 1 << 6,
            UnkFlag_7 = 1 << 7,
        }
    }
}