using System;
using BinarySerializer;

namespace R1Engine.Jade {
	// Found in GEO_v_CreateElementsFromBuffer
	public class GEO_GeometricObjectElement : BinarySerializable {
		public uint TrianglesCount { get; set; }
		public uint UInt_04 { get; set; }
		public uint UInt_14 { get; set; }
		public uint ShortsCount { get; set; }
		public uint[] UInts_Editor { get; set; }

		// Arrays
		public Triangle[] Triangles { get; set; }
		public UnknownStruct Unknown { get; set; }
		public ushort[] Shorts { get; set; }

		// Part 2
		public uint UInt_Part2_00 { get; set; }
		public uint UInt_Part2_Count { get; set; }
		public UnknownPart2Struct[] Part2Structs { get; set; }

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
		public void Serialize_Part2(SerializerObject s) {
			UInt_Part2_00 = s.Serialize<uint>(UInt_Part2_00, name: nameof(UInt_Part2_00));
			UInt_Part2_Count = s.Serialize<uint>(UInt_Part2_Count, name: nameof(UInt_Part2_Count));
			Part2Structs = s.SerializeObjectArray<UnknownPart2Struct>(Part2Structs, UInt_Part2_Count, name: nameof(Part2Structs));
		}

		public class Triangle : BinarySerializable {
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

		public class UnknownStruct : BinarySerializable {
			public uint UInt_00 { get; set; }
			public uint UInt_04 { get; set; }
			public uint UInt_Editor_08 { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
				UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
				UInt_Editor_08 = s.Serialize<uint>(UInt_Editor_08, name: nameof(UInt_Editor_08));
			}
		}

		public class UnknownPart2Struct : BinarySerializable {
			public int Int_00 { get; set; }
			public bool HasEditorInts { get; set; }
			public int[] Ints_Editor { get; set; }
			public VertexUV[] VertexUVMap { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

				s.SerializeBitValues<int>(bitFunc => {
					Int_00 = bitFunc(Int_00, 31, name: nameof(Int_00));
					HasEditorInts = bitFunc(HasEditorInts ? 1 : 0, 1, name: nameof(HasEditorInts)) == 1;
				});
				if (Int_00 >= 0 && !Loader.IsBinaryData) Ints_Editor = s.SerializeArray<int>(Ints_Editor, Int_00, name: nameof(Ints_Editor));
				VertexUVMap = s.SerializeObjectArray<VertexUV>(VertexUVMap, Int_00, name: nameof(VertexUVMap));
			}

			public class VertexUV : BinarySerializable {
				public ushort VertexIndex { get; set; }
				public ushort UVIndex { get; set; }
				public override void SerializeImpl(SerializerObject s) {
					VertexIndex = s.Serialize<ushort>(VertexIndex, name: nameof(VertexIndex));
					UVIndex = s.Serialize<ushort>(UVIndex, name: nameof(UVIndex));
				}
			}
		}
	}
}
