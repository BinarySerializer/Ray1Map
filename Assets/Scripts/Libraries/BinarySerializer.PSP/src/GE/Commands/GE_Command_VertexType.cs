namespace BinarySerializer.PSP 
{
    public class GE_Command_VertexType : GE_CommandData 
    {
        public GE_VertexNumberFormat TextureFormat { get; set; } // ST/UV values
        public GE_ColorFormat ColorFormat { get; set; }
        public GE_VertexNumberFormat NormalFormat { get; set; }
        public GE_VertexNumberFormat PositionFormat { get; set; }
        public GE_VertexNumberFormat WeightFormat { get; set; }
        public GE_IndexFormat IndexFormat { get; set; }
        public byte WeightsCount { get; set; }
        public byte VerticesCount { get; set; }
        public bool BypassTransformPipeline { get; set; } // if true, raw coordinates are used

        public override void SerializeImpl(BitSerializerObject b) {
            TextureFormat = b.SerializeBits<GE_VertexNumberFormat>(TextureFormat, 2, name: nameof(TextureFormat));
            ColorFormat = b.SerializeBits<GE_ColorFormat>(ColorFormat, 3, name: nameof(ColorFormat));
            NormalFormat = b.SerializeBits<GE_VertexNumberFormat>(NormalFormat, 2, name: nameof(NormalFormat));
            PositionFormat = b.SerializeBits<GE_VertexNumberFormat>(PositionFormat, 2, name: nameof(PositionFormat));
            WeightFormat = b.SerializeBits<GE_VertexNumberFormat>(WeightFormat, 2, name: nameof(WeightFormat));
            IndexFormat = b.SerializeBits<GE_IndexFormat>(IndexFormat, 2, name: nameof(IndexFormat));
            b.SerializePadding(1, logIfNotNull: true);
            WeightsCount = b.SerializeBits<byte>(WeightsCount, 3, name: nameof(WeightsCount));
            b.SerializePadding(1, logIfNotNull: true);
            VerticesCount = b.SerializeBits<byte>(VerticesCount, 3, name: nameof(VerticesCount));
            b.SerializePadding(2, logIfNotNull: true);
            BypassTransformPipeline = b.SerializeBits<bool>(BypassTransformPipeline, 1, name: nameof(BypassTransformPipeline));

        }
    }
}