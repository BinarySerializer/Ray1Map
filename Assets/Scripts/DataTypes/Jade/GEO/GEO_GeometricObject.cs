using System;
using BinarySerializer;

namespace R1Engine.Jade {
	// Found in GEO_p_CreateFromBuffer
	public class GEO_GeometricObject : GRO_GraphicRenderObject {
		public uint Code_00 { get; set; }
		public uint Type { get; set; }
		public uint VerticesCount { get; set; }
		public int HasMRM { get; set; }
		public int HasShortPerVertex { get; set; }
		public uint UInt_14_Count { get; set; }
		public uint UVsCount { get; set; }
		public uint ElementsCount { get; set; }
		public uint UInt_Editor { get; set; }
		public uint UInt_2C { get; set; }

		public uint Code_01 { get; set; }
		public GEO_GeometricObject_SKN SkinData { get; set; }
		public GEO_GeometricObject_CollisionData CollisionData { get; set; }
		// (other stuff here)
		public Jade_Vector[] Vertices { get; set; }
		public Jade_Vector[] Normals { get; set; }
		public uint[] UInts_14 { get; set; }
		public UV[] UVs { get; set; }
		public GEO_GeometricObjectElement[] Elements { get; set; }
		public uint ElementsFlags { get; set; }

		public GEO_GeometricObject_MRM MRM { get; set; }

		public uint Last_Count { get; set; }
		public uint Type2_EndCode { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			Code_00 = s.Serialize<uint>(Code_00, name: nameof(Code_00));
			if (Code_00 == (uint)Jade_Code.Code2002) {
				Type = s.Serialize<uint>(Type, name: nameof(Type));
				VerticesCount = s.Serialize<uint>(VerticesCount, name: nameof(VerticesCount));
				HasMRM = s.Serialize<int>(HasMRM, name: nameof(HasMRM));
				if(HasMRM != 0) HasShortPerVertex = s.Serialize<int>(HasShortPerVertex, name: nameof(HasShortPerVertex));
			} else {
				VerticesCount = Code_00;
				Type = 0;
			}
			UInt_14_Count = s.Serialize<uint>(UInt_14_Count, name: nameof(UInt_14_Count));
			UVsCount = s.Serialize<uint>(UVsCount, name: nameof(UVsCount));
			ElementsCount = s.Serialize<uint>(ElementsCount, name: nameof(ElementsCount));
			if(!Loader.IsBinaryData) UInt_Editor = s.Serialize<uint>(UInt_Editor, name: nameof(UInt_Editor));
			UInt_2C = s.Serialize<uint>(UInt_2C, name: nameof(UInt_2C));
			Code_01 = s.Serialize<uint>(Code_01, name: nameof(Code_01));
			if ((Code_01 & (uint)Jade_Code.Code2002) == (uint)Jade_Code.Code2002) {
				SkinData = s.SerializeObject<GEO_GeometricObject_SKN>(SkinData, name: nameof(SkinData));
			}
			if ((Code_01 & 1) != 0) {
				CollisionData = s.SerializeObject<GEO_GeometricObject_CollisionData>(CollisionData, name: nameof(CollisionData));
			}
			Vertices = s.SerializeObjectArray<Jade_Vector>(Vertices, VerticesCount, name: nameof(Vertices));
			if (!Loader.IsBinaryData || s.GetR1Settings().Jade_Version == Jade_Version.Xenon) Normals = s.SerializeObjectArray<Jade_Vector>(Normals, VerticesCount, name: nameof(Normals));
			if (UInt_2C != 0) {
				throw new NotImplementedException($"TODO: Implement {GetType()}: UInt_2C");
			}
			UInts_14 = s.SerializeArray<uint>(UInts_14, UInt_14_Count, name: nameof(UInts_14));
			UVs = s.SerializeObjectArray<UV>(UVs, UVsCount, name: nameof(UVs));

			// Serialize elements
			Elements = s.SerializeObjectArray<GEO_GeometricObjectElement>(Elements, ElementsCount, name: nameof(Elements));
			foreach (var el in Elements) {
				el.SerializeArrays(s);
			}
			ElementsFlags = s.Serialize<uint>(ElementsFlags, name: nameof(ElementsFlags));
			if ((ElementsFlags & 1) != 0) {
				foreach (var el in Elements) {
					el.Serialize_Part2(s);
				}
			}

			// Serialize MRM
			if (HasMRM != 0) {
				MRM = s.SerializeObject<GEO_GeometricObject_MRM>(MRM, onPreSerialize: m => {
					m.Type = Type;
					m.VerticesCount = VerticesCount;
					m.HasShortPerVertex = HasShortPerVertex != 0;
				}, name: nameof(MRM));
			}

			Last_Count = s.Serialize<uint>(Last_Count, name: nameof(Last_Count));
			if (Last_Count != 0 && BitHelpers.ExtractBits((int)Last_Count, 24, 8) == 0) {
				throw new NotImplementedException($"TODO: Implement {GetType()}: Last");
			}
			if(Type >= 2) Type2_EndCode = s.Serialize<uint>(Type2_EndCode, name: nameof(Type2_EndCode));
		}

		public class UV : BinarySerializable {
			public float U { get; set; }
			public float V { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				U = s.Serialize<float>(U, name: nameof(U));
				V = s.Serialize<float>(V, name: nameof(V));
			}
		}
	}
}
