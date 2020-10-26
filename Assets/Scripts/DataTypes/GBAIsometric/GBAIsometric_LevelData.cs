namespace R1Engine
{
    public class GBAIsometric_LevelData : R1Serializable
    {
        public GBAIsometric_LevelDataLayer BG0 { get; set; }
        public GBAIsometric_LevelDataLayer BG1 { get; set; }
        public GBAIsometric_LevelDataLayer BG2 { get; set; }
        public GBAIsometric_LevelDataLayer BG3 { get; set; }

        public uint ObjectsCount { get; set; }
        public uint Unk1 { get; set; }
        public uint Unk2 { get; set; }
        public uint Unk3 { get; set; }

        public Pointer<ObjectArray<GBAIsometric_Object>> ObjectsPointer { get; set; }
        public Pointer Pointer1 { get; set; } // Structs of size 0x0C
        public Pointer Pointer2 { get; set; } // Compressed data?

        // TODO: There appears to be more data here

        public override void SerializeImpl(SerializerObject s)
        {
            BG0 = s.SerializeObject<GBAIsometric_LevelDataLayer>(BG0, name: nameof(BG0));
            BG1 = s.SerializeObject<GBAIsometric_LevelDataLayer>(BG1, name: nameof(BG1));
            BG2 = s.SerializeObject<GBAIsometric_LevelDataLayer>(BG2, name: nameof(BG2));
            BG3 = s.SerializeObject<GBAIsometric_LevelDataLayer>(BG3, name: nameof(BG3));

            ObjectsCount = s.Serialize<uint>(ObjectsCount, name: nameof(ObjectsCount));
            Unk1 = s.Serialize<uint>(Unk1, name: nameof(Unk1));
            Unk2 = s.Serialize<uint>(Unk2, name: nameof(Unk2));
            Unk3 = s.Serialize<uint>(Unk3, name: nameof(Unk3));

            ObjectsPointer = s.SerializePointer<ObjectArray<GBAIsometric_Object>>(ObjectsPointer, resolve: true, onPreSerialize: x => x.Length = ObjectsCount, name: nameof(ObjectsPointer));
            Pointer1 = s.SerializePointer(Pointer1, name: nameof(Pointer1));
            Pointer2 = s.SerializePointer(Pointer2, name: nameof(Pointer2));
        }
    }
}