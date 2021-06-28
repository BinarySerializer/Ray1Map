using BinarySerializer;

namespace R1Engine
{
    public class GBAKlonoa_AnimationFrame : BinarySerializable
    {
        public ushort Pre_ImgDataLength { get; set; }

        public Pointer ImgDataPointer { get; set; }
        public uint Uint_04 { get; set; } // Frame delay?

        // Serialized from pointers
        public byte[] ImgData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ImgDataPointer = s.SerializePointer(ImgDataPointer, allowInvalid: true, name: nameof(ImgDataPointer));
            Uint_04 = s.Serialize<uint>(Uint_04, name: nameof(Uint_04));

            s.DoAt(ImgDataPointer, () => ImgData = s.SerializeArray<byte>(ImgData, Pre_ImgDataLength, name: nameof(ImgData)));
        }
    }
}