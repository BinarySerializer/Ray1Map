namespace R1Engine
{
    public class GBAVV_NitroKart_MapData : R1Serializable
    {
        public Pointer Mode7TileSetPointer { get; set; }
        public Pointer BackgroundTileSetPointer { get; set; }
        public Pointer TilePalettePointer { get; set; }
        public Pointer AdditionalTilePalettesPointer { get; set; }
        public int AdditionalTilePalettesCount { get; set; }
        public Pointer Mode7MapLayerPointer { get; set; }
        public Pointer[] BackgroundMapLayerPointers { get; set; }
        public Pointer BackgroundTileAnimationsPointer { get; set; }
        public Pointer Mode7TileSetIntsPointer { get; set; }
        public int Mode7TileSetLength { get; set; }
        public GBAVV_NitroKart_Objects Objects { get; set; }
        public Pointer TrackDataPointer { get; set; }
        public Pointer Pointer_40 { get; set; } // Usually leads to an empty struct

        // Serialized from pointers
        public byte[] Mode7TileSet { get; set; }
        public GBAVV_Map2D_TileSet BackgroundTileSet { get; set; }
        public RGBA5551Color[] TilePalette { get; set; }
        public Pointer[] AdditionalTilePalettePointers { get; set; }
        public RGBA5551Color[][] AdditionalTilePalettes { get; set; }
        public GBAVV_WorldMap_MapLayer Mode7MapLayer { get; set; }
        public GBAVV_NitroKart_BackgroundMapLayer[] BackgroundMapLayers { get; set; }
        public GBAVV_NitroKart_TileAnimations BackgroundTileAnimations { get; set; }
        public GBAVV_NitroKart_CollisionType[] Mode7TileSetCollision { get; set; }
        public GBAVV_NitroKart_TrackData TrackData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Mode7TileSetPointer = s.SerializePointer(Mode7TileSetPointer, name: nameof(Mode7TileSetPointer));
            BackgroundTileSetPointer = s.SerializePointer(BackgroundTileSetPointer, name: nameof(BackgroundTileSetPointer));
            TilePalettePointer = s.SerializePointer(TilePalettePointer, name: nameof(TilePalettePointer));
            AdditionalTilePalettesPointer = s.SerializePointer(AdditionalTilePalettesPointer, name: nameof(AdditionalTilePalettesPointer));
            AdditionalTilePalettesCount = s.Serialize<int>(AdditionalTilePalettesCount, name: nameof(AdditionalTilePalettesCount));
            Mode7MapLayerPointer = s.SerializePointer(Mode7MapLayerPointer, name: nameof(Mode7MapLayerPointer));
            BackgroundMapLayerPointers = s.SerializePointerArray(BackgroundMapLayerPointers, 3, name: nameof(BackgroundMapLayerPointers));
            BackgroundTileAnimationsPointer = s.SerializePointer(BackgroundTileAnimationsPointer, name: nameof(BackgroundTileAnimationsPointer));
            Mode7TileSetIntsPointer = s.SerializePointer(Mode7TileSetIntsPointer, name: nameof(Mode7TileSetIntsPointer));
            Mode7TileSetLength = s.Serialize<int>(Mode7TileSetLength, name: nameof(Mode7TileSetLength));
            Objects = s.SerializeObject<GBAVV_NitroKart_Objects>(Objects, name: nameof(Objects));
            TrackDataPointer = s.SerializePointer(TrackDataPointer, name: nameof(TrackDataPointer));
            Pointer_40 = s.SerializePointer(Pointer_40, name: nameof(Pointer_40));

            Mode7TileSet = s.DoAt(Mode7TileSetPointer, () => s.SerializeArray<byte>(Mode7TileSet, Mode7TileSetLength * 0x40, name: nameof(Mode7TileSet)));
            BackgroundTileSet = s.DoAt(BackgroundTileSetPointer, () => s.SerializeObject<GBAVV_Map2D_TileSet>(BackgroundTileSet, name: nameof(BackgroundTileSet)));

            TilePalette = s.DoAt(TilePalettePointer, () => s.SerializeObjectArray<RGBA5551Color>(TilePalette, 256, name: nameof(TilePalette)));
            AdditionalTilePalettePointers = s.DoAt(AdditionalTilePalettesPointer, () => s.SerializePointerArray(AdditionalTilePalettePointers, AdditionalTilePalettesCount, name: nameof(AdditionalTilePalettePointers)));

            if (AdditionalTilePalettes == null)
                AdditionalTilePalettes = new RGBA5551Color[AdditionalTilePalettePointers.Length][];

            for (int i = 0; i < AdditionalTilePalettes.Length; i++)
                AdditionalTilePalettes[i] = s.DoAt(AdditionalTilePalettePointers[i], () => s.SerializeObjectArray<RGBA5551Color>(AdditionalTilePalettes[i], 256, name: $"{nameof(AdditionalTilePalettes)}[{i}]"));

            Mode7MapLayer = s.DoAt(Mode7MapLayerPointer, () => s.SerializeObject<GBAVV_WorldMap_MapLayer>(Mode7MapLayer, name: nameof(Mode7MapLayer)));

            if (BackgroundMapLayers == null)
                BackgroundMapLayers = new GBAVV_NitroKart_BackgroundMapLayer[BackgroundMapLayerPointers.Length];

            for (int i = 0; i < BackgroundMapLayers.Length; i++)
                BackgroundMapLayers[i] = s.DoAt(BackgroundMapLayerPointers[i], () => s.SerializeObject<GBAVV_NitroKart_BackgroundMapLayer>(BackgroundMapLayers[i], name: $"{nameof(BackgroundMapLayers)}[{i}]"));

            BackgroundTileAnimations = s.DoAt(BackgroundTileAnimationsPointer, () => s.SerializeObject<GBAVV_NitroKart_TileAnimations>(BackgroundTileAnimations, name: nameof(BackgroundTileAnimations)));
            Mode7TileSetCollision = s.DoAt(Mode7TileSetIntsPointer, () => s.SerializeArray<GBAVV_NitroKart_CollisionType>(Mode7TileSetCollision, Mode7TileSetLength, name: nameof(Mode7TileSetCollision)));
            TrackData = s.DoAt(TrackDataPointer, () => s.SerializeObject<GBAVV_NitroKart_TrackData>(TrackData, name: nameof(TrackData)));
        }
    }
}