using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro_AnimFrame : BinarySerializable
    {
        public ushort FrameImageIndex { get; set; }
        public int UnkBitFieldValue { get; set; }
        public sbyte XPosition { get; set; }
        public sbyte YPosition { get; set; }

        public bool HasExtraData { get; set; } // Set in onPreSerialize
        public ushort ExtraData_04 { get; set; }
        public ushort ExtraData_06 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.SerializeBitValues<ushort>(bitFunc =>
            {
                FrameImageIndex = (ushort)bitFunc(FrameImageIndex, 11, name: nameof(FrameImageIndex));
                UnkBitFieldValue = bitFunc(UnkBitFieldValue, 5, name: nameof(UnkBitFieldValue));
            });
            XPosition = s.Serialize<sbyte>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<sbyte>(YPosition, name: nameof(YPosition));

            if (HasExtraData) {
                ExtraData_04 = s.Serialize<ushort>(ExtraData_04, name: nameof(ExtraData_04));
                ExtraData_06 = s.Serialize<ushort>(ExtraData_06, name: nameof(ExtraData_06));
            }
        }
    }
}