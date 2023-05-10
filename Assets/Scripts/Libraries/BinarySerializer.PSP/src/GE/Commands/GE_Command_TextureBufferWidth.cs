namespace BinarySerializer.PSP 
{
    public class GE_Command_TextureBufferWidth : GE_CommandData 
    {
        public ushort BufferWidth { get; set; } // in pixels
        public byte AddressMSB { get; set; } // 4 most significant bits of pointer

        public override void SerializeImpl(BitSerializerObject b) {
            BufferWidth = b.SerializeBits<ushort>(BufferWidth, 16, name: nameof(BufferWidth));
            AddressMSB = b.SerializeBits<byte>(AddressMSB, 4, name: nameof(AddressMSB));
            b.SerializePadding(4, logIfNotNull: true);
        }
    }
}