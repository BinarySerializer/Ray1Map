namespace R1Engine
{
    public class R1_ZDCData : R1Serializable
    {
        public short XPosition { get; set; }
        public short YPosition { get; set; }
        public byte Width { get; set; }
        public byte Height { get; set; }
        public byte Byte_06 { get; set; }
        public byte LayerIndex { get; set; }
        
        public override void SerializeImpl(SerializerObject s)
        {
            XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
            Width = s.Serialize<byte>(Width, name: nameof(Width));
            Height = s.Serialize<byte>(Height, name: nameof(Height));
            Byte_06 = s.Serialize<byte>(Byte_06, name: nameof(Byte_06));
            LayerIndex = s.Serialize<byte>(LayerIndex, name: nameof(LayerIndex));            
        }
    }
}