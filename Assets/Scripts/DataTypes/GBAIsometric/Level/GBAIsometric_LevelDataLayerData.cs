using System.IO;

namespace R1Engine
{
    public class GBAIsometric_LevelDataLayerData : R1Serializable
    {
        public Pointer<GBAIsometric_LevelDataLayerDataPointers> PointersPointer { get; set; }
        public ushort Ushort_04 { get; set; } // Always 1
        public ushort Ushort_06 { get; set; } // 40, 24 or 12
        public uint Uint_08 { get; set; } // 20 or 12
        public Pointer Pointer_0C { get; set; }

        // TODO: More data?

        public override void SerializeImpl(SerializerObject s)
        {
            PointersPointer = s.SerializePointer<GBAIsometric_LevelDataLayerDataPointers>(PointersPointer, resolve: true, name: nameof(PointersPointer));
            Ushort_04 = s.Serialize<ushort>(Ushort_04, name: nameof(Ushort_04));
            Ushort_06 = s.Serialize<ushort>(Ushort_06, name: nameof(Ushort_06));
            Uint_08 = s.Serialize<uint>(Uint_08, name: nameof(Uint_08));
            Pointer_0C = s.SerializePointer(Pointer_0C, name: nameof(Pointer_0C));
        }
    }
}