namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_RLE : R1Serializable
    {
        public string Magic { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int FramesCount { get; set; }
        public Pointer[] FramePointers { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Magic = s.SerializeString(Magic, 4, name: nameof(Magic));
            Width = s.Serialize<int>(Width, name: nameof(Width));
            Height = s.Serialize<int>(Height, name: nameof(Height));
            FramesCount = s.Serialize<int>(FramesCount, name: nameof(FramesCount));
            FramePointers = s.SerializePointerArray(FramePointers, FramesCount, name: nameof(FramePointers));

            // TODO: Parse each frame

            s.Goto(Offset + s.CurrentLength);
        }
    }
}