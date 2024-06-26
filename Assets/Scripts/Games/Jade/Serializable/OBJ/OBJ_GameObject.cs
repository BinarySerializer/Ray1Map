using System;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class OBJ_GameObject : Jade_File {
		public override bool HasHeaderBFFile => true;
		public override string Export_Extension => "gao";
		public override string Export_FileBasename => Name.Substring(0,Name.Length-4);

		public Jade_FileType FileType { get; set; }
		public uint Version { get; set; }
		public uint EditorFlags { get; set; }
		public OBJ_GameObject_IdentityFlags FlagsIdentity { get; set; }
		public OBJ_GameObject_StatusFlags StatusFlags { get; set; }
		public byte AICustomBits { get; set; }
		public OBJ_GameObject_ControlFlags ControlFlags { get; set; }
		public byte Secto { get; set; }
		public byte MiscFlags { get; set; }
		public float VisibilityDistance { get; set; }
		public byte VisiCoeff { get; set; }
		public ushort UShort_12_Alignment { get; set; }
		public byte LOD_Vis { get; set; }
		public byte LOD_Dist { get; set; }
		public OBJ_GameObject_TypeFlags DesignFlags { get; set; }
		public OBJ_GameObject_FixFlags FixFlags { get; set; }
		public Jade_Matrix Matrix { get; set; }
		public Jade_Matrix Matrix2 { get; set; }
		
		public OBJ_BV_BoundingVolume BoundingVolume { get; set; }
		public OBJ_GameObject_Base Base { get; set; }
		public OBJ_GameObject_Extended Extended { get; set; }
		public Jade_Reference<COL_Instance> COL_Instance { get; set; }
		public Jade_Reference<COL_ColMap> COL_ColMap { get; set; }

		public uint PhoenixMontreal_V12 { get; set; }
		public uint PhoenixMontreal_V14 { get; set; }
		public uint NameLength { get; set; }
		public string Name { get; set; }
		public uint V23_Name2Length { get; set; }
		public string V23_Name2 { get; set; }
		public uint DummyVersion { get; set; }
		public uint CullingVisibility { get; set; }
		public uint ObjectModel { get; set; }
		public uint InvisibleObjectIndex { get; set; }
		public uint ForceLODIndex { get; set; }
		public uint User3 { get; set; }
		public long PhoenixMontreal_V12_Long { get; set; }

		// Editor only
		public Jade_FloatColor InstanceColor { get; set; }
		public uint AdditionalFlagsMask { get; set; }
		public Jade_Code PrefabFileMark { get; set; }
		public Jade_Key Prefab { get; set; }
		public Jade_Reference<OBJ_GameObject> PrefabObject { get; set; }
		public uint MontrealEditor0 { get; set; }
		public uint MontrealEditor1 { get; set; }
		public uint MontrealEditor2 { get; set; }
		public uint MontrealEditor3 { get; set; }

		public bool IsObjInitialized { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			FileType = s.SerializeObject<Jade_FileType>(FileType, name: nameof(FileType));
			if(FileType.Type != Jade_FileType.FileType.OBJ_GameObject)
				throw new BinarySerializableException(this, $"Parsing failed: File was not of type {Jade_FileType.FileType.OBJ_GameObject}");
			if(Loader?.WorldToLoadIn != null) Loader.WorldToLoadIn.SerializedGameObjects.Add(this);

			if (!Loader.IsBinaryData
				|| s.GetR1Settings().EngineFlags.HasFlag(EngineFlags.Jade_Xenon)
				|| s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
				Version = s.Serialize<uint>(Version, name: nameof(Version));
			} else {
				Version = 0;
			}
			EditorFlags = s.Serialize<uint>(EditorFlags, name: nameof(EditorFlags));
			FlagsIdentity = s.Serialize<OBJ_GameObject_IdentityFlags>(FlagsIdentity, name: nameof(FlagsIdentity));
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PhoenixRayman4)) {
				if (Version >= 12) PhoenixMontreal_V12 = s.Serialize<uint>(PhoenixMontreal_V12, name: nameof(PhoenixMontreal_V12));
				if (Version >= 14) PhoenixMontreal_V14 = s.Serialize<uint>(PhoenixMontreal_V14, name: nameof(PhoenixMontreal_V14));
			}
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal) && Version >= 2) {
				NameLength = s.Serialize<uint>(NameLength, name: nameof(NameLength));
				Name = s.SerializeString(Name, NameLength, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
				if (Version >= 23 && s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_TFS)) {
					V23_Name2Length = s.Serialize<uint>(V23_Name2Length, name: nameof(V23_Name2Length));
					V23_Name2 = s.SerializeString(V23_Name2, length: V23_Name2Length, name: nameof(V23_Name2));
				}
			}
			s.DoBits<uint>(b => {
				StatusFlags = b.SerializeBits<OBJ_GameObject_StatusFlags>(StatusFlags, 8, name: nameof(StatusFlags));
				AICustomBits = b.SerializeBits<byte>(AICustomBits, 8, name: nameof(AICustomBits));
				ControlFlags = b.SerializeBits<OBJ_GameObject_ControlFlags>(ControlFlags, 16, name: nameof(ControlFlags));
			});
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
				MiscFlags = s.Serialize<byte>(MiscFlags, name: nameof(MiscFlags));
				if (Version >= 24 && s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_TFS)) {
					VisibilityDistance = s.Serialize<float>(VisibilityDistance, name: nameof(VisibilityDistance));
				}
			} else {
				Secto = s.Serialize<byte>(Secto, name: nameof(Secto));
			}
			VisiCoeff = s.Serialize<byte>(VisiCoeff, name: nameof(VisiCoeff));
			if(!Loader.IsBinaryData || s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_TFS_PSP)) UShort_12_Alignment = s.Serialize<ushort>(UShort_12_Alignment, name: nameof(UShort_12_Alignment));
			LOD_Vis = s.Serialize<byte>(LOD_Vis, name: nameof(LOD_Vis));
			LOD_Dist = s.Serialize<byte>(LOD_Dist, name: nameof(LOD_Dist));
			DesignFlags = s.Serialize<OBJ_GameObject_TypeFlags>(DesignFlags, name: nameof(DesignFlags));
			FixFlags = s.Serialize<OBJ_GameObject_FixFlags>(FixFlags, name: nameof(FixFlags));
			Matrix = s.SerializeObject<Jade_Matrix>(Matrix, name: nameof(Matrix));
			if (!Loader.IsBinaryData && s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_CPP)) {
				Matrix2 = s.SerializeObject<Jade_Matrix>(Matrix2, name: nameof(Matrix2));
			}

			BoundingVolume = s.SerializeObject<OBJ_BV_BoundingVolume>(BoundingVolume, onPreSerialize: bv => bv.FlagsIdentity = FlagsIdentity, name: nameof(BoundingVolume));
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.BaseObject)) {
				Base = s.SerializeObject<OBJ_GameObject_Base>(Base, onPreSerialize: o => {
					o.GameObject = this;
				}, name: nameof(Base));
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.ExtendedObject)) {
				Extended = s.SerializeObject<OBJ_GameObject_Extended>(Extended, onPreSerialize: o => o.GameObject = this, name: nameof(Extended));
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.ZDM) || FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.ZDE)) {
				COL_Instance = s.SerializeObject<Jade_Reference<COL_Instance>>(COL_Instance, name: nameof(COL_Instance))?.Resolve();
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.ColMap)) {
				COL_ColMap = s.SerializeObject<Jade_Reference<COL_ColMap>>(COL_ColMap, name: nameof(COL_ColMap))?
					.Resolve(flags: LOA_Loader.ReferenceFlags.MustExist | LOA_Loader.ReferenceFlags.OnlyOneRef);
			}
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier) || Version < 2) {
				NameLength = s.Serialize<uint>(NameLength, name: nameof(NameLength));
				Name = s.SerializeString(Name, NameLength, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
			}
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRRPrototype)) {
				DummyVersion = s.Serialize<uint>(DummyVersion, name: nameof(DummyVersion));
				if (DummyVersion != 0)
					CullingVisibility = s.Serialize<uint>(CullingVisibility, name: nameof(CullingVisibility));
			} else {
				if (!Loader.IsBinaryData) DummyVersion = s.Serialize<uint>(DummyVersion, name: nameof(DummyVersion));
			}
			if (!Loader.IsBinaryData) {
				ObjectModel = s.Serialize<uint>(ObjectModel, name: nameof(ObjectModel));
				InvisibleObjectIndex = s.Serialize<uint>(InvisibleObjectIndex, name: nameof(InvisibleObjectIndex));
				ForceLODIndex = s.Serialize<uint>(ForceLODIndex, name: nameof(ForceLODIndex));
				User3 = s.Serialize<uint>(User3, name: nameof(User3));
				if ((s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_WW) && Version != 0)
					|| s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_TFS)) {
					MontrealEditor0 = s.Serialize<uint>(MontrealEditor0, name: nameof(MontrealEditor0));
					MontrealEditor1 = s.Serialize<uint>(MontrealEditor1, name: nameof(MontrealEditor1));
					MontrealEditor2 = s.Serialize<uint>(MontrealEditor2, name: nameof(MontrealEditor2));
					MontrealEditor3 = s.Serialize<uint>(MontrealEditor3, name: nameof(MontrealEditor3));
				}
				if (s.CurrentAbsoluteOffset < Offset.AbsoluteOffset + FileSize
					&& s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_BGE_Anniversary)
					&& Version != 0) {
					InstanceColor = s.SerializeObject<Jade_FloatColor>(InstanceColor, name: nameof(InstanceColor));
					if (Version >= 2) {
						AdditionalFlagsMask = s.Serialize<uint>(AdditionalFlagsMask, name: nameof(AdditionalFlagsMask));
					}
				}
			}
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PhoenixRayman4)) {
				if (Version > 12) PhoenixMontreal_V12_Long = s.Serialize<long>(PhoenixMontreal_V12_Long, name: nameof(PhoenixMontreal_V12_Long));
			}
			if (s.CurrentAbsoluteOffset < Offset.AbsoluteOffset + FileSize && !Loader.IsBinaryData) {
				PrefabFileMark = s.Serialize<Jade_Code>(PrefabFileMark, name: nameof(PrefabFileMark));
				switch (PrefabFileMark) {
					case Jade_Code.PrefabFileMark:
						Prefab = s.SerializeObject<Jade_Key>(Prefab, name: nameof(Prefab));
						break;
					case Jade_Code.PrefabFileMark1:
						Prefab = s.SerializeObject<Jade_Key>(Prefab, name: nameof(Prefab));
						PrefabObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(PrefabObject, name: nameof(PrefabObject));
						break;
				}
			}
		}


		public void UnoptimizeGeometry() {
			// Fill in regular geometry data based on optimized data
			if (Context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
				var gao = this;
				var gro = gao.Base?.Visual?.GeometricObject?.Value;
				if (gro != null) {
					if (gro.RenderObject.Type == GRO_Type.GEO_StaticLOD) {
						gro = (gro.RenderObject?.Value as GEO_StaticLOD).LODLevels[0]?.Value;
					}
					if (gro.RenderObject.Type == GRO_Type.GEO) {
						var geo = (GEO_GeometricObject)gro.RenderObject.Value;

						if (geo.OptimizedGeoObject_PS2 != null
							//&& geo.Context.GetR1Settings().Platform == Platform.PS2
							&& !geo.Montreal_FilledUnoptimizedData) {
							var ps2 = geo.OptimizedGeoObject_PS2;
							if (geo.Context.GetR1Settings().Platform == Platform.PS2) {
								geo.Montreal_FilledUnoptimizedData = true;
								gao?.Base?.Visual?.VisuPS2?.ExecuteChainPrograms(gao, geo, ps2);
							} else if (geo.Context.GetR1Settings().Platform == Platform.PSP) {
								geo.Montreal_FilledUnoptimizedData = true;
								gao?.Base?.Visual?.VisuPS2?.ExecuteGEPrograms(gao, geo, ps2);
							}
						} else if (geo.OptimizedGeoObject_PC != null
							&& !geo.Montreal_FilledUnoptimizedData) {
							geo.OptimizedGeoObject_PC.Unoptimize();
						}
						if (geo.Montreal_WasOptimized && geo.OptimizedGeoObject_PC != null) {
							gao?.Base?.Visual?.VisuPC?.Unoptimize(gao?.Base?.Visual, geo, geo?.OptimizedGeoObject_PC);
						}
					}
				}
			}
		}
	}
}
