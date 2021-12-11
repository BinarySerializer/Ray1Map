using System;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBARRR
{
    public class GAX2_Instrument : BinarySerializable {
        public byte Byte_01 { get; set; }
        public byte[] SampleIndices { get; set; }
        public uint UInt_08 { get; set; }
        public Pointer<GAX2_InstrumentEnvelope> Envelope { get; set; }
        public byte RowSpeed { get; set; } // After RowSpeed frames, it switches to the next row
        public byte NumRows { get; set; }
        public ushort UShort_12 { get; set; }
        public Pointer RowsPointer { get; set; }
        public GAX2_InstrumentRow[] Keymap { get; set; } // Actually an array
        public GAX2_SampleDefinition[] Samples { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            SampleIndices = s.SerializeArray<byte>(SampleIndices, 7, name: nameof(SampleIndices));
            UInt_08 = s.Serialize<uint>(UInt_08, name: nameof(UInt_08));
            Envelope = s.SerializePointer<GAX2_InstrumentEnvelope>(Envelope, resolve: true, name: nameof(Envelope));
			RowSpeed = s.Serialize<byte>(RowSpeed, name: nameof(RowSpeed));
			NumRows = s.Serialize<byte>(NumRows, name: nameof(NumRows));
			UShort_12 = s.Serialize<ushort>(UShort_12, name: nameof(UShort_12));
            RowsPointer = s.SerializePointer(RowsPointer, name: nameof(RowsPointer));
            s.DoAt(RowsPointer, () => {
				Keymap = s.SerializeObjectArray<GAX2_InstrumentRow>(Keymap, NumRows, name: nameof(Keymap));
			});
            int numSamples = Math.Max(Keymap.Max(k => k.SampleIndex), (byte)1);
			Samples = s.SerializeObjectArray<GAX2_SampleDefinition>(Samples, numSamples, name: nameof(Samples));
		}
    }
}