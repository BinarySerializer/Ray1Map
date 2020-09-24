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
        public ushort R2_Flags { get; set; }
        
        public override void SerializeImpl(SerializerObject s)
        {
            XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
            Width = s.Serialize<byte>(Width, name: nameof(Width));
            Height = s.Serialize<byte>(Height, name: nameof(Height));

            if (s.GameSettings.EngineVersion == EngineVersion.R2_PS1)
            {
                s.SerializeBitValues<ushort>(bitFunc =>
                {
                    LayerIndex = (byte)bitFunc(LayerIndex, 5, name: nameof(LayerIndex));
                    R2_Flags = (ushort)bitFunc(R2_Flags, 11, name: nameof(R2_Flags));
                });
            }
            else
            {
                Byte_06 = s.Serialize<byte>(Byte_06, name: nameof(Byte_06));
                LayerIndex = s.Serialize<byte>(LayerIndex, name: nameof(LayerIndex));
            }
        }
    }
}