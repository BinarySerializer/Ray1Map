using System;
using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_RHR_LevelInfo : BinarySerializable
    {
        public bool SerializeData { get; set; }

        public Pointer<GBAIsometric_RHR_LevelData> LevelDataPointer { get; set; }

        public ushort MurfyStampScore1 { get; set; }
        public ushort MurfyStampScore2 { get; set; }
        public ushort MurfyStampScore3 { get; set; }

        public short Short_0A { get; set; }

        // Some position or size?
        public ushort Short_0C { get; set; }
        public ushort Short_0E { get; set; }
        public ushort Short_10 { get; set; }

        public short Short_12 { get; set; }
        public short Short_14 { get; set; }
        public byte PaletteShiftIndex { get; set; } // Either 0, 1 or 2
        public byte Byte_17 { get; set; }
        public int Int_18 { get; set; }
        public int Int_1C { get; set; }

        public LevelFlags Flags { get; set; }
        public byte Byte_21 { get; set; }
        public byte Byte_22 { get; set; }
        public byte Byte_23 { get; set; }

        public Pointer<GBAIsometric_RHR_MapLayer> MapPointer { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LevelDataPointer = s.SerializePointer<GBAIsometric_RHR_LevelData>(LevelDataPointer, resolve: SerializeData, name: nameof(LevelDataPointer));

            MurfyStampScore1 = s.Serialize<ushort>(MurfyStampScore1, name: nameof(MurfyStampScore1));
            MurfyStampScore2 = s.Serialize<ushort>(MurfyStampScore2, name: nameof(MurfyStampScore2));
            MurfyStampScore3 = s.Serialize<ushort>(MurfyStampScore3, name: nameof(MurfyStampScore3));

            Short_0A = s.Serialize<short>(Short_0A, name: nameof(Short_0A));
            Short_0C = s.Serialize<ushort>(Short_0C, name: nameof(Short_0C));
            Short_0E = s.Serialize<ushort>(Short_0E, name: nameof(Short_0E));
            Short_10 = s.Serialize<ushort>(Short_10, name: nameof(Short_10));
            Short_12 = s.Serialize<short>(Short_12, name: nameof(Short_12));
            Short_14 = s.Serialize<short>(Short_14, name: nameof(Short_14));
            PaletteShiftIndex = s.Serialize<byte>(PaletteShiftIndex, name: nameof(PaletteShiftIndex));
            Byte_17 = s.Serialize<byte>(Byte_17, name: nameof(Byte_17));

            Int_18 = s.Serialize<int>(Int_18, name: nameof(Int_18));
            Int_1C = s.Serialize<int>(Int_1C, name: nameof(Int_1C));

            Flags = s.Serialize<LevelFlags>(Flags, name: nameof(Flags));
            Byte_21 = s.Serialize<byte>(Byte_21, name: nameof(Byte_21));
            Byte_22 = s.Serialize<byte>(Byte_22, name: nameof(Byte_22));
            Byte_23 = s.Serialize<byte>(Byte_23, name: nameof(Byte_23));

            MapPointer = s.SerializePointer(MapPointer, resolve: SerializeData, name: nameof(MapPointer));
        }

        [Flags]
        public enum LevelFlags : byte
        {
            None = 0,

            /// <summary>
            /// Indicates that the level doesn't count the player score
            /// </summary>
            NoScore = 1 << 0,

            /// <summary>
            /// Indicates that the level is a bonus level
            /// </summary>
            Bonus = 1 << 1,
        }
    }
}