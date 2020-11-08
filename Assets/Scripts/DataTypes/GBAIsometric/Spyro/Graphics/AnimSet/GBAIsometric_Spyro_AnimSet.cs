namespace R1Engine
{
    public class GBAIsometric_Spyro_AnimSet : R1Serializable
    {
        public Pointer AnimDescriptorsPointer { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex TileSetIndex { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex AnimBlockIndex { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex PatternsIndex { get; set; }

        // Parsed
        public byte[] TileSet { get; set; }
        public GBAIsometric_Spyro_AnimDescriptor[] AnimDescriptors { get; set; }
        public GBAIsometric_Spyro_AnimationBlock AnimBlock { get; set; }
        public GBAIsometric_AnimPattern[] Patterns { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            AnimDescriptorsPointer = s.SerializePointer(AnimDescriptorsPointer, name: nameof(AnimDescriptorsPointer));
            TileSetIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(TileSetIndex, name: nameof(TileSetIndex));
            AnimBlockIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(AnimBlockIndex, name: nameof(AnimBlockIndex));
            PatternsIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(PatternsIndex, name: nameof(PatternsIndex));
            s.Serialize<ushort>(default, name: "Padding");

            TileSet = TileSetIndex.DoAtBlock(size => s.SerializeArray<byte>(TileSet, size, name: nameof(TileSet)));
            AnimBlock = AnimBlockIndex.DoAtBlock(size => s.SerializeObject<GBAIsometric_Spyro_AnimationBlock>(AnimBlock, name: nameof(AnimBlock)));
            Patterns = PatternsIndex.DoAtBlock(size => s.SerializeObjectArray<GBAIsometric_AnimPattern>(Patterns, size / 16, name: nameof(Patterns)));

            // TODO: Get correct length
            AnimDescriptors = s.DoAt(AnimDescriptorsPointer, () => s.SerializeObjectArray<GBAIsometric_Spyro_AnimDescriptor>(AnimDescriptors, AnimBlock.Animations.Length, name: nameof(AnimDescriptors)));
        }
    }
}