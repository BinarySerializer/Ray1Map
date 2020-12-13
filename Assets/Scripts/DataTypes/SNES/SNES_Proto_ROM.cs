namespace R1Engine
{
    public class SNES_Proto_ROM : R1Serializable
    {
        public RGBA5551Color[] TilePalette { get; set; }
        public RGBA5551Color[] SpritePalette { get; set; }

        public byte[] TileSet_0000 { get; set; } // 4bpp for normal and foreground map
        public byte[] TileSet_8000 { get; set; } // 2bpp, for background map
        // TODO: Animated tiles (OAM2)

        public MapData BG1_Map { get; set; }
        public MapTile[] BG1_Tiles { get; set; }
        public MapTile[] BG2_Tiles { get; set; } // Foreground
        public MapTile[] BG3_Tiles { get; set; } // Background

        public SNES_Proto_Object Rayman { get; set; }
        public byte[] SpriteTileSet { get; set; }
        public byte[] SpriteTileSetAdd0 { get; set; }
        public byte[] SpriteTileSetAdd1 { get; set; }
        public byte[] SpriteTileSetAdd2 { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize palettes
            s.DoAt(s.CurrentPointer + 0x2ADC4, () =>
            {
                TilePalette = s.SerializeObjectArray<RGBA5551Color>(TilePalette, 8 * 16, name: nameof(TilePalette));
                SpritePalette = s.SerializeObjectArray<RGBA5551Color>(SpritePalette, 8 * 16, name: nameof(SpritePalette));
            });

            // Serialize tile sets
            TileSet_0000 = s.DoAt(s.CurrentPointer + 0x30000, () => s.SerializeArray<byte>(TileSet_0000, 1024 * 32, name: nameof(TileSet_0000)));
            TileSet_8000 = s.DoAt(s.CurrentPointer + 0x36F00, () => s.SerializeArray<byte>(TileSet_8000, 21 * 16, name: nameof(TileSet_8000)));

            // Serialize maps
            BG1_Map = s.DoAt(s.CurrentPointer + 0x28000, () => s.SerializeObject<MapData>(BG1_Map, name: nameof(BG1_Map)));
            BG1_Tiles = s.DoAt(s.CurrentPointer + 0x1AAF8, () => s.SerializeObjectArray<MapTile>(BG1_Tiles, 1024 * 4, onPreSerialize: x => x.SNES_Is8PxTile = true, name: nameof(BG1_Tiles)));
            BG2_Tiles = s.DoAt(s.CurrentPointer + 0x29dc4, () => s.SerializeObjectArray<MapTile>(BG2_Tiles, 32 * 32, onPreSerialize: x => x.SNES_Is8PxTile = true, name: nameof(BG2_Tiles)));
            BG3_Tiles = s.DoAt(s.CurrentPointer + 0x2A5C4, () => s.SerializeObjectArray<MapTile>(BG3_Tiles, 32 * 32, onPreSerialize: x => x.SNES_Is8PxTile = true, name: nameof(BG3_Tiles)));

            // Serialize object data
            Rayman = s.DoAt(s.CurrentPointer + 0x10016, () => s.SerializeObject<SNES_Proto_Object>(Rayman, name: nameof(Rayman)));

            // Serialize sprite tile sets
            SpriteTileSet = s.DoAt(s.CurrentPointer + 0x2afc4, () => s.SerializeArray<byte>(SpriteTileSet, 0x3000, name: nameof(SpriteTileSet)));
            SpriteTileSetAdd0 = s.DoAt(s.CurrentPointer + 0x24ea8, () => s.SerializeArray<byte>(SpriteTileSetAdd0, 0x600, name: nameof(SpriteTileSetAdd0)));
            SpriteTileSetAdd1 = s.DoAt(s.CurrentPointer + 0x24ea8 + 0x600, () => s.SerializeArray<byte>(SpriteTileSetAdd1, 0x600, name: nameof(SpriteTileSetAdd1)));
            SpriteTileSetAdd2 = s.DoAt(s.CurrentPointer + 0x24ea8 + 0xC00, () => s.SerializeArray<byte>(SpriteTileSetAdd2, 0x400, name: nameof(SpriteTileSetAdd2)));
        }
    }
}