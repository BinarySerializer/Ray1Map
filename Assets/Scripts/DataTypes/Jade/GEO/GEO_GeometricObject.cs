using System;
using BinarySerializer;

namespace R1Engine.Jade {
	// Found in GEO_p_CreateFromBuffer
	public class GEO_GeometricObject : GRO_GraphicRenderObject {
		public uint Code_00 { get; set; }
		public uint Version { get; set; }
		public uint VerticesCount { get; set; }
		public int HasMRM { get; set; }
		public int HasReorderBuffer { get; set; }
		public uint ColorsCount { get; set; }
		public uint UVsCount { get; set; }
		public uint ElementsCount { get; set; }
		public uint UInt_Editor { get; set; }
		public uint MRM_ObjectAdditionalInfoPointer { get; set; }

		public uint Code_01 { get; set; }
		public GEO_GeometricObject_Ponderation ObjectPonderation { get; set; }
		public COL_OK3 OK3_Boxes { get; set; }
		// (other stuff here)
		public Jade_Vector[] Vertices { get; set; }
		public Jade_Vector[] Normals { get; set; }
		public Jade_Color[] Colors { get; set; }
		public UV[] UVs { get; set; }
		public GEO_GeometricObjectElement[] Elements { get; set; }
		public uint ElementsFlags { get; set; }

		public GEO_GeometricObject_MRM_Levels MRM_Levels { get; set; }

		public uint SpritesElementsCount { get; set; }
		public uint Version2_EndCode { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			Code_00 = s.Serialize<uint>(Code_00, name: nameof(Code_00));
			if (Code_00 == (uint)Jade_Code.Code2002) {
				Version = s.Serialize<uint>(Version, name: nameof(Version));
				VerticesCount = s.Serialize<uint>(VerticesCount, name: nameof(VerticesCount));
				HasMRM = s.Serialize<int>(HasMRM, name: nameof(HasMRM));
				if(HasMRM != 0) HasReorderBuffer = s.Serialize<int>(HasReorderBuffer, name: nameof(HasReorderBuffer));
			} else {
				VerticesCount = Code_00;
				Version = 0;
			}
			ColorsCount = s.Serialize<uint>(ColorsCount, name: nameof(ColorsCount));
			UVsCount = s.Serialize<uint>(UVsCount, name: nameof(UVsCount));
			ElementsCount = s.Serialize<uint>(ElementsCount, name: nameof(ElementsCount));
			if(!Loader.IsBinaryData) UInt_Editor = s.Serialize<uint>(UInt_Editor, name: nameof(UInt_Editor));
			MRM_ObjectAdditionalInfoPointer = s.Serialize<uint>(MRM_ObjectAdditionalInfoPointer, name: nameof(MRM_ObjectAdditionalInfoPointer));
			Code_01 = s.Serialize<uint>(Code_01, name: nameof(Code_01));
			if ((Code_01 & (uint)Jade_Code.Code2002) == (uint)Jade_Code.Code2002) {
				ObjectPonderation = s.SerializeObject<GEO_GeometricObject_Ponderation>(ObjectPonderation, name: nameof(ObjectPonderation));
			}
			if ((Code_01 & 1) != 0) {
				OK3_Boxes = s.SerializeObject<COL_OK3>(OK3_Boxes, name: nameof(OK3_Boxes));
			}
			Vertices = s.SerializeObjectArray<Jade_Vector>(Vertices, VerticesCount, name: nameof(Vertices));
			if (!Loader.IsBinaryData || s.GetR1Settings().EngineFlags.HasFlag(EngineFlags.Jade_Xenon)) Normals = s.SerializeObjectArray<Jade_Vector>(Normals, VerticesCount, name: nameof(Normals));
			if (MRM_ObjectAdditionalInfoPointer != 0) {
				throw new NotImplementedException($"TODO: Implement {GetType()}: MRM_ObjectAdditionalInfo");
			}
			Colors = s.SerializeObjectArray<Jade_Color>(Colors, ColorsCount, name: nameof(Colors));
			UVs = s.SerializeObjectArray<UV>(UVs, UVsCount, name: nameof(UVs));

			// Serialize elements
			Elements = s.SerializeObjectArray<GEO_GeometricObjectElement>(Elements, ElementsCount, name: nameof(Elements));
			foreach (var el in Elements) {
				el.SerializeArrays(s);
			}
			ElementsFlags = s.Serialize<uint>(ElementsFlags, name: nameof(ElementsFlags));
			if ((ElementsFlags & 1) != 0) {
				foreach (var el in Elements) {
					el.SerializeStripData(s);
				}
			}

			// Serialize MRM
			if (HasMRM != 0) {
				MRM_Levels = s.SerializeObject<GEO_GeometricObject_MRM_Levels>(MRM_Levels, onPreSerialize: m => {
					m.GeometricObject = this;
				}, name: nameof(MRM_Levels));
			}

			SpritesElementsCount = s.Serialize<uint>(SpritesElementsCount, name: nameof(SpritesElementsCount));
			if (SpritesElementsCount != 0 && BitHelpers.ExtractBits((int)SpritesElementsCount, 24, 8) == 0) {
				throw new NotImplementedException($"TODO: Implement {GetType()}: SpritesElements");
			}
			if(Version >= 2) Version2_EndCode = s.Serialize<uint>(Version2_EndCode, name: nameof(Version2_EndCode));
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
