using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_Isometric_Position : BinarySerializable
    {
        public FixedPointInt32 XPos { get; set; }
        public FixedPointInt32 YPos { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            XPos = s.SerializeObject<FixedPointInt32>(XPos, name: nameof(XPos));
            YPos = s.SerializeObject<FixedPointInt32>(YPos, name: nameof(YPos));
        }
    }
}