using System;

namespace R1Engine.Jade {
	// Found in GEO_v_CreateElementsFromBuffer
	public class GEO_GeometricObjectElement : R1Serializable {
		public uint TrianglesCount { get; set; }
		public uint UInt_04 { get; set; }
		public uint UInt_14 { get; set; }
		public uint ShortsCount { get; set; }
		public uint[] UInts_Editor { get; set; }

		// Arrays
		public Triangle[] Triangles { get; set; }
		public UnknownStruct Unknown { get; set; }
		public ushort[] Shorts { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			TrianglesCount = s.Serialize<uint>(TrianglesCount, name: nameof(TrianglesCount));
			UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
			UInt_14 = s.Serialize<uint>(UInt_14, name: nameof(UInt_14));
			ShortsCount = s.Serialize<uint>(ShortsCount, name: nameof(ShortsCount));
			if (!Loader.IsBinaryData) UInts_Editor = s.SerializeArray<uint>(UInts_Editor, 6, name: nameof(UInts_Editor));
		}
		public void SerializeArrays(SerializerObject s) {
			Triangles = s.SerializeObjectArray<Triangle>(Triangles, TrianglesCount, name: nameof(Triangles));
			if(UInt_14 != 0) Unknown = s.SerializeObject<UnknownStruct>(Unknown, name: nameof(Unknown));
			Shorts = s.SerializeArray<ushort>(Shorts, ShortsCount, name: nameof(Shorts));
		}

		public class Triangle : R1Serializable {
			public ushort Vertex0 { get; set; }
			public ushort Vertex1 { get; set; }
			public ushort Vertex2 { get; set; }
			public ushort UV0 { get; set; }
			public ushort UV1 { get; set; }
			public ushort UV2 { get; set; }
			public uint UInt_0C { get; set; }
			public uint UInt_10 { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Vertex0 = s.Serialize<ushort>(Vertex0, name: nameof(Vertex0));
				Vertex1 = s.Serialize<ushort>(Vertex1, name: nameof(Vertex1));
				Vertex2 = s.Serialize<ushort>(Vertex2, name: nameof(Vertex2));
				UV0 = s.Serialize<ushort>(UV0, name: nameof(UV0));
				UV1 = s.Serialize<ushort>(UV1, name: nameof(UV1));
				UV2 = s.Serialize<ushort>(UV2, name: nameof(UV2));
				UInt_0C = s.Serialize<uint>(UInt_0C, name: nameof(UInt_0C));
				UInt_10 = s.Serialize<uint>(UInt_10, name: nameof(UInt_10));
			}
		}

		public class UnknownStruct : R1Serializable {
			public uint UInt_00 { get; set; }
			public uint UInt_04 { get; set; }
			public uint UInt_Editor_08 { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
				UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
				UInt_Editor_08 = s.Serialize<uint>(UInt_Editor_08, name: nameof(UInt_Editor_08));
			}
		}
	}
}
