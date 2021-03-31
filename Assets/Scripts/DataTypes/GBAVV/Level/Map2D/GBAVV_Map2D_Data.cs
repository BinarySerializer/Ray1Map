using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_Map2D_Data : BinarySerializable
    {
        public Pointer[] MapLayersPointers { get; set; }
        public Pointer CollisionDataPointer { get; set; }
        public Pointer LayersBlockPointer { get; set; }
        public bool HasDataBlockHeader { get; set; }
        public Pointer ObjDataPointer { get; set; }
        public Pointer ObjDataUnkTablePointer { get; set; }

        // Serialized from pointers
        public GBAVV_Map2D_MapLayer[] MapLayers { get; set; }
        public GBAVV_Map2D_MapLayer CollisionLayer { get; set; }
        public GBAVV_Map2D_LayersBlock LayersBlock { get; set; }
        public GBAVV_Map2D_ObjData ObjData { get; set; }
        public GBAVV_Map2D_ObjDataUnkTable ObjDataUnkTable { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            MapLayersPointers = s.SerializePointerArray(MapLayersPointers, 4, name: nameof(MapLayersPointers));
            CollisionDataPointer = s.SerializePointer(CollisionDataPointer, name: nameof(CollisionDataPointer));
            LayersBlockPointer = s.SerializePointer(LayersBlockPointer, name: nameof(LayersBlockPointer));
            HasDataBlockHeader = s.Serialize<bool>(HasDataBlockHeader, name: nameof(HasDataBlockHeader));
            s.SerializeArray<byte>(new byte[3], 3, name: "Padding");
            ObjDataPointer = s.SerializePointer(ObjDataPointer, name: nameof(ObjDataPointer));
            ObjDataUnkTablePointer = s.SerializePointer(ObjDataUnkTablePointer, name: nameof(ObjDataUnkTablePointer));
            s.SerializeArray<byte>(new byte[12], 12, name: "Padding"); // Always 0, but still part of the same struct

            if (MapLayers == null)
                MapLayers = new GBAVV_Map2D_MapLayer[MapLayersPointers.Length];

            for (int i = 0; i < MapLayers.Length; i++)
                MapLayers[i] = s.DoAt(MapLayersPointers[i], () => s.SerializeObject<GBAVV_Map2D_MapLayer>(MapLayers[i], name: $"{nameof(MapLayers)}[{i}]"));

            CollisionLayer = s.DoAt(CollisionDataPointer, () => s.SerializeObject<GBAVV_Map2D_MapLayer>(CollisionLayer, name: nameof(CollisionLayer)));

            LayersBlock = s.DoAt(LayersBlockPointer, () => s.SerializeObject<GBAVV_Map2D_LayersBlock>(LayersBlock, x =>
            {
                x.HasHeader = HasDataBlockHeader;
                x.MapData = this;
            }, name: nameof(LayersBlock)));

            ObjData = s.DoAt(ObjDataPointer, () => s.SerializeObject<GBAVV_Map2D_ObjData>(ObjData, name: nameof(ObjData)));
            ObjDataUnkTable = s.DoAt(ObjDataUnkTablePointer, () => s.SerializeObject<GBAVV_Map2D_ObjDataUnkTable>(ObjDataUnkTable, name: nameof(ObjDataUnkTable)));
        }
    }
}