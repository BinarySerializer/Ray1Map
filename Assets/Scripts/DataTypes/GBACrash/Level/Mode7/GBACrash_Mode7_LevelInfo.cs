namespace R1Engine
{
    public class GBACrash_Mode7_LevelInfo : R1Serializable
    {
        public uint LevelType { get; set; } // 0 or 1 since only two types are available per game
        public Pointer TileSetFramesPointer { get; set; }
        public uint TileSetFramesBlockLength { get; set; }
        public uint BackgroundIndex { get; set; }
        public Pointer ObjPalettePointer { get; set; }
        public Pointer ObjDataPointer { get; set; }
        public Pointer Pointer_18 { get; set; }
        public Pointer ObjTileSetPointer { get; set; }
        public uint Uint_20 { get; set; }
        public uint Uint_24 { get; set; }
        public uint Uint_28 { get; set; }
        public uint Uint_2C { get; set; }
        public uint Uint_30 { get; set; }

        // Serialized from pointers
        public GBACrash_Mode7_TileFrames TileSetFrames { get; set; }
        public RGBA5551Color[] ObjPalette { get; set; }
        public GBACrash_Mode7_ObjData ObjData { get; set; }
        public GBACrash_TileSet ObjTileSet { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LevelType = s.Serialize<uint>(LevelType, name: nameof(LevelType));
            TileSetFramesPointer = s.SerializePointer(TileSetFramesPointer, name: nameof(TileSetFramesPointer));
            TileSetFramesBlockLength = s.Serialize<uint>(TileSetFramesBlockLength, name: nameof(TileSetFramesBlockLength));
            BackgroundIndex = s.Serialize<uint>(BackgroundIndex, name: nameof(BackgroundIndex));
            ObjPalettePointer = s.SerializePointer(ObjPalettePointer, name: nameof(ObjPalettePointer));
            ObjDataPointer = s.SerializePointer(ObjDataPointer, name: nameof(ObjDataPointer));
            Pointer_18 = s.SerializePointer(Pointer_18, name: nameof(Pointer_18));
            ObjTileSetPointer = s.SerializePointer(ObjTileSetPointer, name: nameof(ObjTileSetPointer));
            Uint_20 = s.Serialize<uint>(Uint_20, name: nameof(Uint_20));
            Uint_24 = s.Serialize<uint>(Uint_24, name: nameof(Uint_24));
            Uint_28 = s.Serialize<uint>(Uint_28, name: nameof(Uint_28));
            Uint_2C = s.Serialize<uint>(Uint_2C, name: nameof(Uint_2C));
            Uint_30 = s.Serialize<uint>(Uint_30, name: nameof(Uint_30));

            TileSetFrames = s.DoAt(TileSetFramesPointer, () => s.SerializeObject<GBACrash_Mode7_TileFrames>(TileSetFrames, x => x.TileSetFramesBlockLength = TileSetFramesBlockLength, name: nameof(TileSetFrames)));
            ObjPalette = s.DoAt(ObjPalettePointer, () => s.SerializeObjectArray<RGBA5551Color>(ObjPalette, 256, name: nameof(ObjPalette)));
            ObjData = s.DoAt(ObjDataPointer, () => s.SerializeObject<GBACrash_Mode7_ObjData>(ObjData, name: nameof(ObjData)));
            ObjTileSet = s.DoAt(ObjTileSetPointer, () => s.SerializeObject<GBACrash_TileSet>(ObjTileSet, name: nameof(ObjTileSet)));
        }
    }
}