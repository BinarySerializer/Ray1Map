using System;
using System.Linq;
using BinarySerializer;

namespace Ray1Map.GBAKlonoa
{
    public class GBAKlonoa_DCT_GraphicsData : BinarySerializable
    {
        public bool Pre_IsReferencedInLevel { get; set; }

        public GraphicsFlags1 Flags1 { get; set; }
        public GraphicsFlags2 Flags2 { get; set; }
        public ushort ImgDataLength { get; set; }
        public uint ImgDataPointerValue { get; set; }
        public Pointer ImgDataPointer { get; set; }
        public Pointer PalettePointer { get; set; }
        public Pointer AnimationsPointer { get; set; }
        public Pointer OAMsPointer { get; set; }
        public byte OAMsCount { get; set; }

        // Serialized from pointers
        public byte[] ImgData { get; set; }
        public GBAKlonoa_ObjPal Palette { get; set; }
        public Pointer[] AnimationPointers { get; set; }
        public GBAKlonoa_Animation[] Animations { get; set; }
        public GBAKlonoa_OAM[] OAMs { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Flags1 = s.Serialize<GraphicsFlags1>(Flags1, name: nameof(Flags1));
            Flags2 = s.Serialize<GraphicsFlags2>(Flags2, name: nameof(Flags2));
            ImgDataLength = s.Serialize<ushort>(ImgDataLength, name: nameof(ImgDataLength));
            ImgDataPointerValue = s.Serialize<uint>(ImgDataPointerValue, name: nameof(ImgDataPointerValue));

            if (Flags1.HasFlag(GraphicsFlags1.IsCompressed))
            {
                if (Pre_IsReferencedInLevel)
                    ImgDataPointer = new Pointer(ImgDataPointerValue, Offset.Context.GetFile(GBAKlonoa_BaseManager.CompressedObjTileBlockName));
            }
            else
            {
                ImgDataPointer = new Pointer(ImgDataPointerValue, Offset.File);
            }

            s.Log("{0}: {1}", nameof(ImgDataPointer), ImgDataPointer);

            PalettePointer = s.SerializePointer(PalettePointer, name: nameof(PalettePointer));
            AnimationsPointer = s.SerializePointer(AnimationsPointer, name: nameof(AnimationsPointer));
            OAMsPointer = s.SerializePointer(OAMsPointer, name: nameof(OAMsPointer));
            OAMsCount = s.Serialize<byte>(OAMsCount, name: nameof(OAMsCount));
            s.SerializePadding(3, logIfNotNull: true);

            s.DoAt(ImgDataPointer, () => ImgData = s.SerializeArray(ImgData, ImgDataLength, name: nameof(ImgData)));
            s.DoAt(PalettePointer, () => Palette = s.SerializeObject<GBAKlonoa_ObjPal>(Palette, name: nameof(Palette)));

            if (Flags2.HasFlag(GraphicsFlags2.HasAnimations))
            {
                var animCount = s.GetR1Settings().GetGameManagerOfType<GBAKlonoa_BaseManager>().AnimSetInfos.FirstOrDefault(x => x.Offset == AnimationsPointer.AbsoluteOffset)?.AnimCount ?? throw new Exception($"Anim count not specified for anim set at {AnimationsPointer}");

                s.DoAt(AnimationsPointer, () => AnimationPointers = s.SerializePointerArray(AnimationPointers, animCount, allowInvalid: true, name: nameof(AnimationPointers)));

                Animations ??= new GBAKlonoa_Animation[AnimationPointers.Length];

                for (int i = 0; i < Animations.Length; i++)
                    s.DoAt(AnimationPointers[i], () => Animations[i] = s.SerializeObject<GBAKlonoa_Animation>(Animations[i], x =>
                    {
                        x.Pre_ImgDataLength = ImgDataLength;
                        x.Pre_IsReferencedInLevel = Pre_IsReferencedInLevel;
                    }, name: $"{nameof(Animations)}[{i}]"));
            }

            s.DoAt(OAMsPointer, () => OAMs = s.SerializeObjectArray<GBAKlonoa_OAM>(OAMs, OAMsCount, name: nameof(OAMs)));
        }

        [Flags]
        public enum GraphicsFlags1 : byte
        {
            None = 0,

            // If not set then the palette for the animation will not be allocated
            AllocatePalette = 1 << 0,

            // If this is not set then the animations and palettes are ignored
            HasData = 1 << 1,

            // Some animations should only get allocated to VRAM once since every object which uses it will play it the same
            // unlike for example enemies which all need their own animation to be allocated.
            OnlyCreateOnce = 1 << 2,

            // Specifies that only the palette for the animations should be allocated. This is used for animations which are
            // used for fixed objects.
            OnlyAllocatePalette = 1 << 3,

            // Specifies that the object tiles pointer is a relative index to the compressed data level data block.
            IsCompressed = 1 << 4,
        }

        [Flags]
        public enum GraphicsFlags2 : byte
        {
            None = 0,
            HasAnimations = 1 << 1,
        }
    }
}