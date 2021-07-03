using BinarySerializer;

namespace R1Engine
{
    public class GBAKlonoa_AnimationFrame : BinarySerializable
    {
        public ushort Pre_ImgDataLength { get; set; }

        public Pointer ImgDataPointer { get; set; }
        public byte Speed { get; set; }
        public byte Byte_05 { get; set; }
        public byte Byte_06 { get; set; }
        public byte Byte_07 { get; set; }

        // Serialized from pointers
        public byte[] ImgData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ImgDataPointer = s.SerializePointer(ImgDataPointer, allowInvalid: true, name: nameof(ImgDataPointer));
            Speed = s.Serialize<byte>(Speed, name: nameof(Speed));
            Byte_05 = s.Serialize<byte>(Byte_05, name: nameof(Byte_05));
            Byte_06 = s.Serialize<byte>(Byte_06, name: nameof(Byte_06));
            Byte_07 = s.Serialize<byte>(Byte_07, name: nameof(Byte_07));

            s.DoAt(ImgDataPointer, () => ImgData = s.SerializeArray<byte>(ImgData, Pre_ImgDataLength, name: nameof(ImgData)));
        }
    }
}