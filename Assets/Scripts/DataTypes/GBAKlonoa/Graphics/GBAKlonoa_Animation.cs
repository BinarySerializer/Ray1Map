using BinarySerializer;

namespace R1Engine
{
    public class GBAKlonoa_Animation : BinarySerializable
    {
        public ushort Pre_ImgDataLength { get; set; }

        public GBAKlonoa_AnimationFrame[] Frames { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Frames = s.SerializeObjectArrayUntil(Frames, x => x.ImgDataPointer == null, () => new GBAKlonoa_AnimationFrame(), x => x.Pre_ImgDataLength = Pre_ImgDataLength, name: nameof(Frames));
        }
    }
}