using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in GEO_LoadGCGeoObject
	public class GEO_GeoObject_GC_Content : Jade_File {
		public GEO_GeoObject_GC Header { get; set; }
		public GEO_GeometricObject GeometricObject { get; set; }

		public uint ElementsCount => Header.ElementsCount;

		public Jade_Code DeadBabe { get; set; }
		public uint UInt_Unk { get; set; }
		public Jade_Vector[] Vectors0 { get; set; }
		public Jade_Vector[] Vectors1 { get; set; }
		public GEO_GeometricObject.GEO_ObjFlags? Flags { get; set; }
		public GEO_GeometricObject.GEO_ObjFlags_PoPSoT? Flags_SoT { get; set; }
		public bool UseNormals => 
			Flags?.HasFlag(GEO_GeometricObject.GEO_ObjFlags.UseNormalsInEngine) 
			?? Flags_SoT?.HasFlag(GEO_GeometricObject.GEO_ObjFlags_PoPSoT.UseNormalsInEngine) 
			?? false;
		public bool HasLightmap =>
			Flags?.HasFlag(GEO_GeometricObject.GEO_ObjFlags.HasLightMap)
			?? Flags_SoT?.HasFlag(GEO_GeometricObject.GEO_ObjFlags_PoPSoT.HasLightMap)
			?? false;
		public bool Index8Bits =>
			Flags?.HasFlag(GEO_GeometricObject.GEO_ObjFlags.Index8Bits)
			?? Flags_SoT?.HasFlag(GEO_GeometricObject.GEO_ObjFlags_PoPSoT.Index8Bits)
			?? false;
		public bool RenderAOOnWii =>
			Flags?.HasFlag(GEO_GeometricObject.GEO_ObjFlags.NCIS_RenderAOOnWii_TFS_CanBeDisplaced)
			?? Flags_SoT?.HasFlag(GEO_GeometricObject.GEO_ObjFlags_PoPSoT.NCIS_RenderAOOnWii_TFS_CanBeDisplaced)
			?? false;
		public Element_DisplayList[] Elements { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			DeadBabe = s.Serialize<Jade_Code>(DeadBabe, name: nameof(DeadBabe));
			if (DeadBabe != Jade_Code.DEADBABE)
				throw new BinarySerializableException(this, $"Expected code {Jade_Code.DEADBABE} but got {DeadBabe}");
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_CloudyWithAChanceOfMeatballs)) {
				UInt_Unk = s.Serialize<uint>(UInt_Unk, name: nameof(UInt_Unk));
				if (UInt_Unk != 0) {
					Vectors0 = s.SerializeObjectArray<Jade_Vector>(Vectors0, GeometricObject.VerticesCount, name: nameof(Vectors0));
					Vectors1 = s.SerializeObjectArray<Jade_Vector>(Vectors1, GeometricObject.VerticesCount, name: nameof(Vectors1));
				}
			}
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_WW_20040920)) {
				Flags = s.Serialize<GEO_GeometricObject.GEO_ObjFlags>(Flags ?? 0, name: nameof(Flags));
			} else {
				Flags_SoT = s.Serialize<GEO_GeometricObject.GEO_ObjFlags_PoPSoT>(Flags_SoT ?? 0, name: nameof(Flags_SoT));
			}
			Elements = s.SerializeObjectArray<Element_DisplayList>(Elements, ElementsCount, onPreSerialize: el => el.GCObject = this, name: nameof(Elements));
		}

		public class Element_DisplayList : BinarySerializable {
			public GEO_GeoObject_GC_Content GCObject { get; set; }

			public ushort StripsCount { get; set; }
			public ushort UShort_02 { get; set; }
			public DisplayList_Strip[] Strips { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				StripsCount = s.Serialize<ushort>(StripsCount, name: nameof(StripsCount));
				UShort_02 = s.Serialize<ushort>(UShort_02, name: nameof(UShort_02));
				Strips = s.SerializeObjectArray<DisplayList_Strip>(Strips, StripsCount, onPreSerialize: str => str.DL = this, name: nameof(Strips));
			}
		}

		public class DisplayList_Strip : BinarySerializable {
			public Element_DisplayList DL { get; set; }

			public ushort Length { get; set; }
			public DisplayList_Point[] Points { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Length = s.Serialize<ushort>(Length, name: nameof(Length));
				Points = s.SerializeObjectArray<DisplayList_Point>(Points, Length, onPreSerialize: dlp => dlp.Strip = this, name: nameof(Points));
			}
		}


		public class DisplayList_Point : BinarySerializable {
			public DisplayList_Strip Strip { get; set; }

			public ushort Index { get; set; }
			public ushort Normal { get; set; }
			public ushort Col { get; set; }
			public ushort Tex { get; set; }
			public ushort LM1 { get; set; }
			public ushort LM2 { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				if (Strip.DL.GCObject.Index8Bits) {
					Index = s.Serialize<byte>((byte)Index, name: nameof(Index));
				} else {
					Index = s.Serialize<ushort>(Index, name: nameof(Index));
				}
				if (Strip.DL.GCObject.UseNormals) {
					if (Strip.DL.GCObject.Index8Bits) {
						Normal = s.Serialize<byte>((byte)Normal, name: nameof(Normal));
					} else {
						Normal = s.Serialize<ushort>(Normal, name: nameof(Normal));
					}
				}
				if (Strip.DL.GCObject.Index8Bits) {
					Col = s.Serialize<byte>((byte)Col, name: nameof(Col));
				} else {
					Col = s.Serialize<ushort>(Col, name: nameof(Col));
				}
				if (Strip.DL.GCObject.Index8Bits) {
					Tex = s.Serialize<byte>((byte)Tex, name: nameof(Tex));
				} else {
					Tex = s.Serialize<ushort>(Tex, name: nameof(Tex));
				}
				if (Strip.DL.GCObject.HasLightmap
					|| (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_CloudyWithAChanceOfMeatballs)
					&& Strip.DL.GCObject.RenderAOOnWii)) {
					LM1 = s.Serialize<ushort>(LM1, name: nameof(LM1));
					LM2 = s.Serialize<ushort>(LM2, name: nameof(LM2));
				}
			}
		}
	}
}
