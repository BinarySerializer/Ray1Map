namespace R1Engine
{
    public class GBA_R3_ROM : GBA_ROM
    {
        // Consists of 129 offsets. First 65 are for obj blocks for each map. Very last on is the start of 
        public GBA_R3_OffsetTable UiOffsetTable { get; set; }

        public GBA_R3_MapActorsBlock[] ActorBlocks { get; set; }
        public GBA_R3_UnkObjBlock[] UnkActorBlocks { get; set; }


        // Each pointer leads to a small index list. They all begin with 0x00, so read until next 0x00?
        public Pointer[] UnkPointerTable { get; set; }

        // Contains general info about levels, but not anything map related
        public GBA_R3_LevelMapInfo[] LevelInfo { get; set; }

        // Probably irrelevant
        public GBA_R3_UnkLevelBlock UnkBlock { get; set; }

        public GBA_R3_MapHeader MapHeader { get; set; }

        public GBA_R3_OffsetTable MapOffsetTable { get; set; }

        /*
         
        Maps:
        BG_0 - The background (usually clouds, the sky etc.)
        BG_1 - The secondary background (in the first level it's the island and mountains)
        BG_2 - The actual level map (background)
        BG_3 - The actual level map (foreground)
         
         */
        public GBA_R3_MapBlock[] Maps { get; set; }

        public GBA_R3_TileMap Tilemap { get; set; }
        public ARGB1555Color[] BGPalette { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            const int levelCount = 65;
            
            // Get the pointer table
            var pointerTable = PointerTables.GetGBAR3PointerTable(Offset.file);

            // Helper for getting a pointer from the offset tables
            Pointer getPointer(GBA_R3_OffsetTable table, byte index, bool includeHeader) => pointerTable[GBA_R3_Pointer.UiOffsetTable] + (table.Offsets[index] * 4) - (includeHeader ? 4 : 0);

            // Serialize the offset table
            s.DoAt(pointerTable[GBA_R3_Pointer.UiOffsetTable], () => UiOffsetTable = s.SerializeObject<GBA_R3_OffsetTable>(UiOffsetTable, name: nameof(UiOffsetTable)));

            if (ActorBlocks == null)
                ActorBlocks = new GBA_R3_MapActorsBlock[levelCount];
            if (UnkActorBlocks == null)
                UnkActorBlocks = new GBA_R3_UnkObjBlock[levelCount];

            // Serialize the actor blocks
            for (byte i = 0; i < levelCount; i++)
            {
                s.DoAt(getPointer(UiOffsetTable, i, true), () =>
                {
                    // Serialize actor block
                    ActorBlocks[i] = s.SerializeObject<GBA_R3_MapActorsBlock>(ActorBlocks[i], name: $"{nameof(ActorBlocks)}[{i}]");

                    // Align
                    s.Align();

                    // TODO: Sometimes there seems to be another block here
                    // Serialize unknown actor block
                    //UnkObjBlocks[i] = s.SerializeObject<GBA_R3_UnkObjBlock>(UnkObjBlocks[i], name: $"{nameof(UnkObjBlocks)}[{i}]");
                });
            }

            // Serialize unknown pointer table
            UnkPointerTable = s.DoAt(pointerTable[GBA_R3_Pointer.UnkPointerTable], () => s.SerializePointerArray(UnkPointerTable, 252, name: nameof(UnkPointerTable)));

            // Serialize level info
            LevelInfo = s.DoAt(pointerTable[GBA_R3_Pointer.LevelInfo], () => s.SerializeObjectArray<GBA_R3_LevelMapInfo>(LevelInfo, levelCount, name: nameof(LevelInfo)));

            // Serialize current level maps
            var offset = getPointer(UiOffsetTable, 128, true);

            s.DoAt(offset, () =>
            {
                // Serialize unknown block
                UnkBlock = s.SerializeObject<GBA_R3_UnkLevelBlock>(UnkBlock, name: nameof(UnkBlock));

                s.Align();
                s.Goto(s.CurrentPointer + 4);

                // Serialize map header
                MapHeader = s.SerializeObject<GBA_R3_MapHeader>(MapHeader, name: nameof(MapHeader));

                s.Align();

                // Serialize map offset table
                MapOffsetTable = s.SerializeObject<GBA_R3_OffsetTable>(MapOffsetTable, name: nameof(MapOffsetTable));

                // TODO: Before the map blocks are 3 16-byte blocks

                if (Maps == null)
                    Maps = new GBA_R3_MapBlock[MapHeader.MapCount];

                // Serialize maps
                for (int i = 0; i < MapHeader.MapCount; i++)
                    Maps[i] = s.DoAt(getPointer(MapOffsetTable, MapHeader.MapIndexes[i], true), () => s.SerializeObject<GBA_R3_MapBlock>(Maps[i], name: $"{nameof(Maps)}[{i}]"));

                // Serialize tilemap
                Tilemap = s.DoAt(getPointer(MapOffsetTable, 9, true), () => s.SerializeObject<GBA_R3_TileMap>(Tilemap, name: nameof(Tilemap)));
            });

            // Serialize current level palettes
            BGPalette = s.DoAt(Offset + 0x30AFF0, () => s.SerializeObjectArray<ARGB1555Color>(BGPalette, 16 * 16, name: nameof(BGPalette)));
        }
    }

    // TODO: Move to separate files

    public class GBA_R3_TileMap : GBA_R3_BaseBlock
    {
        public byte[] UnkData { get; set; }

        public byte[] TileMapData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize block header
            base.SerializeImpl(s);

            UnkData = s.SerializeArray<byte>(UnkData, 12, name: nameof(UnkData));

            // Serialize tilemap data
            TileMapData = s.SerializeArray<byte>(TileMapData, (2048 - 1) * 32, name: nameof(TileMapData));
        }

    }

    public class GBA_R3_OffsetTable : R1Serializable
    {
        public uint OffsetsCount { get; set; }
        public uint[] Offsets { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize the offset table
            OffsetsCount = s.Serialize<uint>(OffsetsCount, name: nameof(OffsetsCount));
            Offsets = s.SerializeArray<uint>(Offsets, OffsetsCount, name: nameof(Offsets));
        }
    }

    public class GBA_R3_UnkLevelBlock : GBA_R3_BaseBlock
    {
        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize block header
            base.SerializeImpl(s);

            Data = s.SerializeArray<byte>(Data, BlockSize, name: nameof(Data));
        }
    }

    public class GBA_R3_UnkObjBlock : GBA_R3_BaseBlock
    {
        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize block header
            base.SerializeImpl(s);

            Data = s.SerializeArray<byte>(Data, BlockSize, name: nameof(Data));
        }
    }

    // Event types have to be ushorts right now even though this is a byte
    public enum GBA_R3_ActorID : ushort
    {
        MovingPlatform_Vertical = 8,
        Switch = 10,
        YellowLum = 12,
        LevelExit_Back = 30,
        Butterfly = 37,
        MurfyStone = 92,
        LevelExit_Next = 101
    }

    /*
     
    Structure of a DLC level (number of blocks might depend on GBA_R3_Level):

    uint: BlockLength
    byte[]: GBA_R3_MapObjBlock
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

    First DLC map is 800x40 (20 03 28 00)
     
     */
}