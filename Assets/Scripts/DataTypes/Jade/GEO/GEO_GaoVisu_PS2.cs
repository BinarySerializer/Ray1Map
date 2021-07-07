using System;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GEO_GaoVisu_PS2 : BinarySerializable {
		public uint ElementsCount { get; set; }
		public Element[] Elements { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			ElementsCount = s.Serialize<uint>(ElementsCount, name: nameof(ElementsCount));
			Elements = s.SerializeObjectArray<Element>(Elements, ElementsCount, name: nameof(Elements));
		}

		public class Element : BinarySerializable {
			public Jade_Code Foceface { get; set; }
			public uint Count { get; set; }
			public MeshElementRef[] Refs { get; set; }
			public Jade_Code Deadbeef { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Foceface = s.Serialize<Jade_Code>(Foceface, name: nameof(Foceface));
				Count = s.Serialize<uint>(Count, name: nameof(Count));
				Refs = s.SerializeObjectArray<MeshElementRef>(Refs, Count, name: nameof(Refs));
				Deadbeef = s.Serialize<Jade_Code>(Deadbeef, name: nameof(Deadbeef));
			}
		}

		public class MeshElementRef : BinarySerializable {
			public uint Struct0Count { get; set; }
			public Struct0[] Struct0Array { get; set; }
			public uint Struct1Count { get; set; }
			public Struct1[] Struct1Array { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Struct0Count = s.Serialize<uint>(Struct0Count, name: nameof(Struct0Count));
				Struct0Array = s.SerializeObjectArray<Struct0>(Struct0Array, Struct0Count, name: nameof(Struct0Array));
				Struct1Count = s.Serialize<uint>(Struct1Count, name: nameof(Struct1Count));
				Struct1Array = s.SerializeObjectArray<Struct1>(Struct1Array, Struct1Count, name: nameof(Struct1Array));
			}

			public class Struct0 : BinarySerializable {
				public uint UInt_00 { get; set; }
				public uint DataSize { get; set; }
				public byte[] Bytes { get; set; }
				public override void SerializeImpl(SerializerObject s) {
					UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
					DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));
					Bytes = s.SerializeArray<byte>(Bytes, DataSize, name: nameof(Bytes));
				}
			}

			public class Struct1 : BinarySerializable {
				public uint UInt_00 { get; set; }
				public uint DataSize { get; set; }
				public byte[] Bytes { get; set; }
				public override void SerializeImpl(SerializerObject s) {
					UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
					DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));
					Bytes = s.SerializeArray<byte>(Bytes, DataSize, name: nameof(Bytes));
				}
			}
		}
	}
}
