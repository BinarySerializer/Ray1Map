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


        // The background (usually clouds, the sky etc.)
        public GBA_R3_MapBlock BG_0 { get; set; }
        
        // The secondary background (in the first level it's the island and mountains)
        public GBA_R3_MapBlock BG_1 { get; set; }

        // The actual level map (background)
        public GBA_R3_MapBlock BG_2 { get; set; }

        // The actual level map (foreground)
        public GBA_R3_MapBlock BG_3 { get; set; }

        // The map collision
        public GBA_R3_CollisionMapBlock CollisionMap { get; set; }


        public byte[] Tilemap { get; set; }
        public ARGB1555Color[] Palette { get; set; }

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

            BG_0 = s.DoAt(Offset + 0x2E7308, () => s.SerializeObject<GBA_R3_MapBlock>(BG_0, name: nameof(BG_0)));
            BG_1 = s.DoAt(Offset + 0x2E8094, () => s.SerializeObject<GBA_R3_MapBlock>(BG_1, name: nameof(BG_1)));
            BG_2 = s.DoAt(Offset + 0x2E86DC, () => s.SerializeObject<GBA_R3_MapBlock>(BG_2, name: nameof(BG_2)));
            BG_3 = s.DoAt(Offset + 0x2EB258, () => s.SerializeObject<GBA_R3_MapBlock>(BG_3, name: nameof(BG_3)));
            CollisionMap = s.DoAt(Offset + 0x2EC7BC, () => s.SerializeObject<GBA_R3_CollisionMapBlock>(CollisionMap, name: nameof(CollisionMap)));

            Tilemap = s.DoAt(Offset + 0x2ED078, () => s.SerializeArray<byte>(Tilemap, 32 * (BG_2.MapData.Max(x => BitHelpers.ExtractBits(x, 11, 0)) + 1), name: nameof(Tilemap)));
            Palette = s.DoAt(Offset + 0x30AFF0, () => s.SerializeObjectArray<ARGB1555Color>(Palette, 16 * 16, name: nameof(Palette)));

            // Parse the offset table
            UnkOffsetTablePointers = UnkOffsetTable.Select(x => pointerTable[GBA_R3_Pointer.UnkOffsetTable] + 4 + (x * 4)).ToArray();
        }
    }

    // TODO: Move to separate files

    // FUN_080c5378 in proto uses this to load map layers

    // First  level is at 0x082E7288
    // Second level is at 0x08362544
    // 0x03000e20 has pointer to this struct for the current level during runtime

    // Always 128 bytes long - appears right before BG_0 for a map
    public class GBA_R3_Level : R1Serializable
    {
        // This determines how the level gets loaded (false == normal map, true == ?)
        public bool Unk_00 { get; set; }

        public byte Unk_01 { get; set; }
        public byte Unk_02 { get; set; }
        public byte Unk_03 { get; set; }

        public byte TileLayersCount { get; set; }
        public byte UnkIndexesCount { get; set; }
        
        public byte[] TileLayerIDs { get; set; }
        public byte[] UnkIndexes { get; set; }

        // More data - part of the same struct?

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Unk_00 = s.Serialize<bool>(Unk_00, name: nameof(Unk_00));
            Unk_01 = s.Serialize<byte>(Unk_01, name: nameof(Unk_01));
            Unk_02 = s.Serialize<byte>(Unk_02, name: nameof(Unk_02));
            Unk_03 = s.Serialize<byte>(Unk_03, name: nameof(Unk_03));
            TileLayersCount = s.Serialize<byte>(TileLayersCount, name: nameof(TileLayersCount));
            UnkIndexesCount = s.Serialize<byte>(UnkIndexesCount, name: nameof(UnkIndexesCount));
            TileLayerIDs = s.SerializeArray<byte>(TileLayerIDs, 4, name: nameof(TileLayerIDs));
            UnkIndexes = s.SerializeArray<byte>(UnkIndexes, 6, name: nameof(UnkIndexes));
        }
    }

    /*
     
    Structure of a DLC level (number of blocks might depend on GBA_R3_Level):

    uint: BlockLength
    byte[]: UnkBlock (same block type which UnkOffsetTable leads to!)
    uint: BlockLength ?
    byte[]: GBA_R3_Level (88 bytes?)
    uint: BlockLength
    byte[]: GBA_R3_MapBlock
    uint: BlockLength
    byte[]: GBA_R3_MapBlock
    uint: BlockLength
    byte[]: GBA_R3_MapBlock
    uint: BlockLength
    byte[]: GBA_R3_CollisionMapBlock

    First DLC level is 800x40 (20 03 28 00)
     
     */
}