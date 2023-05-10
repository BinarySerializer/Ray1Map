namespace BinarySerializer.PSP 
{
    public class GE_Command_TextureSize : GE_CommandData 
    {
        public byte TW { get; set; } // Width = 2^TW
        public byte TH { get; set; } // Height = 2^TH

        public override void SerializeImpl(BitSerializerObject b) {
            TW = b.SerializeBits<byte>(TW, 8, name: nameof(TW));
            TH = b.SerializeBits<byte>(TH, 8, name: nameof(TH));
            b.SerializePadding(8, logIfNotNull: true);
        }
    }
}