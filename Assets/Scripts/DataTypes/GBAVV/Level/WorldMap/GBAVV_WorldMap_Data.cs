namespace R1Engine
{
    public class GBAVV_WorldMap_Data : R1Serializable
    {
        public Pointer TilePalettePointer { get; set; }
        public Pointer[] MapLayerPointers { get; set; }
        public Pointer Pointer_14 { get; set; } // Compressed data - tilemap of size 0x80 x 0x80 - but all tiles are 0 - maybe collision?
        public Pointer ObjDataPointer { get; set; }
        public Pointer TileSetsPointer { get; set; }

        // Serialized from pointers

        public RGBA5551Color[] TilePalette { get; set; }
        public GBAVV_WorldMap_MapLayer[] MapLayers { get; set; }
        public GBAVV_Map2D_ObjData ObjData { get; set; }
        public GBAVV_WorldMap_TileSets TileSets { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TilePalettePointer = s.SerializePointer(TilePalettePointer, name: nameof(TilePalettePointer));
            MapLayerPointers = s.SerializePointerArray(MapLayerPointers, 4, name: nameof(MapLayerPointers));
            Pointer_14 = s.SerializePointer(Pointer_14, name: nameof(Pointer_14));
            ObjDataPointer = s.SerializePointer(ObjDataPointer, name: nameof(ObjDataPointer));
            TileSetsPointer = s.SerializePointer(TileSetsPointer, name: nameof(TileSetsPointer));

            TilePalette = s.DoAt(TilePalettePointer, () => s.SerializeObjectArray<RGBA5551Color>(TilePalette, 256, name: nameof(TilePalette)));

            if (MapLayers == null)
                MapLayers = new GBAVV_WorldMap_MapLayer[MapLayerPointers.Length];

            for (int i = 0; i < MapLayers.Length; i++)
                MapLayers[i] = s.DoAt(MapLayerPointers[i], () => s.SerializeObject<GBAVV_WorldMap_MapLayer>(MapLayers[i], x => x.Is8bpp = i == 3, name: $"{nameof(MapLayers)}[{i}]"));

            ObjData = s.DoAt(ObjDataPointer, () => s.SerializeObject<GBAVV_Map2D_ObjData>(ObjData, name: nameof(ObjData)));
            TileSets = s.DoAt(TileSetsPointer, () => s.SerializeObject<GBAVV_WorldMap_TileSets>(TileSets, name: nameof(TileSets)));
        }
    }
}