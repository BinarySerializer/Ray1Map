using System;
using BinarySerializer;

namespace R1Engine.Jade {
	// Found in GEO_p_CreateFromBuffer
	public class COL_OK3 : BinarySerializable {
		public uint BoxCount { get; set; }
		public COL_OK3_Box[] Boxes { get; set; }
		public COL_OK3_Node Tree { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			BoxCount = s.Serialize<uint>(BoxCount, name: nameof(BoxCount));
			Boxes = s.SerializeObjectArray<COL_OK3_Box>(Boxes, BoxCount, name: nameof(Boxes));
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong)) {
				Tree = s.SerializeObject<COL_OK3_Node>(Tree, name: nameof(Tree));
			}
		}

		public class COL_OK3_Box : BinarySerializable {
			public uint ElementsCount { get; set; }
			public Jade_Vector Max { get; set; }
			public Jade_Vector Min { get; set; }
			public COL_OK3_Element[] Elements { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				ElementsCount = s.Serialize<uint>(ElementsCount, name: nameof(ElementsCount));
				Max = s.SerializeObject<Jade_Vector>(Max, name: nameof(Max));
				Min = s.SerializeObject<Jade_Vector>(Min, name: nameof(Min));
				Elements = s.SerializeObjectArray<COL_OK3_Element>(Elements, ElementsCount, name: nameof(Elements));
			}

			public class COL_OK3_Element : BinarySerializable {
				public ushort Element { get; set; }
				public ushort TrianglesCount { get; set; }
				public short[] Triangles { get; set; }

				public override void SerializeImpl(SerializerObject s) {
					Element = s.Serialize<ushort>(Element, name: nameof(Element));
					TrianglesCount = s.Serialize<ushort>(TrianglesCount, name: nameof(TrianglesCount));
					Triangles = s.SerializeArray<short>(Triangles, TrianglesCount, name: nameof(Triangles));
				}
			}
		}
	}
}
