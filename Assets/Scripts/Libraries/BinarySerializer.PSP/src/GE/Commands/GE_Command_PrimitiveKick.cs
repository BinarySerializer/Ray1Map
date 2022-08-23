namespace BinarySerializer.PSP 
{
    public class GE_Command_PrimitiveKick : GE_CommandData 
    {
        public ushort VerticesCount { get; set; }
        public GE_PrimitiveType PrimitiveType { get; set; }

        public override void SerializeImpl(BitSerializerObject b) {
            VerticesCount = b.SerializeBits<ushort>(VerticesCount, 16, name: nameof(VerticesCount));
            PrimitiveType = b.SerializeBits<GE_PrimitiveType>(PrimitiveType, 3, name: nameof(PrimitiveType));
            b.SerializePadding(5, logIfNotNull: true);
        }
    }
}