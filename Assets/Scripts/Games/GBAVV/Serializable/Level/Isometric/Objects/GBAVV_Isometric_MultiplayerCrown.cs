using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_Isometric_MultiplayerCrown : BinarySerializable
    {
        public int XPos { get; set; }
        public int YPos { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            XPos = s.Serialize<int>(XPos, name: nameof(XPos));
            YPos = s.Serialize<int>(YPos, name: nameof(YPos));
        }
    }
}