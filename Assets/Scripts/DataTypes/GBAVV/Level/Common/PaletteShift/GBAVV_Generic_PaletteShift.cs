using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    public class GBAVV_Generic_PaletteShift : BinarySerializable
    {
        public uint PalettePointer { get; set; } // Memory pointer, always 0x05000000
        public Pointer ColorIndicesPointer { get; set; }
        public int ColorIndicesCount { get; set; }
        public int ShiftSpeed { get; set; }
        public bool IsReverse { get; set; }

        // Serialized from pointers
        public byte[] ColorIndices { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            PalettePointer = s.Serialize<uint>(PalettePointer, name: nameof(PalettePointer));

            if (PalettePointer != 0x05000000)
                Debug.LogWarning($"Palette pointer is of unexpected value: 0x{PalettePointer:X8}");

            ColorIndicesPointer = s.SerializePointer(ColorIndicesPointer, name: nameof(ColorIndicesPointer));
            ColorIndicesCount = s.Serialize<int>(ColorIndicesCount, name: nameof(ColorIndicesCount));
            ShiftSpeed = s.Serialize<int>(ShiftSpeed, name: nameof(ShiftSpeed));
            IsReverse = s.Serialize<bool>(IsReverse, name: nameof(IsReverse));
            s.SerializeArray<byte>(new byte[3], 3, name: "Padding");

            ColorIndices = s.DoAt(ColorIndicesPointer, () => s.SerializeArray<byte>(ColorIndices, ColorIndicesCount, name: nameof(ColorIndices)));
        }
    }
}