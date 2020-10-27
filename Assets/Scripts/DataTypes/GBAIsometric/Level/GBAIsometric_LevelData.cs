namespace R1Engine
{
    public class GBAIsometric_LevelData : R1Serializable
    {
        public GBAIsometric_LevelDataLayer[] MapLayers { get; set; }

        public int ObjectsCount { get; set; }
        public int WaypointsCount { get; set; }
        public int Unk2 { get; set; }
        public int Unk3 { get; set; }

        public Pointer ObjectsPointer { get; set; }
        public Pointer WaypointsPointer { get; set; }
        public Pointer Pointer2 { get; set; } // Compressed data?

        public byte Byte_6C { get; set; }
        public byte Byte_6D { get; set; }
        public byte Byte_6E { get; set; }
        public byte Byte_6F { get; set; }

        public uint LevelNameLocIndex { get; set; }

        // Parsed from pointers

        public GBAIsometric_Object[] Objects { get; set; }
        public GBAIsometric_Waypoint[] Waypoints { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            MapLayers = s.SerializeObjectArray<GBAIsometric_LevelDataLayer>(MapLayers, 4, name: nameof(MapLayers));

            ObjectsCount = s.Serialize<int>(ObjectsCount, name: nameof(ObjectsCount));
            WaypointsCount = s.Serialize<int>(WaypointsCount, name: nameof(WaypointsCount));
            Unk2 = s.Serialize<int>(Unk2, name: nameof(Unk2));
            Unk3 = s.Serialize<int>(Unk3, name: nameof(Unk3));

            ObjectsPointer = s.SerializePointer(ObjectsPointer, name: nameof(ObjectsPointer));
            WaypointsPointer = s.SerializePointer(WaypointsPointer, name: nameof(WaypointsPointer));
            Pointer2 = s.SerializePointer(Pointer2, name: nameof(Pointer2));

            Byte_6C = s.Serialize<byte>(Byte_6C, name: nameof(Byte_6C));
            Byte_6D = s.Serialize<byte>(Byte_6D, name: nameof(Byte_6D));
            Byte_6E = s.Serialize<byte>(Byte_6E, name: nameof(Byte_6E));
            Byte_6F = s.Serialize<byte>(Byte_6F, name: nameof(Byte_6F));

            LevelNameLocIndex = s.Serialize<uint>(LevelNameLocIndex, name: nameof(LevelNameLocIndex));

            // Parse from pointers
            Objects = s.DoAt(ObjectsPointer, () => s.SerializeObjectArray<GBAIsometric_Object>(Objects, ObjectsCount, name: nameof(Objects)));
            Waypoints = s.DoAt(WaypointsPointer, () => s.SerializeObjectArray<GBAIsometric_Waypoint>(Waypoints, WaypointsCount, name: nameof(Waypoints)));
        }
    }
}