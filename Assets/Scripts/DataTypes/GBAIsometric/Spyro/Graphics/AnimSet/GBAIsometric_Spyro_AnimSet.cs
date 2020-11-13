using System.Linq;

namespace R1Engine
{
    public class GBAIsometric_Spyro_AnimSet : R1Serializable
    {
        public Pointer AnimGroupsPointer { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex TileSetIndex { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex AnimBlockIndex { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex FrameImagesIndex { get; set; }

        // Parsed
        public byte[] TileSet { get; set; }
        public GBAIsometric_Spyro_AnimGroup[] AnimGroups { get; set; }
        public GBAIsometric_Spyro_AnimationBlock AnimBlock { get; set; }
        public GBAIsometric_Spyro_AnimFrameImage[] AnimFrameImages { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            AnimGroupsPointer = s.SerializePointer(AnimGroupsPointer, name: nameof(AnimGroupsPointer));
            TileSetIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(TileSetIndex, name: nameof(TileSetIndex));
            AnimBlockIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(AnimBlockIndex, name: nameof(AnimBlockIndex));
            FrameImagesIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(FrameImagesIndex, name: nameof(FrameImagesIndex));
            s.Serialize<ushort>(default, name: "Padding");

            TileSet = TileSetIndex.DoAtBlock(size => s.SerializeArray<byte>(TileSet, size, name: nameof(TileSet)));
            AnimBlock = AnimBlockIndex.DoAtBlock(size => s.SerializeObject<GBAIsometric_Spyro_AnimationBlock>(AnimBlock, name: nameof(AnimBlock)));
            AnimFrameImages = FrameImagesIndex.DoAtBlock(size => s.SerializeObjectArray<GBAIsometric_Spyro_AnimFrameImage>(AnimFrameImages, AnimBlock.Animations.Max(a => a.Frames.Max(f => f.FrameImageIndex)) + 1, name: nameof(AnimFrameImages)));

            // TODO: Get correct length by reading until group.Index + group.Count >= animBlock.anims.Count
            AnimGroups = s.DoAt(AnimGroupsPointer, () => s.SerializeObjectArray<GBAIsometric_Spyro_AnimGroup>(AnimGroups, AnimBlock.Animations.Length, name: nameof(AnimGroups)));
        }
    }
}