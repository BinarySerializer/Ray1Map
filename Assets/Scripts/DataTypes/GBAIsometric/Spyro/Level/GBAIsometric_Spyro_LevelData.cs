namespace R1Engine
{
    public class GBAIsometric_Spyro_LevelData : R1Serializable
    {
        public bool Is2D { get; set; }
        public int SerializeDataForID { get; set; }

        public Pointer[] MapLayerPointers { get; set; }

        public GBAIsometric_Spyro_DataBlockIndex TilePaletteIndex { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex Collision2DIndex { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex Collision3DIndex { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex ObjPaletteIndex { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex Index3 { get; set; } // 2D map of 2 byte structs
        
        public uint ID { get; set; } // Levels are referenced by ID instead of index

        // Parsed
        public GBAIsometric_Spyro_MapLayer[] MapLayers { get; set; }
        public ARGB1555Color[] TilePalette { get; set; }
        public ARGB1555Color[] ObjPalette { get; set; }
        public GBAIsometric_Spyro_Collision2DMapData Collision2D { get; set; }
        public GBAIsometric_Spyro_Collision3DMapData Collision3D { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            MapLayerPointers = s.SerializePointerArray(MapLayerPointers, 4, name: nameof(MapLayerPointers));

            if (Is2D)
            {
                if (s.GameSettings.EngineVersion == EngineVersion.GBAIsometric_Spyro2)
                    Index3 = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(Index3, x => x.HasPadding = true, name: nameof(Index3));

                TilePaletteIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(TilePaletteIndex, x => x.HasPadding = true, name: nameof(TilePaletteIndex));

                if (s.GameSettings.EngineVersion == EngineVersion.GBAIsometric_Spyro3)
                    Collision2DIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(Collision2DIndex, x => x.HasPadding = true, name: nameof(Collision2DIndex));

                ObjPaletteIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(ObjPaletteIndex, x => x.HasPadding = true, name: nameof(ObjPaletteIndex));

                if (s.GameSettings.EngineVersion == EngineVersion.GBAIsometric_Spyro3)
                    ID = s.Serialize<uint>(ID, name: nameof(ID));
            }
            else
            {
                TilePaletteIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(TilePaletteIndex, x => x.HasPadding = true, name: nameof(TilePaletteIndex));
                Collision3DIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(Collision3DIndex, x => x.HasPadding = true, name: nameof(Collision3DIndex));
                ObjPaletteIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(ObjPaletteIndex, x => x.HasPadding = true, name: nameof(ObjPaletteIndex));
                Index3 = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(Index3, x => x.HasPadding = true, name: nameof(Index3));
                ID = s.Serialize<uint>(ID, name: nameof(ID));
            }

            if (SerializeDataForID == ID)
            {
                if (MapLayers == null)
                    MapLayers = new GBAIsometric_Spyro_MapLayer[MapLayerPointers.Length];

                for (int i = 0; i < MapLayers.Length; i++)
                    MapLayers[i] = s.DoAt(MapLayerPointers[i], () => s.SerializeObject<GBAIsometric_Spyro_MapLayer>(MapLayers[i], name: $"{nameof(MapLayers)}[{i}]"));

                TilePalette = TilePaletteIndex.DoAtBlock(size => s.SerializeObjectArray<ARGB1555Color>(TilePalette, 256, name: nameof(TilePalette)));
                ObjPalette = ObjPaletteIndex.DoAtBlock(size => s.SerializeObjectArray<ARGB1555Color>(ObjPalette, 256, name: nameof(ObjPalette)));
                Collision3D = Collision3DIndex?.DoAtBlock(size => s.SerializeObject<GBAIsometric_Spyro_Collision3DMapData>(Collision3D, name: nameof(Collision3D)));
                Collision2D = Collision2DIndex?.DoAtBlock(size => s.SerializeObject<GBAIsometric_Spyro_Collision2DMapData>(Collision2D, name: nameof(Collision2D)));
            }
        }
    }
}