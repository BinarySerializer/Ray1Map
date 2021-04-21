using System;
using BinarySerializer;

namespace R1Engine.Jade {
	// Found in GEO_p_CreateFromBuffer
	public class GEO_GeometricObject_CollisionData : BinarySerializable {
		public uint Count { get; set; }
		public Entry[] Entries { get; set; }
		public COL_Node Tree { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Count = s.Serialize<uint>(Count, name: nameof(Count));
			Entries = s.SerializeObjectArray<Entry>(Entries, Count, name: nameof(Entries));
			if (s.GetR1Settings().EngineVersion >= EngineVersion.Jade_KingKong) {
				Tree = s.SerializeObject<COL_Node>(Tree, name: nameof(Tree));
			}
		}

		public class Entry : BinarySerializable {
			public uint Count { get; set; }
			public Jade_Vector Vector_04 { get; set; }
			public Jade_Vector Vector_10 { get; set; }
			public Entry2[] Entries { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Count = s.Serialize<uint>(Count, name: nameof(Count));
				Vector_04 = s.SerializeObject<Jade_Vector>(Vector_04, name: nameof(Vector_04));
				Vector_10 = s.SerializeObject<Jade_Vector>(Vector_10, name: nameof(Vector_10));
				Entries = s.SerializeObjectArray<Entry2>(Entries, Count, name: nameof(Entries));
			}

			public class Entry2 : BinarySerializable {
				public ushort UShort_00 { get; set; }
				public ushort Count { get; set; }
				public short[] Shorts { get; set; }

				public override void SerializeImpl(SerializerObject s) {
					UShort_00 = s.Serialize<ushort>(UShort_00, name: nameof(UShort_00));
					Count = s.Serialize<ushort>(Count, name: nameof(Count));
					Shorts = s.SerializeArray<short>(Shorts, Count, name: nameof(Shorts));
				}
			}
		}
	}
}
