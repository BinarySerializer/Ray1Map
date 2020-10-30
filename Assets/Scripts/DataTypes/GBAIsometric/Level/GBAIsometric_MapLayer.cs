namespace R1Engine
{
    public class GBAIsometric_MapLayer : R1Serializable
    {
        public Pointer<GBAIsometric_TileMapData> TileMapPointer { get; set; }
        public MapLayerType StructType { get; set; }
        public ushort Width { get; set; } // 40, 24 or 12 - 4 for maps
        public ushort Height { get; set; } // 20 or 12 - 4 for maps
        public ushort Ushort_0A { get; set; } // Always 0
        public Pointer MapDataPointer { get; set; }

        // Maps only
        public Pointer MapPalettePointer { get; set; }
        public ARGB1555Color[] MapPalette { get; set; }
        public ARGB1555Color[] UnkMapPalette { get; set; } // Maybe not part of the same struct?
        public byte[] RemainingData { get; set; }

        // Parsed
        public MapTile[] MapData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TileMapPointer = s.SerializePointer<GBAIsometric_TileMapData>(TileMapPointer, resolve: true, name: nameof(TileMapPointer));
            StructType = s.Serialize<MapLayerType>(StructType, name: nameof(StructType));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
            Ushort_0A = s.Serialize<ushort>(Ushort_0A, name: nameof(Ushort_0A));
            MapDataPointer = s.SerializePointer(MapDataPointer, name: nameof(MapDataPointer));
            MapPalettePointer = s.SerializePointer(MapPalettePointer, name: nameof(MapPalettePointer));

            if (StructType == MapLayerType.Map)
            {
                UnkMapPalette = s.SerializeObjectArray<ARGB1555Color>(UnkMapPalette, 256, name: nameof(UnkMapPalette));
                RemainingData = s.SerializeArray<byte>(RemainingData, 44, name: nameof(RemainingData));

                MapPalette = s.DoAt(MapPalettePointer, () => s.SerializeObjectArray<ARGB1555Color>(MapPalette, 256, name: nameof(MapPalette)));
            }

            s.DoAt(MapDataPointer, () =>
            {
                s.DoEncoded(new RHREncoder(), () => MapData = s.SerializeObjectArray<MapTile>(MapData, Width * Height, name: nameof(MapData)));
            });
        }

        public enum MapLayerType : ushort
        {
            Normal = 1,
            Map = 2
        }
    }
}