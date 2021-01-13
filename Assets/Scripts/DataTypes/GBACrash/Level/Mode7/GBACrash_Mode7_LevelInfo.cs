namespace R1Engine
{
    public class GBACrash_Mode7_LevelInfo : R1Serializable
    {
        public bool SerializeData { get; set; } // Set before serializing

        public uint LevelType { get; set; } // 0 or 1 since only two types are available per game
        public Pointer TileSetFramesPointer { get; set; }
        public uint TileSetFramesBlockLength { get; set; }
        public uint BackgroundIndex { get; set; }
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
        public RGBA5551Color[] ObjPalette { get; set; }
        public GBACrash_Mode7_ObjData ObjData { get; set; }
        public GBACrash_Mode7_AnimSet[] AnimSets { get; set; }
        public GBACrash_Mode7_ObjGraphics ObjGraphics { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LevelType = s.Serialize<uint>(LevelType, name: nameof(LevelType));
            TileSetFramesPointer = s.SerializePointer(TileSetFramesPointer, name: nameof(TileSetFramesPointer));
            TileSetFramesBlockLength = s.Serialize<uint>(TileSetFramesBlockLength, name: nameof(TileSetFramesBlockLength));
            BackgroundIndex = s.Serialize<uint>(BackgroundIndex, name: nameof(BackgroundIndex));
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

            TileSetFrames = s.DoAt(TileSetFramesPointer, () => s.SerializeObject<GBACrash_Mode7_TileFrames>(TileSetFrames, x => x.TileSetFramesBlockLength = TileSetFramesBlockLength, name: nameof(TileSetFrames)));
            ObjPalette = s.DoAt(ObjPalettePointer, () => s.SerializeObjectArray<RGBA5551Color>(ObjPalette, 256, name: nameof(ObjPalette)));
            ObjData = s.DoAt(ObjDataPointer, () => s.SerializeObject<GBACrash_Mode7_ObjData>(ObjData, name: nameof(ObjData)));

            var animSetsCount = LevelType == 0 ? 41 : 0;

            AnimSets = s.DoAt(AnimSetsPointer, () => s.SerializeObjectArray<GBACrash_Mode7_AnimSet>(AnimSets, animSetsCount, name: nameof(AnimSets)));
            ObjGraphics = s.DoAt(ObjGraphicsPointer, () => s.SerializeObject<GBACrash_Mode7_ObjGraphics>(ObjGraphics, x => x.AnimSets = AnimSets, name: nameof(ObjGraphics)));
        }
    }
}