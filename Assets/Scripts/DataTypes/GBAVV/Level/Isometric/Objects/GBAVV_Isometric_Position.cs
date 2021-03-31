using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_Isometric_Position : BinarySerializable
    {
        public FixedPointInt XPos { get; set; }
        public FixedPointInt YPos { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            XPos = s.SerializeObject<FixedPointInt>(XPos, name: nameof(XPos));
            YPos = s.SerializeObject<FixedPointInt>(YPos, name: nameof(YPos));
        }
    }
}