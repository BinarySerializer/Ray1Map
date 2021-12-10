using System;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBARRR
{
    public class GAX2_Instrument : BinarySerializable {
        public byte Byte_01 { get; set; }
        public byte Sample { get; set; }
        public ushort UShort_02 { get; set; }
        public uint UInt_04 { get; set; }
        public uint UInt_08 { get; set; }
        public Pointer<GAX2_InstrumentVolumeEnvelope> Envelope { get; set; }
        public byte Byte_10 { get; set; }
        public byte Byte_11 { get; set; }
        public ushort UShort_12 { get; set; }
        public Pointer<GAX2_InstrumentConfig2> InstrumentConfig { get; set; }
        public short Pitch { get; set; }
        public byte Byte_1A { get; set; }
        public byte Byte_1B { get; set; }
        public int Int_1C { get; set; }
        public uint LoopStart { get; set; }
        public uint LoopEnd { get; set; }
        public int Int_28 { get; set; }
        public int Int_2C { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            Sample = s.Serialize<byte>(Sample, name: nameof(Sample));
            UShort_02 = s.Serialize<ushort>(UShort_02, name: nameof(UShort_02));
            UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
            UInt_08 = s.Serialize<uint>(UInt_08, name: nameof(UInt_08));
            Envelope = s.SerializePointer<GAX2_InstrumentVolumeEnvelope>(Envelope, resolve: true, name: nameof(Envelope));
			Byte_10 = s.Serialize<byte>(Byte_10, name: nameof(Byte_10));
			Byte_11 = s.Serialize<byte>(Byte_11, name: nameof(Byte_11));
			UShort_12 = s.Serialize<ushort>(UShort_12, name: nameof(UShort_12));
			InstrumentConfig = s.SerializePointer<GAX2_InstrumentConfig2>(InstrumentConfig, resolve: true, name: nameof(InstrumentConfig));
            Pitch = s.Serialize<short>(Pitch, name: nameof(Pitch));
			Byte_1A = s.Serialize<byte>(Byte_1A, name: nameof(Byte_1A));
			Byte_1B = s.Serialize<byte>(Byte_1B, name: nameof(Byte_1B));
			Int_1C = s.Serialize<int>(Int_1C, name: nameof(Int_1C));
			LoopStart = s.Serialize<uint>(LoopStart, name: nameof(LoopStart));
			LoopEnd = s.Serialize<uint>(LoopEnd, name: nameof(LoopEnd));
			Int_28 = s.Serialize<int>(Int_28, name: nameof(Int_28));
			Int_2C = s.Serialize<int>(Int_2C, name: nameof(Int_2C));
		}
    }
}