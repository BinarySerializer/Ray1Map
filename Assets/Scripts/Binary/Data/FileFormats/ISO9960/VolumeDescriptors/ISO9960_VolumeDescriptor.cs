using BinarySerializer;

namespace Ray1Map
{
    // See: https://wiki.osdev.org/ISO_9660
    public abstract class ISO9960_VolumeDescriptor : BinarySerializable
    {
        public byte TypeCode { get; set; }
        public string StandardIdentifier { get; set; }
        public byte Version { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            TypeCode = s.Serialize<byte>(TypeCode, name: nameof(TypeCode));
            StandardIdentifier = s.SerializeString(StandardIdentifier, length: 5, name: nameof(StandardIdentifier));
            Version = s.Serialize<byte>(Version, name: nameof(Version));
        }
    }
}