using System;
using System.Linq;
using BinarySerializer;

namespace R1Engine
{
    public class GBAKlonoa_ObjectGraphics : BinarySerializable
    {
        public GBAKlonoa_LevelObjectCollection Pre_LevelObjects { get; set; }

        public uint AnimationsPointerValue { get; set; }
        public Pointer AnimationsPointer { get; set; }
        public uint VRAMPointer { get; set; }
        public ushort ImgDataLength { get; set; }
        public byte ObjIndex { get; set; }

        // Serialized from pointers
        public Pointer[] AnimationPointers { get; set; }
        public GBAKlonoa_Animation[] Animations { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            AnimationsPointerValue = s.Serialize<uint>(AnimationsPointerValue, name: nameof(AnimationsPointerValue));

            VRAMPointer = s.Serialize<uint>(VRAMPointer, name: nameof(VRAMPointer));
            ImgDataLength = s.Serialize<ushort>(ImgDataLength, name: nameof(ImgDataLength));
            ObjIndex = s.Serialize<byte>(ObjIndex, name: nameof(ObjIndex));
            s.SerializePadding(1, logIfNotNull: true);

            if (AnimationsPointerValue != 0 && AnimationsPointerValue != 1)
            {
                var value = AnimationsPointerValue;

                // Hack for waterfall due to the set having animations with different shapes
                if (Pre_LevelObjects != null && 
                    (Pre_LevelObjects.Objects.ElementAtOrDefault(ObjIndex - GBAKlonoa_EmpireOfDreams_Manager.FixCount)?.ObjType == 57 ||
                    Pre_LevelObjects.Objects.ElementAtOrDefault(ObjIndex - GBAKlonoa_EmpireOfDreams_Manager.FixCount)?.ObjType == 58))
                    value += 4;

                AnimationsPointer = new Pointer(value, Offset.File);
            }

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