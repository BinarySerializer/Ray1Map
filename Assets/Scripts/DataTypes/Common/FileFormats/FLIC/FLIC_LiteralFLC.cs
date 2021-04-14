using BinarySerializer;

namespace R1Engine
{
    public class FLIC_LiteralFLC : BinarySerializable
    {
        public FLIC Flic { get; set; } // Set before serializing

        public byte[] ImgData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ImgData = s.SerializeArray<byte>(ImgData, Flic.Width * Flic.Height, name: nameof(ImgData));
        }
    }
}