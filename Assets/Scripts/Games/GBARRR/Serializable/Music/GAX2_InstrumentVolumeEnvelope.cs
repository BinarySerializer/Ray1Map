using System;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBARRR
{
    public class GAX2_InstrumentVolumeEnvelope : BinarySerializable {
        public byte NumPointsVolume { get; set; }
        public byte NumPointsPanning { get; set; }
        public byte Byte_02 { get; set; }
        public byte Byte_03 { get; set; }
        public Point[] Points { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            NumPointsVolume = s.Serialize<byte>(NumPointsVolume, name: nameof(NumPointsVolume));
            NumPointsPanning = s.Serialize<byte>(NumPointsPanning, name: nameof(NumPointsPanning));
            Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
            Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
            Points = s.SerializeObjectArray<Point>(Points, NumPointsVolume, name: nameof(Points));
        }

		public class Point : BinarySerializable {
            public ushort X { get; set; }
            public short Short_02 { get; set; }
            public ushort Y { get; set; }
            public ushort UShort_06 { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				X = s.Serialize<ushort>(X, name: nameof(X));
                Short_02 = s.Serialize<short>(Short_02, name: nameof(Short_02));
                Y = s.Serialize<ushort>(Y, name: nameof(Y));
                UShort_06 = s.Serialize<ushort>(UShort_06, name: nameof(UShort_06));
			}
		}
	}
}