using System;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBARRR
{
    public class GAX2_InstrumentEnvelope : BinarySerializable {
        public byte NumPoints { get; set; }
        public byte? Sustain { get; set; }
        public byte? LoopStart { get; set; }
        public byte? LoopEnd { get; set; }
        public Point[] Points { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            NumPoints = s.Serialize<byte>(NumPoints, name: nameof(NumPoints));
            Sustain = s.Serialize<byte?>(Sustain, name: nameof(Sustain));
            LoopStart = s.Serialize<byte?>(LoopStart, name: nameof(LoopStart));
            LoopEnd = s.Serialize<byte?>(LoopEnd, name: nameof(LoopEnd));
            Points = s.SerializeObjectArray<Point>(Points, NumPoints, name: nameof(Points));
        }

		public class Point : BinarySerializable {
            public ushort X { get; set; } // In ticks
            public short Interpolation { get; set; } // CurrentY = (Point1.Interpolation * (currentX - Point0.X)) >> 8) + (uint)(Point0.Y & 0xff)
            public byte Y { get; set; }
            public byte Byte_05 { get; set; }
            public ushort UShort_06 { get; set; } // Maybe padding

			public override void SerializeImpl(SerializerObject s) {
				X = s.Serialize<ushort>(X, name: nameof(X));
                Interpolation = s.Serialize<short>(Interpolation, name: nameof(Interpolation));
                Y = s.Serialize<byte>(Y, name: nameof(Y));
				Byte_05 = s.Serialize<byte>(Byte_05, name: nameof(Byte_05));
				UShort_06 = s.Serialize<ushort>(UShort_06, name: nameof(UShort_06));
			}
		}
	}
}