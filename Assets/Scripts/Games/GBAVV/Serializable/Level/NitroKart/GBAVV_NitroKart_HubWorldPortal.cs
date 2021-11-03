using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_NitroKart_HubWorldPortal : BinarySerializable
    {
        public int LevelID { get; set; } // The ID in the current hub
        public Pointer NamePointer { get; set; }
        public int[] Data { get; set; }

        // Serialized from pointers
        public GBAVV_LocalizedString Name { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LevelID = s.Serialize<int>(LevelID, name: nameof(LevelID));
            NamePointer = s.SerializePointer(NamePointer, name: nameof(NamePointer));
            Data = s.SerializeArray<int>(Data, 18, name: nameof(Data));

            Name = s.DoAt(NamePointer, () => s.SerializeObject<GBAVV_LocalizedString>(Name, name: nameof(Name)));
        }
    }
}