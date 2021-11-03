using BinarySerializer;

namespace Ray1Map.GBA
{
    /// <summary>
    /// Information for a scene
    /// </summary>
    public class GBA_R3_SceneInfo : BinarySerializable
    {
        public ushort MusicIndex { get; set; }
        
        // Also music related?
        public ushort UnkIndex { get; set; }

        // Some offset? Seems to get bigger with each info struct. Is the same for some levels.
        public ushort Unk1 { get; set; }

        // Related to Unk1
        public ushort Unk2 { get; set; }

        public byte LumCount { get; set; }
        public byte CageCount { get; set; }

        // Either 0 or 1 - some bool?
        public ushort Unk3 { get; set; }

        public uint Unk4 { get; set; }

        public Pointer UnkPointer { get; set; }

        public byte Unk5 { get; set; }
        public byte Unk6 { get; set; }

        // The index of the level the map belongs to
        public ushort LevelIndex { get; set; }

        // Seems to be 0xFFFF for special maps (3D ones, world map, bosses etc.)
        public ushort Unk7 { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            MusicIndex = s.Serialize<ushort>(MusicIndex, name: nameof(MusicIndex));
            UnkIndex = s.Serialize<ushort>(UnkIndex, name: nameof(UnkIndex));
            Unk1 = s.Serialize<ushort>(Unk1, name: nameof(Unk1));
            Unk2 = s.Serialize<ushort>(Unk2, name: nameof(Unk2));
            LumCount = s.Serialize<byte>(LumCount, name: nameof(LumCount));
            CageCount = s.Serialize<byte>(CageCount, name: nameof(CageCount));
            Unk3 = s.Serialize<ushort>(Unk3, name: nameof(Unk3));
            Unk4 = s.Serialize<uint>(Unk4, name: nameof(Unk4));
            UnkPointer = s.SerializePointer(UnkPointer, name: nameof(UnkPointer));
            Unk5 = s.Serialize<byte>(Unk5, name: nameof(Unk5));
            Unk6 = s.Serialize<byte>(Unk6, name: nameof(Unk6));
            LevelIndex = s.Serialize<ushort>(LevelIndex, name: nameof(LevelIndex));
            Unk7 = s.Serialize<ushort>(Unk7, name: nameof(Unk7));
            s.Serialize<ushort>(0, name: "Padding");
        }
    }
}