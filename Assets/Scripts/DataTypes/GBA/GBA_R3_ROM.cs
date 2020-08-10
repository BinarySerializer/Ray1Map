using System.Linq;

namespace R1Engine
{
    public class GBA_R3_ROM : GBA_ROM
    {
        // Consists of 129 offsets. First 65 are for obj blocks for each map. Very last on is the start of 
        public uint OffsetTableCount { get; set; }
        public uint[] OffsetTable { get; set; }

        public GBA_R3_MapActorsBlock[] ActorBlocks { get; set; }
        public GBA_R3_UnkObjBlock[] UnkActorBlocks { get; set; }


        // Each pointer leads to a small index list. They all begin with 0x00, so read until next 0x00?
        public Pointer[] UnkPointerTable { get; set; }

        // Contains general info about levels, but not anything map related
        public GBA_R3_LevelMapInfo[] LevelInfo { get; set; }


        public GBA_R3_UnkLevelBlock UnkBlock { get; set; }

        public GBA_R3_BGParallax BG_0_Parallax { get; set; }

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

            // Helper for getting a pointer from the offset table
            Pointer getPointer(byte index, bool includeHeader) => pointerTable[GBA_R3_Pointer.OffsetTable] + (OffsetTable[index] * 4) - (includeHeader ? 4 : 0);

            // Serialize the offset table
            s.DoAt(pointerTable[GBA_R3_Pointer.OffsetTable], () =>
            {
                OffsetTableCount = s.Serialize<uint>(OffsetTableCount, name: nameof(OffsetTableCount));
                OffsetTable = s.SerializeArray<uint>(OffsetTable, OffsetTableCount, name: nameof(OffsetTable));
            });

            if (ActorBlocks == null)
                ActorBlocks = new GBA_R3_MapActorsBlock[levelCount];
            if (UnkActorBlocks == null)
                UnkActorBlocks = new GBA_R3_UnkObjBlock[levelCount];

            // Serialize the actor blocks
            for (byte i = 0; i < levelCount; i++)
            {
                s.DoAt(getPointer(i, true), () =>
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
            var offset = getPointer(128, true);

            // Serialize unknown block
            updateOffset(UnkBlock = s.DoAt(offset, () => s.SerializeObject<GBA_R3_UnkLevelBlock>(UnkBlock, name: nameof(UnkBlock))));

            // TODO: Not sure if all levels are in this structure and one after another like this - works for the first few
            void updateOffset(GBA_R3_BaseBlock block)
            {
                offset += block.BlockSize + 4;
                
                // Align
                if (offset.AbsoluteOffset % 4 != 0)
                    offset += 4 - (offset.AbsoluteOffset % 4);
                
                offset += 0x04;
            }

            // Serialize parallax info
            updateOffset(BG_0_Parallax = s.DoAt(offset, () => s.SerializeObject<GBA_R3_BGParallax>(BG_0_Parallax, name: nameof(BG_0_Parallax))));

            offset += 0x6C;
            offset += 0x04;

            // Serialize maps
            updateOffset(BG_0 = s.DoAt(offset, () => s.SerializeObject<GBA_R3_MapBlock>(BG_0, name: nameof(BG_0))));
            updateOffset(BG_1 = s.DoAt(offset, () => s.SerializeObject<GBA_R3_MapBlock>(BG_1, name: nameof(BG_1))));
            updateOffset(BG_2 = s.DoAt(offset, () => s.SerializeObject<GBA_R3_MapBlock>(BG_2, name: nameof(BG_2))));
            updateOffset(BG_3 = s.DoAt(offset, () => s.SerializeObject<GBA_R3_MapBlock>(BG_3, name: nameof(BG_3))));

            // Serialize collision
            CollisionMap = s.DoAt(offset, () => s.SerializeObject<GBA_R3_CollisionMapBlock>(CollisionMap, name: nameof(CollisionMap)));

            // Serialize current level tilemap
            Tilemap = s.DoAt(Offset + 0x2ED078, () => s.SerializeArray<byte>(Tilemap, 32 * (BG_2.MapData.Max(x => BitHelpers.ExtractBits(x, 11, 0)) + 1), name: nameof(Tilemap)));

            // Serialize current level palettes
            BGPalette = s.DoAt(Offset + 0x30AFF0, () => s.SerializeObjectArray<ARGB1555Color>(BGPalette, 16 * 16, name: nameof(BGPalette)));
        }
    }

    // TODO: Move to separate files

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