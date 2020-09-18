namespace R1Engine
{
    public class R1_TypeZDC : R1Serializable
    {
        public byte ZDCIndex { get; set; }
        public byte ZDCCount { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ushort value = 0;

            value = (ushort)BitHelpers.SetBits(value, ZDCIndex, 11, 0);
            value = (ushort)BitHelpers.SetBits(value, ZDCCount, 5, 11);

            value = s.Serialize<ushort>(value, name: nameof(value));

            ZDCIndex = (byte)BitHelpers.ExtractBits(value, 11, 0);
            ZDCCount = (byte)BitHelpers.ExtractBits(value, 5, 11);

            s.Log($"{nameof(ZDCIndex)}: {ZDCIndex}");
            s.Log($"{nameof(ZDCCount)}: {ZDCCount}");
        }
    }
}