using System;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    public class GAX2_InstrumentConfig2 : BinarySerializable {
        public sbyte RelativeNoteNumber { get; set; }
        public byte Byte_01 { get; set; }
        public byte[] Bytes { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            RelativeNoteNumber = s.Serialize<sbyte>(RelativeNoteNumber, name: nameof(RelativeNoteNumber));
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            Bytes = s.SerializeArray<byte>(Bytes, 3 * 2,name: nameof(Bytes));
        }
    }
}