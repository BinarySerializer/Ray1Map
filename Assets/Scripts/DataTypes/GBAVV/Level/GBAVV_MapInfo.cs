namespace R1Engine
{
    public class GBAVV_MapInfo : R1Serializable
    {
        public Pointer TilePalette2DPointer { get; set; }
        public Pointer MapData2DPointer { get; set; }
        public GBAVV_MapType MapType { get; set; }
        public uint Uint_0C { get; set; } // Set to 1 for certain bonus maps
        public short Index3D { get; set; } // For Mode7 and isometric levels
        public byte Alpha_BG3 { get; set; }
        public byte Alpha_BG2 { get; set; } // Might not be BG2

        // Serialized from pointers

        public RGBA5551Color[] TilePalette2D { get; set; }
        public GBAVV_Map2D_Data MapData2D { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TilePalette2DPointer = s.SerializePointer(TilePalette2DPointer, name: nameof(TilePalette2DPointer));
            MapData2DPointer = s.SerializePointer(MapData2DPointer, name: nameof(MapData2DPointer));
            MapType = s.Serialize<GBAVV_MapType>(MapType, name: nameof(MapType));
            Uint_0C = s.Serialize<uint>(Uint_0C, name: nameof(Uint_0C));
            Index3D = s.Serialize<short>(Index3D, name: nameof(Index3D));
            Alpha_BG3 = s.Serialize<byte>(Alpha_BG3, name: nameof(Alpha_BG3));
            Alpha_BG2 = s.Serialize<byte>(Alpha_BG2, name: nameof(Alpha_BG2));

            TilePalette2D = s.DoAt(TilePalette2DPointer, () => s.SerializeObjectArray<RGBA5551Color>(TilePalette2D, 256, name: nameof(TilePalette2D)));
            MapData2D = s.DoAt(MapData2DPointer, () => s.SerializeObject<GBAVV_Map2D_Data>(MapData2D, name: nameof(MapData2D)));
        }

        public enum GBAVV_MapType : int
        {
            // 2D
            Normal = 0,
            Normal_Vehicle_0 = 1, // Underwater in Crash 1, flying carpet in Crash 2
            Normal_Vehicle_1 = 2, // Motorcycle in Crash 1, copter in Crash 2

            // 3D
            Mode7 = 3, // The water and space levels
            Isometric = 4, // The atlasphere levels

            WorldMap = 0xFF // Not actually a part of the value, but we add it here as it uses a separate format
        }
    }
}