using System;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBARRR
{
    public class GAX2_SampleDefinition : BinarySerializable {
        public short Pitch { get; set; }
        public byte Byte_02 { get; set; }
        public byte Byte_03 { get; set; }
        public int Int_04 { get; set; }
        public uint LoopStart { get; set; }
        public uint LoopEnd { get; set; }
        public int Int_10 { get; set; }
        public ushort UShort_14 { get; set; }
        public ushort UShort_16 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Pitch = s.Serialize<short>(Pitch, name: nameof(Pitch));
			Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
			Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
			Int_04 = s.Serialize<int>(Int_04, name: nameof(Int_04));
			LoopStart = s.Serialize<uint>(LoopStart, name: nameof(LoopStart));
			LoopEnd = s.Serialize<uint>(LoopEnd, name: nameof(LoopEnd));
			Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
			UShort_14 = s.Serialize<ushort>(UShort_14, name: nameof(UShort_14));
			UShort_16 = s.Serialize<ushort>(UShort_16, name: nameof(UShort_16));
		}
    }
}