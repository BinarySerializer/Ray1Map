using System;
using BinarySerializer;

namespace R1Engine.Jade {
	public class OBJ_GameObject : Jade_File {
		public Jade_FileType FileType { get; set; }
		public uint Version { get; set; }
		public uint UInt_04 { get; set; }
		public OBJ_GameObject_IdentityFlags FlagsIdentity { get; set; }
		public ushort StatusFlags { get; set; }
		public ushort ControlFlags { get; set; }
		public byte Secto { get; set; }
		public byte MiscFlags { get; set; }
		public byte VisiCoeff { get; set; }
		public ushort UShort_12_Editor { get; set; }
		public byte LOD_Vis { get; set; }
		public byte LOD_Dist { get; set; }
		public byte DesignFlags { get; set; }
		public byte FixFlags { get; set; }
		public Jade_Matrix Matrix { get; set; }
		
		public OBJ_BV_BoundingVolume BoundingVolume { get; set; }
		public OBJ_GameObject_Base Base { get; set; }
		public OBJ_GameObject_Extended Extended { get; set; }
		public Jade_Reference<COL_Instance> COL_Instance { get; set; }
		public Jade_Reference<COL_ColMap> COL_ColMap { get; set; }

		public uint NameLength { get; set; }
		public string Name { get; set; }
		public uint UInt_AfterName_00 { get; set; }
		public uint UInt_AfterName_04 { get; set; }
		public uint UInt_AfterName_Editor_00 { get; set; }
		public uint UInt_AfterName_Editor_04 { get; set; }
		public uint UInt_AfterName_Editor_08 { get; set; }
		public uint UInt_AfterName_Editor_0C { get; set; }

		public bool IsInitialized { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			FileType = s.SerializeObject<Jade_FileType>(FileType, name: nameof(FileType));
			if(FileType.Type != Jade_FileType.FileType.OBJ_GameObject)
				throw new Exception($"Parsing failed: File at {Offset} was not of type {Jade_FileType.FileType.OBJ_GameObject}");
			if(Loader?.WorldToLoadIn != null) Loader.WorldToLoadIn.SerializedGameObjects.Add(this);

			if (!Loader.IsBinaryData
				|| s.GetR1Settings().Jade_Version == Jade_Version.Xenon
				|| s.GetR1Settings().Jade_Version >= Jade_Version.Montreal) {
				Version = s.Serialize<uint>(Version, name: nameof(Version));
			}
			UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
			FlagsIdentity = s.Serialize<OBJ_GameObject_IdentityFlags>(FlagsIdentity, name: nameof(FlagsIdentity));
			if (s.GetR1Settings().Jade_Version >= Jade_Version.Montreal && Version >= 2) {
				NameLength = s.Serialize<uint>(NameLength, name: nameof(NameLength));
				Name = s.SerializeString(Name, NameLength, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
			}
			s.SerializeBitValues<uint>(bitFunc => {
				StatusFlags = (ushort)bitFunc(StatusFlags, 16, name: nameof(StatusFlags));
				ControlFlags = (ushort)bitFunc(ControlFlags, 16, name: nameof(ControlFlags));
			});
			if (s.GetR1Settings().Jade_Version >= Jade_Version.Montreal) {
				MiscFlags = s.Serialize<byte>(MiscFlags, name: nameof(MiscFlags));
			} else {
				Secto = s.Serialize<byte>(Secto, name: nameof(Secto));
			}
			VisiCoeff = s.Serialize<byte>(VisiCoeff, name: nameof(VisiCoeff));
			if(!Loader.IsBinaryData) UShort_12_Editor = s.Serialize<ushort>(UShort_12_Editor, name: nameof(UShort_12_Editor));
			LOD_Vis = s.Serialize<byte>(LOD_Vis, name: nameof(LOD_Vis));
			LOD_Dist = s.Serialize<byte>(LOD_Dist, name: nameof(LOD_Dist));
			DesignFlags = s.Serialize<byte>(DesignFlags, name: nameof(DesignFlags));
			FixFlags = s.Serialize<byte>(FixFlags, name: nameof(FixFlags));
			Matrix = s.SerializeObject<Jade_Matrix>(Matrix, name: nameof(Matrix));

			BoundingVolume = s.SerializeObject<OBJ_BV_BoundingVolume>(BoundingVolume, onPreSerialize: bv => bv.FlagsIdentity = FlagsIdentity, name: nameof(BoundingVolume));
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasBase)) {
				Base = s.SerializeObject<OBJ_GameObject_Base>(Base, onPreSerialize: o => {
					o.FlagsIdentity = FlagsIdentity;
					o.Type = Version;
				}, name: nameof(Base));
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasExtended)) {
				Extended = s.SerializeObject<OBJ_GameObject_Extended>(Extended, onPreSerialize: o => o.GameObject = this, name: nameof(Extended));
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.Flag9) || FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.Flag10)) {
				COL_Instance = s.SerializeObject<Jade_Reference<COL_Instance>>(COL_Instance, name: nameof(COL_Instance))?.Resolve();
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasCOL_ColMap)) {
				COL_ColMap = s.SerializeObject<Jade_Reference<COL_ColMap>>(COL_ColMap, name: nameof(COL_ColMap))?
					.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.DontUseAlreadyLoadedCallback);
			}
			if (s.GetR1Settings().Jade_Version < Jade_Version.Montreal || Version < 2) {
				NameLength = s.Serialize<uint>(NameLength, name: nameof(NameLength));
				Name = s.SerializeString(Name, NameLength, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
			}
			if (s.GetR1Settings().Jade_Version < Jade_Version.Montreal && s.GetR1Settings().EngineVersion >= EngineVersion.Jade_RRR) {
				UInt_AfterName_00 = s.Serialize<uint>(UInt_AfterName_00, name: nameof(UInt_AfterName_00));
				if (UInt_AfterName_00 != 0)
					UInt_AfterName_04 = s.Serialize<uint>(UInt_AfterName_04, name: nameof(UInt_AfterName_04));
			} else {
				if (!Loader.IsBinaryData) UInt_AfterName_00 = s.Serialize<uint>(UInt_AfterName_00, name: nameof(UInt_AfterName_00));
			}
			if (!Loader.IsBinaryData) {
				UInt_AfterName_Editor_00 = s.Serialize<uint>(UInt_AfterName_Editor_00, name: nameof(UInt_AfterName_Editor_00));
				UInt_AfterName_Editor_04 = s.Serialize<uint>(UInt_AfterName_Editor_04, name: nameof(UInt_AfterName_Editor_04));
				UInt_AfterName_Editor_08 = s.Serialize<uint>(UInt_AfterName_Editor_08, name: nameof(UInt_AfterName_Editor_08));
				UInt_AfterName_Editor_0C = s.Serialize<uint>(UInt_AfterName_Editor_0C, name: nameof(UInt_AfterName_Editor_0C));
			}
		}
	}
}
