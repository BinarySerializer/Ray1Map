namespace R1Engine
{
    public class PC_BigMap : R1Serializable
    {
        public Pointer MapTilesPointer { get; set; }

        public Pointer MapTileTexturesPointersPointer { get; set; }

        public Pointer UnkPointer { get; set; }
        
        public Pointer TileTexturesPointer { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            MapTilesPointer = s.SerializePointer(MapTilesPointer, name: nameof(MapTilesPointer));
            MapTileTexturesPointersPointer = s.SerializePointer(MapTileTexturesPointersPointer, name: nameof(MapTileTexturesPointersPointer));
            UnkPointer = s.SerializePointer(UnkPointer, name: nameof(UnkPointer));
            TileTexturesPointer = s.SerializePointer(TileTexturesPointer, name: nameof(TileTexturesPointer));
        }
    }
}