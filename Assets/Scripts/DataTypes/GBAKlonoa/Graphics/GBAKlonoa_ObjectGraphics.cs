using System;
using System.Linq;
using BinarySerializer;

namespace R1Engine
{
    public class GBAKlonoa_ObjectGraphics : BinarySerializable
    {
        public uint AnimationsPointerValue { get; set; }
        public Pointer AnimationsPointer { get; set; }
        public uint VRAMPointer { get; set; }
        public ushort ImgDataLength { get; set; }
        public byte ObjIndex { get; set; }
        public byte Byte_0B { get; set; }

        // Serialized from pointers
        public Pointer[] AnimationPointers { get; set; }
        public GBAKlonoa_Animation[] Animations { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            AnimationsPointerValue = s.Serialize<uint>(AnimationsPointerValue, name: nameof(AnimationsPointerValue));

            if (AnimationsPointerValue != 0 && AnimationsPointerValue != 1)
                AnimationsPointer = new Pointer(AnimationsPointerValue, Offset.File);

            VRAMPointer = s.Serialize<uint>(VRAMPointer, name: nameof(VRAMPointer));
            ImgDataLength = s.Serialize<ushort>(ImgDataLength, name: nameof(ImgDataLength));
            ObjIndex = s.Serialize<byte>(ObjIndex, name: nameof(ObjIndex));
            Byte_0B = s.Serialize<byte>(Byte_0B, name: nameof(Byte_0B));

            if (AnimationsPointer == null)
                return;

            var animCount = s.GetR1Settings().GetGameManagerOfType<GBAKlonoa_EmpireOfDreams_Manager>().AnimSetInfos.FirstOrDefault(x => x.Offset == AnimationsPointer.AbsoluteOffset)?.AnimCount ?? throw new Exception($"Anim count not specified for anim set at {AnimationsPointer}");

            s.DoAt(AnimationsPointer, () => AnimationPointers = s.SerializePointerArray(AnimationPointers, animCount, allowInvalid: true, name: nameof(AnimationPointers)));

            Animations ??= new GBAKlonoa_Animation[AnimationPointers.Length];

            for (int i = 0; i < Animations.Length; i++)
                s.DoAt(AnimationPointers[i], () => Animations[i] = s.SerializeObject<GBAKlonoa_Animation>(Animations[i], x => x.Pre_ImgDataLength = ImgDataLength, name: $"{nameof(Animations)}[{i}]"));
        }
    }
}