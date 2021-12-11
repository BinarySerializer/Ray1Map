using System;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBARRR
{
    public class GAX2_InstrumentRow : BinarySerializable {
        public sbyte RelativeNoteNumber { get; set; }
        public bool DontUseNotePitch { get; set; } // Is this a sign?
        public byte SampleIndex { get; set; } // Starting from 1
        public byte Byte_03 { get; set; }
        public Effect[] Effects { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            RelativeNoteNumber = s.Serialize<sbyte>(RelativeNoteNumber, name: nameof(RelativeNoteNumber));
            DontUseNotePitch = s.Serialize<bool>(DontUseNotePitch, name: nameof(DontUseNotePitch));
			SampleIndex = s.Serialize<byte>(SampleIndex, name: nameof(SampleIndex));
			Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
			Effects = s.SerializeObjectArray<Effect>(Effects, 2, name: nameof(Effects));
		}

		public class Effect : BinarySerializable {
            public byte Type { get; set; }
            public byte Parameter { get; set; }
			public override void SerializeImpl(SerializerObject s) {
                s.DoBits<ushort>(b => {
                    Parameter = b.SerializeBits<byte>(Parameter, 8, name: nameof(Parameter));
                    Type = b.SerializeBits<byte>(Type, 8, name: nameof(Type));
                });
			}

            public enum EffectType : byte {
                None = 0,
                Type1 = 1,
                Type2 = 2,
                //Type3 = 3,
                //Type4 = 4,
                Type5 = 5,
                Type6 = 6,
                //Type7 = 7,
                //Type8 = 8,
                //Type9 = 9,
                Type10 = 10,
                Type11 = 11,
                Volume = 12,
                Speed = 15, // Set ticks per Instrument row
            }
		}
	}
}