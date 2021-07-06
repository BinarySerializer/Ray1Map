using System;
using System.Linq;
using BinarySerializer;

namespace R1Engine
{
    public class GBAKlonoa_DCT_GraphicsData : BinarySerializable
    {
        public GraphicsFlags1 Flags1 { get; set; }
        public GraphicsFlags2 Flags2 { get; set; }
        public ushort ImgDataLength { get; set; }
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
            ImgDataPointer = s.SerializePointer(ImgDataPointer, allowInvalid: true, name: nameof(ImgDataPointer));
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
                    s.DoAt(AnimationPointers[i], () => Animations[i] = s.SerializeObject<GBAKlonoa_Animation>(Animations[i], x => x.Pre_ImgDataLength = ImgDataLength, name: $"{nameof(Animations)}[{i}]"));
            }

            s.DoAt(OAMsPointer, () => OAMs = s.SerializeObjectArray<GBAKlonoa_OAM>(OAMs, OAMsCount, name: nameof(OAMs)));
        }

        [Flags]
        public enum GraphicsFlags1 : byte
        {
            None = 0,
            Flag_0 = 1 << 0,
            HasData = 1 << 1,
            Flag_2 = 1 << 2,
            Flag_3 = 1 << 3,
            IsCompressed = 1 << 4,
            Flag_5 = 1 << 5,
            Flag_6 = 1 << 6,
            Flag_7 = 1 << 7,
        }

        [Flags]
        public enum GraphicsFlags2 : byte
        {
            None = 0,
            Flag_0 = 1 << 0,
            HasAnimations = 1 << 1,
            Flag_2 = 1 << 2,
            Flag_3 = 1 << 3,
            Flag_4 = 1 << 4,
            Flag_5 = 1 << 5,
            Flag_6 = 1 << 6,
            Flag_7 = 1 << 7,
        }
    }
}