using BinarySerializer;

namespace Ray1Map.Jade
{
    public class TEXPRO_UbiEvent : BinarySerializable {
        public uint FileSize { get; set; } // Set in onPreSerialize

        public byte TextureType { get; set; }

        public override void SerializeImpl(SerializerObject s) {
           TextureType = s.Serialize<byte>(TextureType, name: nameof(TextureType));
        }
	}
}