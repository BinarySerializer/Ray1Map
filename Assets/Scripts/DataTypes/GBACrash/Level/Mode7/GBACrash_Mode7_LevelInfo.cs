using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    public class GBACrash_Mode7_LevelInfo : R1Serializable
    {
        public bool SerializeData { get; set; } // Set before serializing

        public uint LevelType { get; set; } // 0 or 1 since only two types are available per game
        public Pointer TileSetFramesPointer { get; set; }
        public uint TileSetFramesBlockLength { get; set; }
        public Pointer Crash1_BackgroundPointer { get; set; }
        public uint Crash2_BackgroundIndex { get; set; }
        public Pointer ObjPalettePointer { get; set; }
        public Pointer ObjDataPointer { get; set; }
        public Pointer AnimSetsPointer { get; set; }
        public Pointer ObjGraphicsPointer { get; set; }
        public uint Uint_20 { get; set; }
        public uint Uint_24 { get; set; }
        public uint Uint_28 { get; set; }
        public uint Uint_2C { get; set; }
        public uint Uint_30 { get; set; }

        // Serialized from pointers
        public GBACrash_Mode7_TileFrames TileSetFrames { get; set; }
        public GBACrash_Mode7_Background Crash1_Background { get; set; }
        public RGBA5551Color[] ObjPalette { get; set; }
        public GBACrash_Mode7_ObjData ObjData { get; set; }
        public GBACrash_Mode7_AnimSet[] AnimSets { get; set; }
        public GBACrash_Mode7_ObjGraphics ObjGraphics { get; set; }

        public GBACrash_Mode7_AnimSet AnimSet_Chase { get; set; } // Bear in Crash 1, Shark in Crash 2

        public IEnumerable<GBACrash_Mode7_AnimSet> GetAllAnimSets => LevelType == 0 ? AnimSets.Append(AnimSet_Chase) : AnimSets;

        public override void SerializeImpl(SerializerObject s)
        {
            LevelType = s.Serialize<uint>(LevelType, name: nameof(LevelType));
            TileSetFramesPointer = s.SerializePointer(TileSetFramesPointer, name: nameof(TileSetFramesPointer));
            TileSetFramesBlockLength = s.Serialize<uint>(TileSetFramesBlockLength, name: nameof(TileSetFramesBlockLength));

            if (s.GameSettings.EngineVersion == EngineVersion.GBACrash_Crash1)
                Crash1_BackgroundPointer = s.SerializePointer(Crash1_BackgroundPointer, name: nameof(Crash1_BackgroundPointer));
            else
                Crash2_BackgroundIndex = s.Serialize<uint>(Crash2_BackgroundIndex, name: nameof(Crash2_BackgroundIndex));

            ObjPalettePointer = s.SerializePointer(ObjPalettePointer, name: nameof(ObjPalettePointer));
            ObjDataPointer = s.SerializePointer(ObjDataPointer, name: nameof(ObjDataPointer));
            AnimSetsPointer = s.SerializePointer(AnimSetsPointer, name: nameof(AnimSetsPointer));
            ObjGraphicsPointer = s.SerializePointer(ObjGraphicsPointer, name: nameof(ObjGraphicsPointer));
            Uint_20 = s.Serialize<uint>(Uint_20, name: nameof(Uint_20));
            Uint_24 = s.Serialize<uint>(Uint_24, name: nameof(Uint_24));
            Uint_28 = s.Serialize<uint>(Uint_28, name: nameof(Uint_28));
            Uint_2C = s.Serialize<uint>(Uint_2C, name: nameof(Uint_2C));
            Uint_30 = s.Serialize<uint>(Uint_30, name: nameof(Uint_30));

            if (!SerializeData)
                return;

            TileSetFrames = s.DoAt(TileSetFramesPointer, () => s.SerializeObject<GBACrash_Mode7_TileFrames>(TileSetFrames, x =>
            {
                x.TileSetFramesBlockLength = TileSetFramesBlockLength;
                x.HasPaletteIndices = LevelType == 0 && s.GameSettings.EngineVersion == EngineVersion.GBACrash_Crash1;
            }, name: nameof(TileSetFrames)));

            Crash1_Background = s.DoAt(Crash1_BackgroundPointer, () => s.SerializeObject<GBACrash_Mode7_Background>(Crash1_Background, name: nameof(Crash1_Background)));

            ObjPalette = s.DoAt(ObjPalettePointer, () => s.SerializeObjectArray<RGBA5551Color>(ObjPalette, 256, name: nameof(ObjPalette)));
            ObjData = s.DoAt(ObjDataPointer, () => s.SerializeObject<GBACrash_Mode7_ObjData>(ObjData, name: nameof(ObjData)));

            int animSetsCount = 0;

            if (LevelType == 0)
                animSetsCount = 41;
            else if ((LevelType == 1 || LevelType == 2) && s.GameSettings.EngineVersion == EngineVersion.GBACrash_Crash1)
                animSetsCount = 47;
            else if (LevelType == 1 && s.GameSettings.EngineVersion == EngineVersion.GBACrash_Crash2)
                animSetsCount = 55;

            AnimSets = s.DoAt(AnimSetsPointer, () => s.SerializeObjectArray<GBACrash_Mode7_AnimSet>(AnimSets, animSetsCount, name: nameof(AnimSets)));

            if (LevelType == 0)
            {
                var pointerTable = PointerTables.GBACrash_PointerTable(s.GameSettings.GameModeSelection, Offset.file);

                AnimSet_Chase = s.SerializeObject<GBACrash_Mode7_AnimSet>(AnimSet_Chase, x =>
                {
                    x.SerializeValues = false;
                    x.AnimationsPointer = pointerTable[GBACrash_Pointer.Mode7_Type0_ChaseObjAnimations];
                    x.FrameOffsetsPointer = pointerTable[GBACrash_Pointer.Mode7_Type0_ChaseObjFrames];
                    x.PaletteIndex = (uint)(s.GameSettings.EngineVersion == EngineVersion.GBACrash_Crash1 ? 0x1F : 0x12); // Tile pal 0x0F and 0x02
                }, name: nameof(AnimSet_Chase));
            }

            ObjGraphics = s.DoAt(ObjGraphicsPointer, () => s.SerializeObject<GBACrash_Mode7_ObjGraphics>(ObjGraphics, x => x.AnimSets = GetAllAnimSets.ToArray(), name: nameof(ObjGraphics)));
        }
    }
}