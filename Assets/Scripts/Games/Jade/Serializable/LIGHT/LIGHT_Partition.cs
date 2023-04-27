using BinarySerializer;

namespace Ray1Map.Jade {
	public class LIGHT_Partition : Jade_File {
		public uint SizeX { get; set; }
		public uint SizeY { get; set; }
		public uint SizeZ { get; set; }
		public Jade_Vector Min { get; set; }
		public Jade_Vector Max { get; set; }
		public ushort ObjectsCount { get; set; }
		public Jade_Reference<OBJ_GameObject>[] Objects { get; set; }
		public NodeDataList[] HashTable { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			SizeX = s.Serialize<uint>(SizeX, name: nameof(SizeX));
			SizeY = s.Serialize<uint>(SizeY, name: nameof(SizeY));
			SizeZ = s.Serialize<uint>(SizeZ, name: nameof(SizeZ));
			Min = s.SerializeObject<Jade_Vector>(Min, name: nameof(Min));
			Max = s.SerializeObject<Jade_Vector>(Max, name: nameof(Max));
			ObjectsCount = s.Serialize<ushort>(ObjectsCount, name: nameof(ObjectsCount));
			Objects = s.SerializeObjectArray<Jade_Reference<OBJ_GameObject>>(Objects, ObjectsCount, name: nameof(Objects));
			HashTable = s.SerializeObjectArray<NodeDataList>(HashTable, 63, name: nameof(HashTable));
		}

		public class NodeDataList : BinarySerializable {
			public uint Count { get; set; }
			public NodeData[] Nodes { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Count = s.Serialize<uint>(Count, name: nameof(Count));
				Nodes = s.SerializeObjectArray<NodeData>(Nodes, Count, name: nameof(Nodes));
			}
		}
		public class NodeData : BinarySerializable {
			public uint X { get; set; }
			public uint Y { get; set; }
			public uint Z { get; set; }
			public uint ObjectDataCount { get; set; }
			public ObjectData[] ObjectData { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				X = s.Serialize<uint>(X, name: nameof(X));
				Y = s.Serialize<uint>(Y, name: nameof(Y));
				Z = s.Serialize<uint>(Z, name: nameof(Z));
				ObjectDataCount = s.Serialize<uint>(ObjectDataCount, name: nameof(ObjectDataCount));
				ObjectData = s.SerializeObjectArray<ObjectData>(ObjectData, ObjectDataCount, name: nameof(ObjectData));
			}
		}
		public class ObjectData : BinarySerializable {
			public ushort ObjectIndex { get; set; }
			public uint ElementsCount { get; set; }
			public ElementData[] Elements { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				ObjectIndex = s.Serialize<ushort>(ObjectIndex, name: nameof(ObjectIndex));
				ElementsCount = s.Serialize<uint>(ElementsCount, name: nameof(ElementsCount));
				Elements = s.SerializeObjectArray<ElementData>(Elements, ElementsCount, name: nameof(Elements));
			}
		}
		public class ElementData : BinarySerializable {
			public ushort TrianglesCount { get; set; }
			public ushort[] Triangles { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				TrianglesCount = s.Serialize<ushort>(TrianglesCount, name: nameof(TrianglesCount));
				Triangles = s.SerializeArray<ushort>(Triangles, TrianglesCount, name: nameof(Triangles));
			}
		}
	}
}
