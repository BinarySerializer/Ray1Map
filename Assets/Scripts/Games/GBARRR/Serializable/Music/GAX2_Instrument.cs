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
        public byte Byte_10 { get; set; }
        public byte NumKeymapEntries { get; set; }
        public ushort UShort_12 { get; set; }
        public Pointer KeymapPointer { get; set; }
        public GAX2_InstrumentKeyMap[] Keymap { get; set; } // Actually an array
        public GAX2_SampleDefinition[] Samples { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            SampleIndices = s.SerializeArray<byte>(SampleIndices, 7, name: nameof(SampleIndices));
            UInt_08 = s.Serialize<uint>(UInt_08, name: nameof(UInt_08));
            Envelope = s.SerializePointer<GAX2_InstrumentEnvelope>(Envelope, resolve: true, name: nameof(Envelope));
			Byte_10 = s.Serialize<byte>(Byte_10, name: nameof(Byte_10));
			NumKeymapEntries = s.Serialize<byte>(NumKeymapEntries, name: nameof(NumKeymapEntries));
			UShort_12 = s.Serialize<ushort>(UShort_12, name: nameof(UShort_12));
            KeymapPointer = s.SerializePointer(KeymapPointer, name: nameof(KeymapPointer));
            s.DoAt(KeymapPointer, () => {
				Keymap = s.SerializeObjectArray<GAX2_InstrumentKeyMap>(Keymap, NumKeymapEntries, name: nameof(Keymap));
			});
            int numSamples = Math.Max(Keymap.Max(k => k.SampleIndex), (byte)1);
			Samples = s.SerializeObjectArray<GAX2_SampleDefinition>(Samples, numSamples, name: nameof(Samples));
		}
    }
}