using System;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GAX2_Instrument : R1Serializable {
        public byte Byte_01 { get; set; }
        public byte Sample { get; set; }
        public ushort UShort_02 { get; set; }
        public uint UInt_04 { get; set; }
        public uint UInt_08 { get; set; }
        public Pointer Pointer_0C { get; set; }
        public uint UInt_10 { get; set; }
        public Pointer Pointer_14 { get; set; }
        public short Short_18 { get; set; }
        public short Short_1A { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            Sample = s.Serialize<byte>(Sample, name: nameof(Sample));
            UShort_02 = s.Serialize<ushort>(UShort_02, name: nameof(UShort_02));
            UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
            UInt_08 = s.Serialize<uint>(UInt_08, name: nameof(UInt_08));
            Pointer_0C = s.SerializePointer(Pointer_0C, name: nameof(Pointer_0C));
            UInt_10 = s.Serialize<uint>(UInt_10, name: nameof(UInt_10));
            Pointer_14 = s.SerializePointer(Pointer_14, name: nameof(Pointer_14));
            Short_18 = s.Serialize<short>(Short_18, name: nameof(Short_18));
            Short_1A = s.Serialize<short>(Short_1A, name: nameof(Short_1A));
        }
    }
}