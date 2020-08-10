namespace R1Engine
{
    public class GBA_R3_ROM : GBA_ROM
    {
        public GBA_R3_OffsetTable UiOffsetTable { get; set; }

        public GBA_R3_LevelBlock LevelBlock { get; set; }


        // Each pointer leads to a small index list. They all begin with 0x00, so read until next 0x00? - probably irrelevant
        public Pointer[] UnkPointerTable { get; set; }

        // Contains general info about levels, but not anything map related
        public GBA_R3_LevelMapInfo[] LevelInfo { get; set; }

        public GBA_R3_UnkBlock UnkBlock { get; set; }

        public ARGB1555Color[] BGPalette { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            // TODO: Prototype has 64 maps
            const int levelCount = 65;

            // Get the pointer table
            var pointerTable = PointerTables.GetGBAR3PointerTable(Offset.file);

            // Serialize the offset table
            s.DoAt(pointerTable[GBA_R3_Pointer.UiOffsetTable], () => UiOffsetTable = s.SerializeObject<GBA_R3_OffsetTable>(UiOffsetTable, name: nameof(UiOffsetTable)));

            // Serialize the level block for the current level
                LevelBlock = s.DoAt(UiOffsetTable.GetPointer(s.Context.Settings.Level, true), () => s.SerializeObject<GBA_R3_LevelBlock>(LevelBlock, name: nameof(LevelBlock)));

            // Serialize unknown pointer table
            UnkPointerTable = s.DoAt(pointerTable[GBA_R3_Pointer.UnkPointerTable], () => s.SerializePointerArray(UnkPointerTable, 252, name: nameof(UnkPointerTable)));

            // Serialize level info
            LevelInfo = s.DoAt(pointerTable[GBA_R3_Pointer.LevelInfo], () => s.SerializeObjectArray<GBA_R3_LevelMapInfo>(LevelInfo, levelCount, name: nameof(LevelInfo)));

            // Serialize unknown block
            UnkBlock = s.DoAt(UiOffsetTable.GetPointer(128, true), () => s.SerializeObject<GBA_R3_UnkBlock>(UnkBlock, name: nameof(UnkBlock)));

        }
    }

    /*

    Maps:
    BG_0 - The background (usually clouds, the sky etc.)
    BG_1 - The secondary background (in the first level it's the island and mountains)
    BG_2 - The actual level map (background)
    BG_3 - The actual level map (foreground)

     */

    // TODO: Move to separate files

    public class GBA_R3_OffsetTable : R1Serializable
    {
        public uint OffsetsCount { get; set; }
        public uint[] Offsets { get; set; }

        public Pointer GetPointer(int index, bool includeHeader) => PointerTables.GetGBAR3PointerTable(Offset.file)[GBA_R3_Pointer.UiOffsetTable] + (Offsets[index] * 4) - (includeHeader ? 4 : 0);

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize the offset table
            OffsetsCount = s.Serialize<uint>(OffsetsCount, name: nameof(OffsetsCount));
            Offsets = s.SerializeArray<uint>(Offsets, OffsetsCount, name: nameof(Offsets));
        }
    }

    public class GBA_R3_UnkBlock : GBA_R3_BaseBlock
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
        Pirate = 25,
        LevelExit_Back = 30,
        Butterfly = 37,
        BreakableGround = 50,
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