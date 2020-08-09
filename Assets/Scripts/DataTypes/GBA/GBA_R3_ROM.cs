using System.Linq;

namespace R1Engine
{
    public class GBA_R3_ROM : GBA_ROM
    {
        // TODO: Only around the first half of the items are obj! What are the rest?
        public uint ObjBlocksOffsetTableCount { get; set; }
        public uint[] ObjBlocksOffsetTable { get; set; }
        public GBA_R3_MapObjBlock[] ObjBlocks { get; set; }


        // Each pointer leads to a small index list. They all begin with 0x00, so read until next 0x00?
        public Pointer[] UnkPointerTable { get; set; }

        // Contains general info about levels, but not anything map related
        public GBA_R3_LevelMapInfo[] LevelInfo { get; set; }


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

            // Get the pointer table
            var pointerTable = PointerTables.GetGBAR3PointerTable(Offset.file);

            // Serialize the object blocks offset table
            s.DoAt(pointerTable[GBA_R3_Pointer.ObjBlocksOffsetTable], () =>
            {
                ObjBlocksOffsetTableCount = s.Serialize<uint>(ObjBlocksOffsetTableCount, name: nameof(ObjBlocksOffsetTableCount));
                ObjBlocksOffsetTable = s.SerializeArray<uint>(ObjBlocksOffsetTable, ObjBlocksOffsetTableCount, name: nameof(ObjBlocksOffsetTable));
            });

            if (ObjBlocks == null)
                ObjBlocks = new GBA_R3_MapObjBlock[ObjBlocksOffsetTable.Length];

            // Serialize the object blocks
            for (int i = 0; i < ObjBlocks.Length; i++)
                ObjBlocks[i] = s.DoAt(pointerTable[GBA_R3_Pointer.ObjBlocksOffsetTable] + 4 + (ObjBlocksOffsetTable[i] * 4), () => s.SerializeObject<GBA_R3_MapObjBlock>(ObjBlocks[i], name: $"{nameof(ObjBlocks)}[{i}]"));

            // Serialize unknown pointer table
            UnkPointerTable = s.DoAt(pointerTable[GBA_R3_Pointer.UnkPointerTable], () => s.SerializePointerArray(UnkPointerTable, 252, name: nameof(UnkPointerTable)));

            // Serialize level info
            LevelInfo = s.DoAt(pointerTable[GBA_R3_Pointer.LevelInfo], () => s.SerializeObjectArray<GBA_R3_LevelMapInfo>(LevelInfo, 65, name: nameof(LevelInfo)));

            // Serialize current level maps
            var offset = Offset + 0x2E7288;

            // TODO: Not sure if all levels are in this structure and one after another like this - works for the first few
            void updateOffset(GBA_R3_MapBlock block)
            {
                offset += block.BlockSize + 4;
                offset += offset.AbsoluteOffset % 4;
                offset += 0x04;
            }

            BG_0_Parallax = s.DoAt(offset, () => s.SerializeObject<GBA_R3_BGParallax>(BG_0_Parallax, name: nameof(BG_0_Parallax)));
            offset += 0x80;
            offset += offset.AbsoluteOffset % 4;
            offset += 0x04;

            updateOffset(BG_0 = s.DoAt(offset, () => s.SerializeObject<GBA_R3_MapBlock>(BG_0, name: nameof(BG_0))));
            updateOffset(BG_1 = s.DoAt(offset, () => s.SerializeObject<GBA_R3_MapBlock>(BG_1, name: nameof(BG_1))));
            updateOffset(BG_2 = s.DoAt(offset, () => s.SerializeObject<GBA_R3_MapBlock>(BG_2, name: nameof(BG_2))));
            updateOffset(BG_3 = s.DoAt(offset, () => s.SerializeObject<GBA_R3_MapBlock>(BG_3, name: nameof(BG_3))));

            CollisionMap = s.DoAt(offset, () => s.SerializeObject<GBA_R3_CollisionMapBlock>(CollisionMap, name: nameof(CollisionMap)));

            // Serialize current level tilemap
            Tilemap = s.DoAt(Offset + 0x2ED078, () => s.SerializeArray<byte>(Tilemap, 32 * (BG_2.MapData.Max(x => BitHelpers.ExtractBits(x, 11, 0)) + 1), name: nameof(Tilemap)));

            // Serialize current level palettes
            BGPalette = s.DoAt(Offset + 0x30AFF0, () => s.SerializeObjectArray<ARGB1555Color>(BGPalette, 16 * 16, name: nameof(BGPalette)));
        }
    }

    // TODO: Move to separate files

    public class GBA_R3_MapObjBlock : R1Serializable
    {
        public byte ObjectsCount { get; set; }
        public byte Unk_01 { get; set; }
        public byte Unk_02 { get; set; }
        public byte Unk_03 { get; set; }

        // Might not be the exact same struct, but seems to match
        public GBA_R3_MapObj StartPosObj { get; set; }

        public GBA_R3_MapObj[] MapObjects { get; set; }

        // There are more bytes after this. Count seems to match ObjectsCount*4, but the data doesn't.

        public override void SerializeImpl(SerializerObject s)
        {
            ObjectsCount = s.Serialize<byte>(ObjectsCount, name: nameof(ObjectsCount));
            Unk_01 = s.Serialize<byte>(Unk_01, name: nameof(Unk_01));
            Unk_02 = s.Serialize<byte>(Unk_02, name: nameof(Unk_02));
            Unk_03 = s.Serialize<byte>(Unk_03, name: nameof(Unk_03));

            StartPosObj = s.SerializeObject<GBA_R3_MapObj>(StartPosObj, name: nameof(StartPosObj));

            MapObjects = s.SerializeObjectArray<GBA_R3_MapObj>(MapObjects, ObjectsCount, name: nameof(MapObjects));
        }
    }

    public class GBA_R3_MapObj : R1Serializable
    {
        // Almost always -1
        public int Unk_00 { get; set; }

        public ushort XPos { get; set; }
        public ushort YPos { get; set; }

        public byte Unk_08 { get; set; }
        public byte Unk_09 { get; set; }
        public byte Unk_0A { get; set; }

        // Seems to determine the state/animation/palette
        public byte Unk_0B { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Unk_00 = s.Serialize<int>(Unk_00, name: nameof(Unk_00));

            XPos = s.Serialize<ushort>(XPos, name: nameof(XPos));
            YPos = s.Serialize<ushort>(YPos, name: nameof(YPos));

            Unk_08 = s.Serialize<byte>(Unk_08, name: nameof(Unk_08));
            Unk_09 = s.Serialize<byte>(Unk_09, name: nameof(Unk_09));
            Unk_0A = s.Serialize<byte>(Unk_0A, name: nameof(Unk_0A));
            Unk_0B = s.Serialize<byte>(Unk_0B, name: nameof(Unk_0B));
        }
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