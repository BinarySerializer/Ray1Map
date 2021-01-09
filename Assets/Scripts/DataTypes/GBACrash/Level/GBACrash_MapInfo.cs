namespace R1Engine
{
    public class GBACrash_MapInfo : R1Serializable
    {
        public Pointer TilePalette2DPointer { get; set; }
        public Pointer MapData2DPointer { get; set; }
        public GBACrash_MapType MapType { get; set; }
        public uint Uint_0C { get; set; } // Set to 1 for certain bonus maps
        public ushort Index3D { get; set; } // For Mode7 and isometric levels
        public byte Byte_12 { get; set; }
        public byte Byte_13 { get; set; }

        // Serialized from pointers

        public RGBA5551Color[] TilePalette2D { get; set; }
        public GBACrash_MapData2D MapData2D { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TilePalette2DPointer = s.SerializePointer(TilePalette2DPointer, name: nameof(TilePalette2DPointer));
            MapData2DPointer = s.SerializePointer(MapData2DPointer, name: nameof(MapData2DPointer));
            MapType = s.Serialize<GBACrash_MapType>(MapType, name: nameof(MapType));
            Uint_0C = s.Serialize<uint>(Uint_0C, name: nameof(Uint_0C));
            Index3D = s.Serialize<ushort>(Index3D, name: nameof(Index3D));
            Byte_12 = s.Serialize<byte>(Byte_12, name: nameof(Byte_12));
            Byte_13 = s.Serialize<byte>(Byte_13, name: nameof(Byte_13));

            TilePalette2D = s.DoAt(TilePalette2DPointer, () => s.SerializeObjectArray<RGBA5551Color>(TilePalette2D, 256, name: nameof(TilePalette2D)));
            MapData2D = s.DoAt(MapData2DPointer, () => s.SerializeObject<GBACrash_MapData2D>(MapData2D, name: nameof(MapData2D)));
        }

        public enum GBACrash_MapType : int
        {
            // 2D
            Normal = 0,
            FlyingCarpet = 1, // Only used for the Evil Crunch fight
            Helicopter = 2, // Only used for the Evil Coco fight

            // 3D
            Mode7 = 3, // The water and space levels
            Isometric = 4, // The atlasphere levels
        }
    }
}