using System.Linq;

namespace R1Engine
{
    public class GBA_R3_ROM : GBA_ROM
    {
        // Related the level maps
        public uint UnkOffsetTableCount { get; set; }
        public uint[] UnkOffsetTable { get; set; }
        public Pointer[] UnkOffsetTablePointers { get; set; }

        // Each pointer leads to a small index list. They all begin with 0x00, so read until next 0x00?
        public Pointer[] UnkPointerTable { get; set; }

        // Contains general info about levels, but not anything map related
        public GBA_R3_LevelMapInfo[] LevelInfo { get; set; }

        public GBA_R3_MapBlock BackgroundMap { get; set; }
        public GBA_R3_MapBlock ForegroundMap { get; set; }
        public GBA_R3_CollisionMapBlock CollisionMap { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            // Get the pointer table
            var pointerTable = PointerTables.GetGBAR3PointerTable(Offset.file);

            s.DoAt(pointerTable[GBA_R3_Pointer.UnkOffsetTable], () =>
            {
                UnkOffsetTableCount = s.Serialize<uint>(UnkOffsetTableCount, name: nameof(UnkOffsetTableCount));
                UnkOffsetTable = s.SerializeArray<uint>(UnkOffsetTable, UnkOffsetTableCount, name: nameof(UnkOffsetTable));
            });
            UnkPointerTable = s.DoAt(pointerTable[GBA_R3_Pointer.UnkPointerTable], () => s.SerializePointerArray(UnkPointerTable, 252, name: nameof(UnkPointerTable)));
            LevelInfo = s.DoAt(pointerTable[GBA_R3_Pointer.LevelInfo], () => s.SerializeObjectArray<GBA_R3_LevelMapInfo>(LevelInfo, 65, name: nameof(LevelInfo)));

            BackgroundMap = s.DoAt(Offset + 0x2E86DC, () => s.SerializeObject<GBA_R3_MapBlock>(BackgroundMap, name: nameof(BackgroundMap)));
            ForegroundMap = s.DoAt(Offset + 0x2EB258, () => s.SerializeObject<GBA_R3_MapBlock>(ForegroundMap, name: nameof(ForegroundMap)));
            CollisionMap = s.DoAt(Offset + 0x2EC7BC, () => s.SerializeObject<GBA_R3_CollisionMapBlock>(CollisionMap, name: nameof(CollisionMap)));

            // Parse the offset table
            UnkOffsetTablePointers = UnkOffsetTable.Select(x => pointerTable[GBA_R3_Pointer.UnkOffsetTable] + 4 + (x * 4)).ToArray();
        }
    }

    // TODO: Move to separate files

    public class GBA_R3_LevelMapInfo : R1Serializable
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

    public class GBA_R3_DLCMapInfo : R1Serializable
    {
        public ushort Unk1 { get; set; }
        public ushort MusicIndex { get; set; }
        public ushort Unk2 { get; set; }
        public ushort Unk3 { get; set; }
        public string FileName { get; set; }
        public uint FileSize { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Unk1 = s.Serialize<ushort>(Unk1, name: nameof(Unk1));
            MusicIndex = s.Serialize<ushort>(MusicIndex, name: nameof(MusicIndex));
            Unk2 = s.Serialize<ushort>(Unk2, name: nameof(Unk2));
            Unk3 = s.Serialize<ushort>(Unk3, name: nameof(Unk3));
            FileName = s.SerializeString(FileName, 32, name: nameof(FileName));
            FileSize = s.Serialize<uint>(FileSize, name: nameof(FileSize));
        }
    }

    public class GBA_R3_MapBlock : R1Serializable
    {
        // Always 0?
        public uint Unk1 { get; set; }

        public uint BlockSize { get; set; }

        // Flags?
        public uint Unk2 { get; set; }

        public ushort Width { get; set; }
        public ushort Height { get; set; }

        // Two ushorts?
        public uint Unk3 { get; set; }

        // Always 1?
        public uint Unk4 { get; set; }

        // The tile indexes
        public ushort[] MapData { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Unk1 = s.Serialize<uint>(Unk1, name: nameof(Unk1));
            BlockSize = s.Serialize<uint>(BlockSize, name: nameof(BlockSize));
            Unk2 = s.Serialize<uint>(Unk2, name: nameof(Unk2));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
            Unk3 = s.Serialize<uint>(Unk3, name: nameof(Unk3));
            Unk4 = s.Serialize<uint>(Unk4, name: nameof(Unk4));

            // TODO: It seems the compressed block contains more data than just the tile indexes?
            s.DoEncoded(new LZSSEncoder(), () => MapData = s.SerializeArray<ushort>(MapData, Width * Height, name: nameof(MapData)));
        }
    }

    public class GBA_R3_CollisionMapBlock : R1Serializable
    {
        // Always 0?
        public uint Unk1 { get; set; }

        public uint BlockSize { get; set; }

        // Flags?
        public uint Unk2 { get; set; }

        public ushort Width { get; set; }
        public ushort Height { get; set; }

        public GBA_TileCollisionType[] CollisionData { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Unk1 = s.Serialize<uint>(Unk1, name: nameof(Unk1));
            BlockSize = s.Serialize<uint>(BlockSize, name: nameof(BlockSize));
            Unk2 = s.Serialize<uint>(Unk2, name: nameof(Unk2));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));

            s.DoEncoded(new LZSSEncoder(), () => CollisionData = s.SerializeArray<GBA_TileCollisionType>(CollisionData, Width * Height, name: nameof(CollisionData)));
        }
    }

    public enum GBA_TileCollisionType : byte
    {
        Solid = 0x00,
        
        Hill_Slight_Left_1 = 0x12,
        Hill_Slight_Left_2 = 0x13,
        Hill_Slight_Right_2 = 0x14,
        Hill_Slight_Right_1 = 0x15,
        
        Reactionary_Up = 0x28,
        Reactionary_Down = 0x29,
        
        Hang = 0x2E,
        Climb = 0x2F,
        
        Water = 0x30,
        
        Empty = 0xFF
    }
}