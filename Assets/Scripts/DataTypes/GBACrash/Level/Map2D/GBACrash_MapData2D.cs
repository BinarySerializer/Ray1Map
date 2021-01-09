namespace R1Engine
{
    public class GBACrash_MapData2D : R1Serializable
    {
        public Pointer[] MapLayersPointers { get; set; }
        public Pointer CollisionDataPointer { get; set; }
        public Pointer DataBlockPointer { get; set; } // Contains the data for the level, which is referenced by offsets - this will be a nightmare to parse :(
        public bool IsDataBlockFormatted { get; set; }
        public Pointer ObjDataPointer { get; set; }
        public Pointer ObjDataUnkTablePointer { get; set; }

        // Serialized from pointers
        public GBACrash_MapLayer[] MapLayers { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            MapLayersPointers = s.SerializePointerArray(MapLayersPointers, 4, name: nameof(MapLayersPointers));
            CollisionDataPointer = s.SerializePointer(CollisionDataPointer, name: nameof(CollisionDataPointer));
            DataBlockPointer = s.SerializePointer(DataBlockPointer, name: nameof(DataBlockPointer));
            IsDataBlockFormatted = s.Serialize<bool>(IsDataBlockFormatted, name: nameof(IsDataBlockFormatted));
            s.SerializeArray<byte>(new byte[3], 3, name: "Padding");
            ObjDataPointer = s.SerializePointer(ObjDataPointer, name: nameof(ObjDataPointer));
            ObjDataUnkTablePointer = s.SerializePointer(ObjDataUnkTablePointer, name: nameof(ObjDataUnkTablePointer));
            s.SerializeArray<byte>(new byte[12], 12, name: "Padding"); // Always 0, but still part of the same struct

            if (MapLayers == null)
                MapLayers = new GBACrash_MapLayer[MapLayersPointers.Length];

            for (int i = 0; i < MapLayers.Length; i++)
                MapLayers[i] = s.DoAt(MapLayersPointers[i], () => s.SerializeObject<GBACrash_MapLayer>(MapLayers[i], name: $"{nameof(MapLayers)}[{i}]"));
        }
    }
}