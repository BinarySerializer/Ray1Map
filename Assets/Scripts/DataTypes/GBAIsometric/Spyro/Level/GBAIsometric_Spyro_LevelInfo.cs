namespace R1Engine
{
    public class GBAIsometric_Spyro_LevelInfo : R1Serializable
    {
        public bool Is2D { get; set; }
        public bool SerializeData { get; set; } = true;

        public Pointer<GBAIsometric_Spyro_MapLayer>[] MapLayers { get; set; }

        public GBAIsometric_Spyro_DataBlockIndex TilePaletteIndex { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex CollisionIndex { get; set; } // 2D map of 4 byte structs
        public GBAIsometric_Spyro_DataBlockIndex ObjPaletteIndex { get; set; } // 
        public GBAIsometric_Spyro_DataBlockIndex Index3 { get; set; } // 2D map of 2 byte structs
        
        public uint ID { get; set; } // Levels are referenced by ID instead of index

        // Parsed
        public ARGB1555Color[] TilePalette { get; set; }
        public ARGB1555Color[] ObjPalette { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            MapLayers = s.SerializePointerArray<GBAIsometric_Spyro_MapLayer>(MapLayers, 4, resolve: SerializeData, name: nameof(MapLayers));

            if (Is2D)
            {
                if (s.GameSettings.EngineVersion == EngineVersion.GBAIsometric_Spyro2)
                    Index3 = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(Index3, x => x.HasPadding = true, name: nameof(Index3));

                TilePaletteIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(TilePaletteIndex, x => x.HasPadding = true, name: nameof(TilePaletteIndex));
                ObjPaletteIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(ObjPaletteIndex, x => x.HasPadding = true, name: nameof(ObjPaletteIndex));

                if (s.GameSettings.EngineVersion == EngineVersion.GBAIsometric_Spyro3)
                {
                    Index3 = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(Index3, x => x.HasPadding = true, name: nameof(Index3));
                    ID = s.Serialize<uint>(ID, name: nameof(ID));
                }
            }
            else
            {
                TilePaletteIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(TilePaletteIndex, x => x.HasPadding = true, name: nameof(TilePaletteIndex));
                CollisionIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(CollisionIndex, x => x.HasPadding = true, name: nameof(CollisionIndex));
                ObjPaletteIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(ObjPaletteIndex, x => x.HasPadding = true, name: nameof(ObjPaletteIndex));
                Index3 = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(Index3, x => x.HasPadding = true, name: nameof(Index3));
                ID = s.Serialize<uint>(ID, name: nameof(ID));
            }

            if (SerializeData)
            {
                TilePalette = TilePaletteIndex.DoAtBlock(size => s.SerializeObjectArray<ARGB1555Color>(TilePalette, 256, name: nameof(TilePalette)));
                ObjPalette = ObjPaletteIndex.DoAtBlock(size => s.SerializeObjectArray<ARGB1555Color>(ObjPalette, 256, name: nameof(ObjPalette)));
            }
        }
    }
}