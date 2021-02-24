namespace R1Engine
{
    public class GBAVV_WorldMap_Data : R1Serializable
    {
        public bool SerializeData { get; set; } = true; // Set before serializing

        public Pointer TilePalettePointer { get; set; }
        public Pointer[] MapLayerPointers { get; set; }
        public Pointer MapCollisionPointer { get; set; }
        public Pointer ObjDataPointer { get; set; }
        public Pointer TileSetsPointer { get; set; }

        // Serialized from pointers

        public RGBA5551Color[] TilePalette { get; set; }
        public GBAVV_WorldMap_MapLayer[] MapLayers { get; set; }
        public GBAVV_Map2D_ObjData ObjData { get; set; }
        public GBAVV_WorldMap_TileSets TileSets { get; set; }
        public GBAVV_Fusion_MapCollisionSector Fusion_Collision { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TilePalettePointer = s.SerializePointer(TilePalettePointer, name: nameof(TilePalettePointer));
            MapLayerPointers = s.SerializePointerArray(MapLayerPointers, 4, name: nameof(MapLayerPointers));
            MapCollisionPointer = s.SerializePointer(MapCollisionPointer, name: nameof(MapCollisionPointer));
            ObjDataPointer = s.SerializePointer(ObjDataPointer, name: nameof(ObjDataPointer));
            TileSetsPointer = s.SerializePointer(TileSetsPointer, name: nameof(TileSetsPointer));

            if (!SerializeData)
                return;

            TilePalette = s.DoAt(TilePalettePointer, () => s.SerializeObjectArray<RGBA5551Color>(TilePalette, 256, name: nameof(TilePalette)));

            if (MapLayers == null)
                MapLayers = new GBAVV_WorldMap_MapLayer[MapLayerPointers.Length];

            for (int i = 0; i < MapLayers.Length; i++)
                MapLayers[i] = s.DoAt(MapLayerPointers[i], () => s.SerializeObject<GBAVV_WorldMap_MapLayer>(MapLayers[i], name: $"{nameof(MapLayers)}[{i}]"));

            ObjData = s.DoAt(ObjDataPointer, () => s.SerializeObject<GBAVV_Map2D_ObjData>(ObjData, name: nameof(ObjData)));
            TileSets = s.DoAt(TileSetsPointer, () => s.SerializeObject<GBAVV_WorldMap_TileSets>(TileSets, name: nameof(TileSets)));

            if (s.GameSettings.GBAVV_IsFusion) // In Crash 2 the collision is a compressed tilemap, but it's empty so we ignore it
                Fusion_Collision = s.DoAt(MapCollisionPointer, () => s.SerializeObject<GBAVV_Fusion_MapCollisionSector>(Fusion_Collision, name: nameof(Fusion_Collision)));
        }
    }
}