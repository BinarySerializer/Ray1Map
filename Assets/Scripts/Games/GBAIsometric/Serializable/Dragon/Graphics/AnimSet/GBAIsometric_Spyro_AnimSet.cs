﻿using System.Collections.Generic;
using System.Linq;
using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro_AnimSet : BinarySerializable
    {
        public Pointer AnimGroupsPointer { get; set; }
        public GBAIsometric_IceDragon_ResourceRef TileSetIndex { get; set; }
        public GBAIsometric_IceDragon_ResourceRef AnimBlockIndex { get; set; }
        public GBAIsometric_IceDragon_ResourceRef FrameImagesIndex { get; set; }

        // Parsed
        public byte[] TileSet { get; set; }
        public GBAIsometric_Spyro_AnimGroup[] AnimGroups { get; set; }
        public GBAIsometric_Spyro_AnimationBlock AnimBlock { get; set; }
        public GBAIsometric_Spyro_AnimFrameImage[] AnimFrameImages { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            AnimGroupsPointer = s.SerializePointer(AnimGroupsPointer, name: nameof(AnimGroupsPointer));
            TileSetIndex = s.SerializeObject<GBAIsometric_IceDragon_ResourceRef>(TileSetIndex, name: nameof(TileSetIndex));
            AnimBlockIndex = s.SerializeObject<GBAIsometric_IceDragon_ResourceRef>(AnimBlockIndex, name: nameof(AnimBlockIndex));
            FrameImagesIndex = s.SerializeObject<GBAIsometric_IceDragon_ResourceRef>(FrameImagesIndex, name: nameof(FrameImagesIndex));
            s.Serialize<ushort>(default, name: "Padding");

            if (AnimGroupsPointer == null)
                return;

            TileSetIndex.DoAt(size => TileSet = s.SerializeArray<byte>(TileSet, size, name: nameof(TileSet)));
            AnimBlockIndex.DoAt(size =>
            {
                var startOffset = s.CurrentPointer;
                AnimBlock = s.SerializeObject<GBAIsometric_Spyro_AnimationBlock>(AnimBlock, name: nameof(AnimBlock));
                s.Goto(startOffset + size); // Go to end to avoid block reading warning
            });
            FrameImagesIndex.DoAt(size => AnimFrameImages = s.SerializeObjectArray<GBAIsometric_Spyro_AnimFrameImage>(AnimFrameImages, AnimBlock.Animations.Max(a => a.Frames.Max(f => f.FrameImageIndex)) + 1, name: nameof(AnimFrameImages)));

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