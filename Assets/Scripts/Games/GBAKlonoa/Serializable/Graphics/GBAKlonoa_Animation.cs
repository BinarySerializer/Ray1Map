using System.Linq;
using BinarySerializer;

namespace Ray1Map.GBAKlonoa
{
    public class GBAKlonoa_Animation : BinarySerializable
    {
        public ushort Pre_ImgDataLength { get; set; }
        public bool Pre_IsReferencedInLevel { get; set; }
        public bool Pre_IsMapAnimation { get; set; }

        public GBAKlonoa_AnimationFrame[] SerializedFrames { get; set; }
        public GBAKlonoa_AnimationFrame[] Frames { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            SerializedFrames = s.SerializeObjectArrayUntil(SerializedFrames, x => x.ImgDataPointer == null, onPreSerialize: x =>
            {
                x.Pre_ImgDataLength = Pre_ImgDataLength;
                x.Pre_IsReferencedInLevel = Pre_IsReferencedInLevel;
                x.Pre_IsMapAnimation = Pre_IsMapAnimation;
            }, name: nameof(SerializedFrames));

            Frames = SerializedFrames.Take(SerializedFrames.Length - 1).ToArray();
        }
    }
}