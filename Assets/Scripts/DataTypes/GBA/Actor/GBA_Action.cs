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
        public short Hitbox_XPos { get; set; }
        public short Hitbox_YPos { get; set; }
        public short Hitbox_Width { get; set; }
        public short Hitbox_Height { get; set; }

        public byte AnimationIndex { get; set; }
        public ActorStateFlags Flags { get; set; }

        // Related to the struct Byte_07 points to
        public sbyte StateDataType { get; set; }
        public byte StateDataOffsetIndex { get; set; }

        // Parsed
        public GBA_ActorStateData StateData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBA_BatmanVengeance) {
                Hitbox_XPos = s.Serialize<short>(Hitbox_XPos, name: nameof(Hitbox_XPos));
                Hitbox_YPos = s.Serialize<short>(Hitbox_YPos, name: nameof(Hitbox_YPos));
                Hitbox_Width = s.Serialize<short>(Hitbox_Width, name: nameof(Hitbox_Width));
                Hitbox_Height = s.Serialize<short>(Hitbox_Height, name: nameof(Hitbox_Height));
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