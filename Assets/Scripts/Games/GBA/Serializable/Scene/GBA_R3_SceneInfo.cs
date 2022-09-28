using BinarySerializer;

namespace Ray1Map.GBA
{
    /// <summary>
    /// Information for a scene
    /// </summary>
    public class GBA_R3_SceneInfo : BinarySerializable
    {
        public ushort MusicIndex1 { get; set; }
        public ushort MusicIndex2 { get; set; } // When is this used?

        public ushort GlobalLumIndex { get; set; }
        public ushort Ushort_06 { get; set; } // Global cage index?

        public byte LumCount { get; set; }
        public byte CageCount { get; set; }

        // Either 0 or 1 - some bool?
        public ushort Ushort_0A { get; set; }

        public uint Uint_0C { get; set; }

        public Pointer LoadFunctionPointer { get; set; }

        public byte NextLevelIndex { get; set; }
        public byte Byte_15 { get; set; }

        // The index of the level the map belongs to
        public ushort LevelIndex { get; set; }

        // Seems to be 0xFFFF for special maps (3D ones, world map, bosses etc.)
        public ushort Ushort_18 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            MusicIndex1 = s.Serialize<ushort>(MusicIndex1, name: nameof(MusicIndex1));
            MusicIndex2 = s.Serialize<ushort>(MusicIndex2, name: nameof(MusicIndex2));
            GlobalLumIndex = s.Serialize<ushort>(GlobalLumIndex, name: nameof(GlobalLumIndex));
            Ushort_06 = s.Serialize<ushort>(Ushort_06, name: nameof(Ushort_06));
            LumCount = s.Serialize<byte>(LumCount, name: nameof(LumCount));
            CageCount = s.Serialize<byte>(CageCount, name: nameof(CageCount));
            Ushort_0A = s.Serialize<ushort>(Ushort_0A, name: nameof(Ushort_0A));
            Uint_0C = s.Serialize<uint>(Uint_0C, name: nameof(Uint_0C));
            LoadFunctionPointer = s.SerializePointer(LoadFunctionPointer, name: nameof(LoadFunctionPointer));
            NextLevelIndex = s.Serialize<byte>(NextLevelIndex, name: nameof(NextLevelIndex));
            Byte_15 = s.Serialize<byte>(Byte_15, name: nameof(Byte_15));
            LevelIndex = s.Serialize<ushort>(LevelIndex, name: nameof(LevelIndex));
            Ushort_18 = s.Serialize<ushort>(Ushort_18, name: nameof(Ushort_18));
            s.SerializePadding(2, logIfNotNull: true);
        }
    }
}