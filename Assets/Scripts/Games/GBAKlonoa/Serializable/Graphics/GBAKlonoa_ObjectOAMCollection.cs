using BinarySerializer;

namespace Ray1Map.GBAKlonoa
{
    public class GBAKlonoa_ObjectOAMCollection : BinarySerializable
    {
        public byte Count { get; set; }
        public Pointer OAMsPointer { get; set; }

        // Serialized from pointers
        public GBAKlonoa_OAM[] OAMs { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Count = s.Serialize<byte>(Count, name: nameof(Count));
            s.SerializePadding(3, logIfNotNull: true);
            OAMsPointer = s.SerializePointer(OAMsPointer, name: nameof(OAMsPointer));

            s.DoAt(OAMsPointer, () => OAMs = s.SerializeObjectArray<GBAKlonoa_OAM>(OAMs, Count, name: nameof(OAMs)));
        }
    }
}