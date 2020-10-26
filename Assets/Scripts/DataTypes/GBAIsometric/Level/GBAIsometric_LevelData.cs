namespace R1Engine
{
    public class GBAIsometric_LevelData : R1Serializable
    {
        public GBAIsometric_LevelDataLayer[] MapLayers { get; set; }

        public uint ObjectsCount { get; set; }
        public uint UnkDataCount { get; set; } // Waypoints?
        public uint Unk2 { get; set; }
        public uint Unk3 { get; set; }

        public Pointer ObjectsPointer { get; set; }
        public Pointer UnkDataPointer { get; set; }
        public Pointer Pointer2 { get; set; } // Compressed data?

        public byte Byte_6C { get; set; }
        public byte Byte_6D { get; set; }
        public byte Byte_6E { get; set; }
        public byte Byte_6F { get; set; }

        public uint LevelNameLocIndex { get; set; }

        // Parsed from pointers

        public GBAIsometric_Object[] Objects { get; set; }
        public GBAIsometric_UnkData[] UnkData { get; set; } // Waypoints?

        public override void SerializeImpl(SerializerObject s)
        {
            MapLayers = s.SerializeObjectArray<GBAIsometric_LevelDataLayer>(MapLayers, 4, name: nameof(MapLayers));

            ObjectsCount = s.Serialize<uint>(ObjectsCount, name: nameof(ObjectsCount));
            UnkDataCount = s.Serialize<uint>(UnkDataCount, name: nameof(UnkDataCount));
            Unk2 = s.Serialize<uint>(Unk2, name: nameof(Unk2));
            Unk3 = s.Serialize<uint>(Unk3, name: nameof(Unk3));

            ObjectsPointer = s.SerializePointer(ObjectsPointer, name: nameof(ObjectsPointer));
            UnkDataPointer = s.SerializePointer(UnkDataPointer, name: nameof(UnkDataPointer));
            Pointer2 = s.SerializePointer(Pointer2, name: nameof(Pointer2));

            Byte_6C = s.Serialize<byte>(Byte_6C, name: nameof(Byte_6C));
            Byte_6D = s.Serialize<byte>(Byte_6D, name: nameof(Byte_6D));
            Byte_6E = s.Serialize<byte>(Byte_6E, name: nameof(Byte_6E));
            Byte_6F = s.Serialize<byte>(Byte_6F, name: nameof(Byte_6F));

            LevelNameLocIndex = s.Serialize<uint>(LevelNameLocIndex, name: nameof(LevelNameLocIndex));

            // Parse from pointers
            Objects = s.DoAt(ObjectsPointer, () => s.SerializeObjectArray<GBAIsometric_Object>(Objects, ObjectsCount, name: nameof(Objects)));
            UnkData = s.DoAt(ObjectsPointer, () => s.SerializeObjectArray<GBAIsometric_UnkData>(UnkData, UnkDataCount, name: nameof(UnkData)));
        }
    }
}