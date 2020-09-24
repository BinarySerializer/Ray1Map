namespace R1Engine
{
    public class R1_ZDCEntry : R1Serializable
    {
        public ushort ZDCIndex { get; set; }
        public byte ZDCCount { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.SerializeBitValues<ushort>(bitFunc =>
            {
                ZDCIndex = (ushort)bitFunc(ZDCIndex, 11, name: nameof(ZDCIndex));
                ZDCCount = (byte)bitFunc(ZDCCount, 5, name: nameof(ZDCCount));
            });
        }
    }
}