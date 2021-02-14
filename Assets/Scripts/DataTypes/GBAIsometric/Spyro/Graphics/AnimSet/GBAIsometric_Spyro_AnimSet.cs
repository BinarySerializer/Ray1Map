using System.Collections.Generic;
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

            if (AnimGroupsPointer == null)
                return;

            TileSet = TileSetIndex.DoAtBlock(size => s.SerializeArray<byte>(TileSet, size, name: nameof(TileSet)));
            AnimBlock = AnimBlockIndex.DoAtBlock(size =>
            {
                var startOffset = s.CurrentPointer;
                var anim = s.SerializeObject<GBAIsometric_Spyro_AnimationBlock>(AnimBlock, name: nameof(AnimBlock));
                s.Goto(startOffset + size); // Go to end to avoid block reading warning
                return anim;
            });
            AnimFrameImages = FrameImagesIndex.DoAtBlock(size => s.SerializeObjectArray<GBAIsometric_Spyro_AnimFrameImage>(AnimFrameImages, AnimBlock.Animations.Max(a => a.Frames.Max(f => f.FrameImageIndex)) + 1, name: nameof(AnimFrameImages)));

            if (AnimGroups == null && AnimGroupsPointer != null && AnimBlock?.Animations.Length > 0) {
                s.DoAt(AnimGroupsPointer, () => {
                    List<GBAIsometric_Spyro_AnimGroup> groups = new List<GBAIsometric_Spyro_AnimGroup>();
                    int numAnims = AnimBlock.Animations.Length;
                    GBAIsometric_Spyro_AnimGroup curGroup = null;
                    while (curGroup == null || curGroup.AnimIndex + curGroup.AnimCount < numAnims) {
                        curGroup = s.SerializeObject<GBAIsometric_Spyro_AnimGroup>(default, name: $"{nameof(AnimGroups)}[{groups.Count}]");
                        if (curGroup.AnimIndex + curGroup.AnimCount <= numAnims) {
                            groups.Add(curGroup);
                        }
                    }
                    AnimGroups = groups.ToArray();

                });
            } else {
                AnimGroups = s.DoAt(AnimGroupsPointer, () => s.SerializeObjectArray<GBAIsometric_Spyro_AnimGroup>(AnimGroups, AnimGroups.Length, name: nameof(AnimGroups)));
            }
        }
    }
}