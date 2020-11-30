namespace R1Engine
{
    public class GBC_PlayField : GBC_BaseBlock 
    {
        public uint Width { get; set; }
        public uint Height { get; set; }
        public MapTile[] MapTiles { get; set; }

        // GBC
        public byte PaletteCount { get; set; }
        public byte VRAMBank1MapCount { get; set; }
        public byte VRAMBank2MapCount { get; set; }
        public GBC_TileVRAMMap[] VRAMBank1Map { get; set; }
        public GBC_TileVRAMMap[] VRAMBank2Map { get; set; }
        public RGBA5551Color[] Palette { get; set; }

        // Parsed from offset table
        public GBC_TileKit TileKit { get; set; }
        public GBC_MapBlock BGMapTileNumbers { get; set; }
        public GBC_MapBlock BGMapAttributes { get; set; }
        public GBC_MapBlock Collision { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBC_R1) {
                Width = s.Serialize<byte>((byte)Width, name: nameof(Width));
                Height = s.Serialize<byte>((byte)Height, name: nameof(Height));
                PaletteCount = s.Serialize<byte>(PaletteCount, name: nameof(PaletteCount));
                VRAMBank1MapCount = s.Serialize<byte>(VRAMBank1MapCount, name: nameof(VRAMBank1MapCount));
                VRAMBank2MapCount = s.Serialize<byte>(VRAMBank2MapCount, name: nameof(VRAMBank2MapCount));
                VRAMBank1Map = s.SerializeObjectArray<GBC_TileVRAMMap>(VRAMBank1Map, VRAMBank1MapCount, name: nameof(VRAMBank1Map));
                VRAMBank2Map = s.SerializeObjectArray<GBC_TileVRAMMap>(VRAMBank2Map, VRAMBank2MapCount, name: nameof(VRAMBank2Map));
                Palette = s.SerializeObjectArray<RGBA5551Color>(Palette, PaletteCount * 4, name: nameof(Palette));


                BGMapTileNumbers = s.DoAt(DependencyTable.GetPointer(1), () => s.SerializeObject<GBC_MapBlock>(BGMapTileNumbers, onPreSerialize: b => {
                    b.Width = (byte)Width;
                    b.Height = (byte)Height;
                    b.Type = MapTile.GBC_TileType.BGMapTileNumbers;
                }, name: nameof(BGMapTileNumbers)));
                BGMapAttributes = s.DoAt(DependencyTable.GetPointer(2), () => s.SerializeObject<GBC_MapBlock>(BGMapAttributes, onPreSerialize: b => {
                    b.Width = (byte)Width;
                    b.Height = (byte)Height;
                    b.Type = MapTile.GBC_TileType.BGMapAttributes;
                }, name: nameof(BGMapTileNumbers)));
                Collision = s.DoAt(DependencyTable.GetPointer(3), () => s.SerializeObject<GBC_MapBlock>(Collision, onPreSerialize: b => {
                    b.Width = (byte)Width;
                    b.Height = (byte)Height;
                    b.Type = MapTile.GBC_TileType.Collision;
                }, name: nameof(BGMapTileNumbers)));
            } else {
                Width = s.Serialize<uint>(Width, name: nameof(Width));
                Height = s.Serialize<uint>(Height, name: nameof(Height));
                MapTiles = s.SerializeObjectArray<MapTile>(MapTiles, Width * Height, name: nameof(MapTiles));
            }

            TileKit = s.DoAt(DependencyTable.GetPointer(0), () => s.SerializeObject<GBC_TileKit>(TileKit, name: nameof(TileKit)));

        }
    }
}