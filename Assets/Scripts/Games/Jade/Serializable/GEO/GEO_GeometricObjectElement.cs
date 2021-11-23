using System;
using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in GEO_v_CreateElementsFromBuffer
	public class GEO_GeometricObjectElement : BinarySerializable {
		public GEO_GeometricObject GeometricObject { get; set; } // Set in onPreSerialize

		public uint TrianglesCount { get; set; }
		public uint MaterialID { get; set; }
		public uint MrmElementAdditionalInfoPointer { get; set; }
		public uint UsedIndexCount { get; set; }
		public uint[] UInts_Editor { get; set; }

		// Arrays
		public Triangle[] Triangles { get; set; }
		public MRM_Element MrmElementAdditionalInfo { get; set; }
		public ushort[] ListOfUsedIndex { get; set; }

		// Part 2
		public uint StripDataFlags { get; set; }
		public uint StripDataCount { get; set; }
		public OneStrip[] Strips { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			TrianglesCount = s.Serialize<uint>(TrianglesCount, name: nameof(TrianglesCount));
			MaterialID = s.Serialize<uint>(MaterialID, name: nameof(MaterialID));
			if (GeometricObject.ObjectVersion < 2) {
				MrmElementAdditionalInfoPointer = s.Serialize<uint>(MrmElementAdditionalInfoPointer, name: nameof(MrmElementAdditionalInfoPointer));
				UsedIndexCount = s.Serialize<uint>(UsedIndexCount, name: nameof(UsedIndexCount));
			}
			if (!Loader.IsBinaryData) UInts_Editor = s.SerializeArray<uint>(UInts_Editor, 6, name: nameof(UInts_Editor));
		}
		public void SerializeArrays(SerializerObject s) {
			Triangles = s.SerializeObjectArray<Triangle>(Triangles, TrianglesCount, name: nameof(Triangles));

			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
				if (MrmElementAdditionalInfoPointer != 0) MrmElementAdditionalInfo = s.SerializeObject<MRM_Element>(MrmElementAdditionalInfo, name: nameof(MrmElementAdditionalInfo));
				ListOfUsedIndex = s.SerializeArray<ushort>(ListOfUsedIndex, UsedIndexCount, name: nameof(ListOfUsedIndex));
			}
		}
		public void SerializeStripData(SerializerObject s) {
			StripDataFlags = s.Serialize<uint>(StripDataFlags, name: nameof(StripDataFlags));
			StripDataCount = s.Serialize<uint>(StripDataCount, name: nameof(StripDataCount));
			Strips = s.SerializeObjectArray<OneStrip>(Strips, StripDataCount, name: nameof(Strips));
		}

		public class Triangle : BinarySerializable {
			public ushort Vertex0 { get; set; }
			public ushort Vertex1 { get; set; }
			public ushort Vertex2 { get; set; }
			public ushort UV0 { get; set; }
			public ushort UV1 { get; set; }
			public ushort UV2 { get; set; }
			public uint SmoothingGroup { get; set; }
			public uint UInt_10 { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

				Vertex0 = s.Serialize<ushort>(Vertex0, name: nameof(Vertex0));
				Vertex1 = s.Serialize<ushort>(Vertex1, name: nameof(Vertex1));
				Vertex2 = s.Serialize<ushort>(Vertex2, name: nameof(Vertex2));
				UV0 = s.Serialize<ushort>(UV0, name: nameof(UV0));
				UV1 = s.Serialize<ushort>(UV1, name: nameof(UV1));
				UV2 = s.Serialize<ushort>(UV2, name: nameof(UV2));
				SmoothingGroup = s.Serialize<uint>(SmoothingGroup, name: nameof(SmoothingGroup));

				if(s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier) || !Loader.IsBinaryData) {
					UInt_10 = s.Serialize<uint>(UInt_10, name: nameof(UInt_10));
				}
			}
		}

		public class MRM_Element : BinarySerializable {
			public uint One_UV_Per_Point_Per_Element_Base { get; set; }
			public uint RealNumberOfTriangle { get; set; }
			public uint Number_Of_Triangle_vs_Point_Equivalence { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				One_UV_Per_Point_Per_Element_Base = s.Serialize<uint>(One_UV_Per_Point_Per_Element_Base, name: nameof(One_UV_Per_Point_Per_Element_Base));
				RealNumberOfTriangle = s.Serialize<uint>(RealNumberOfTriangle, name: nameof(RealNumberOfTriangle));
				Number_Of_Triangle_vs_Point_Equivalence = s.Serialize<uint>(Number_Of_Triangle_vs_Point_Equivalence, name: nameof(Number_Of_Triangle_vs_Point_Equivalence));
			}
		}

		public class OneStrip : BinarySerializable {
			public int VertexCount { get; set; }
			public bool HasNoEditorInts { get; set; }
			public int[] Ints_Editor { get; set; }
			public VertexUV[] VertexUVMap { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

				s.DoBits<int>(b => {
					VertexCount = b.SerializeBits<int>(VertexCount, 31, name: nameof(VertexCount));
					HasNoEditorInts = b.SerializeBits<bool>(HasNoEditorInts, 1, name: nameof(HasNoEditorInts));
				});
				if (!HasNoEditorInts && !Loader.IsBinaryData
					&& s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier))
					Ints_Editor = s.SerializeArray<int>(Ints_Editor, VertexCount, name: nameof(Ints_Editor));
				VertexUVMap = s.SerializeObjectArray<VertexUV>(VertexUVMap, VertexCount, name: nameof(VertexUVMap));
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
