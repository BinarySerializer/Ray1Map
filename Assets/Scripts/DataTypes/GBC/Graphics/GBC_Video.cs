using BinarySerializer;

namespace R1Engine
{
    public class GBC_Video : GBC_BaseBlock {
        public byte FrameCount { get; set; }
        public byte Width { get; set; }
        public byte Height { get; set; }
        public byte Byte_03 { get; set; }
        public byte Byte_04 { get; set; }
        public byte Byte_05 { get; set; }

        public GBC_VideoFrame[] Frames { get; set; }

        public override void SerializeBlock(SerializerObject s) {
            FrameCount = s.Serialize<byte>(FrameCount, name: nameof(FrameCount));
            Width = s.Serialize<byte>(Width, name: nameof(Width));
            Height = s.Serialize<byte>(Height, name: nameof(Height));
            Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
            Byte_04 = s.Serialize<byte>(Byte_04, name: nameof(Byte_04));
            Byte_05 = s.Serialize<byte>(Byte_05, name: nameof(Byte_05));

            if (Frames == null) Frames = new GBC_VideoFrame[DependencyTable.DependenciesCount];

            for (int i = 0; i < Frames.Length; i++)
                Frames[i] = s.DoAt(DependencyTable.GetPointer(i), () => s.SerializeObject<GBC_VideoFrame>(Frames[i], name: $"{nameof(Frames)}[{i}]"));
        }
    }
}