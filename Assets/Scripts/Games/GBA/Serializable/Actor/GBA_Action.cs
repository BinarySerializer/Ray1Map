using System;
using BinarySerializer;

namespace Ray1Map.GBA
{
    public class GBA_Action : BinarySerializable
    {
        public byte Byte_00 { get; set; }
        public byte Byte_01 { get; set; }
        public byte Byte_02 { get; set; }
        public byte Byte_03 { get; set; }

        // Batman
        public short Hitbox_Y1 { get; set; }
        public short Hitbox_X1 { get; set; }
        public short Hitbox_Y2 { get; set; }
        public short Hitbox_X2 { get; set; }

        public ushort AnimationIndex { get; set; }
        public ActorStateFlags Flags { get; set; }

        // Related to the struct Byte_07 points to
        public sbyte StateDataType { get; set; }
        public byte Index_StateData { get; set; }

        // Milan
        public ushort Milan_Ushort_00 { get; set; }
        public uint Milan_Uint_04 { get; set; } // Seems to determine the length of the remaining data in the block

        // Parsed
        public GBA_ActorStateData StateData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GetR1Settings().GBA_IsMilan)
            {
                Milan_Ushort_00 = s.Serialize<ushort>(Milan_Ushort_00, name: nameof(Milan_Ushort_00));
                AnimationIndex = s.Serialize<ushort>(AnimationIndex, name: nameof(AnimationIndex));
                Milan_Uint_04 = s.Serialize<uint>(Milan_Uint_04, name: nameof(Milan_Uint_04));
            }
            else
            {
                if (s.GetR1Settings().EngineVersion == EngineVersion.GBA_BatmanVengeance)
                {
                    Hitbox_Y1 = s.Serialize<short>(Hitbox_Y1, name: nameof(Hitbox_Y1));
                    Hitbox_X1 = s.Serialize<short>(Hitbox_X1, name: nameof(Hitbox_X1));
                    Hitbox_Y2 = s.Serialize<short>(Hitbox_Y2, name: nameof(Hitbox_Y2));
                    Hitbox_X2 = s.Serialize<short>(Hitbox_X2, name: nameof(Hitbox_X2));
                }
                else
                {
                    Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
                    Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
                    Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
                    Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
                }
                AnimationIndex = s.Serialize<byte>((byte)AnimationIndex, name: nameof(AnimationIndex));
                Flags = s.Serialize<ActorStateFlags>(Flags, name: nameof(Flags));
                StateDataType = s.Serialize<sbyte>(StateDataType, name: nameof(StateDataType));
                Index_StateData = s.Serialize<byte>(Index_StateData, name: nameof(Index_StateData));
            }
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