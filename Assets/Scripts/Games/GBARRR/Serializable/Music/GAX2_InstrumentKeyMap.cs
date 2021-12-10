using System;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBARRR
{
    public class GAX2_InstrumentKeyMap : BinarySerializable {
        public sbyte RelativeNoteNumber { get; set; }
        public byte RelativeNoteNumberSign { get; set; } // Is this a sign?
        public byte SampleIndex { get; set; } // Starting from 1
        public byte Byte_03 { get; set; }
        public byte Byte_04 { get; set; } // Next 4 bytes are used as 2 ushorts in GAX2_ProcessInstrumentConfig, with the top byte of each specifying some effect
        public byte Byte_05 { get; set; }
        public byte Byte_06 { get; set; }
        public byte Byte_07 { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            RelativeNoteNumber = s.Serialize<sbyte>(RelativeNoteNumber, name: nameof(RelativeNoteNumber));
            RelativeNoteNumberSign = s.Serialize<byte>(RelativeNoteNumberSign, name: nameof(RelativeNoteNumberSign));
			SampleIndex = s.Serialize<byte>(SampleIndex, name: nameof(SampleIndex));
			Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
			Byte_04 = s.Serialize<byte>(Byte_04, name: nameof(Byte_04));
			Byte_05 = s.Serialize<byte>(Byte_05, name: nameof(Byte_05));
			Byte_06 = s.Serialize<byte>(Byte_06, name: nameof(Byte_06));
			Byte_07 = s.Serialize<byte>(Byte_07, name: nameof(Byte_07));
        }
    }
}