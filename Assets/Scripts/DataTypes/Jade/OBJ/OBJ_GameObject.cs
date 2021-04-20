using System;
using BinarySerializer;

namespace R1Engine.Jade {
	public class OBJ_GameObject : Jade_File {
		public Jade_FileType FileType { get; set; }
		public uint Type { get; set; }
		public uint UInt_04 { get; set; }
		public OBJ_GameObject_IdentityFlags FlagsIdentity { get; set; }
		public ushort FlagsStatus { get; set; }
		public ushort FlagsControl { get; set; }
		public byte Byte_10 { get; set; }
		public byte Byte_11 { get; set; }
		public ushort UShort_12_Editor { get; set; }
		public byte Byte_12 { get; set; }
		public byte Byte_13 { get; set; }
		public byte FlagsDesign { get; set; }
		public byte FlagsFix { get; set; }
		public Jade_Matrix Matrix { get; set; }
		
		public OBJ_BV_BoundingVolume BoundingVolume { get; set; }
		public OBJ_GameObject_Visual Visual { get; set; }
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

			if(!Loader.IsBinaryData || s.GetR1Settings().EngineVersion == EngineVersion.Jade_RRR_Xbox360) Type = s.Serialize<uint>(Type, name: nameof(Type));
			UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
			FlagsIdentity = s.Serialize<OBJ_GameObject_IdentityFlags>(FlagsIdentity, name: nameof(FlagsIdentity));
			s.SerializeBitValues<uint>(bitFunc => {
				FlagsStatus = (ushort)bitFunc(FlagsStatus, 16, name: nameof(FlagsStatus));
				FlagsControl = (ushort)bitFunc(FlagsControl, 16, name: nameof(FlagsControl));
			});
			Byte_10 = s.Serialize<byte>(Byte_10, name: nameof(Byte_10));
			Byte_11 = s.Serialize<byte>(Byte_11, name: nameof(Byte_11));
			if(!Loader.IsBinaryData) UShort_12_Editor = s.Serialize<ushort>(UShort_12_Editor, name: nameof(UShort_12_Editor));
			Byte_12 = s.Serialize<byte>(Byte_12, name: nameof(Byte_12));
			Byte_13 = s.Serialize<byte>(Byte_13, name: nameof(Byte_13));
			FlagsDesign = s.Serialize<byte>(FlagsDesign, name: nameof(FlagsDesign));
			FlagsFix = s.Serialize<byte>(FlagsFix, name: nameof(FlagsFix));
			Matrix = s.SerializeObject<Jade_Matrix>(Matrix, name: nameof(Matrix));

			BoundingVolume = s.SerializeObject<OBJ_BV_BoundingVolume>(BoundingVolume, onPreSerialize: bv => bv.FlagsIdentity = FlagsIdentity, name: nameof(BoundingVolume));
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasVisual)) {
				Visual = s.SerializeObject<OBJ_GameObject_Visual>(Visual, onPreSerialize: o => {
					o.FlagsIdentity = FlagsIdentity;
					o.Type = Type;
				}, name: nameof(Visual));
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasExtended)) {
				Extended = s.SerializeObject<OBJ_GameObject_Extended>(Extended, onPreSerialize: o => o.FlagsIdentity = FlagsIdentity, name: nameof(Extended));
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.Flag9) || FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.Flag10)) {
				COL_Instance = s.SerializeObject<Jade_Reference<COL_Instance>>(COL_Instance, name: nameof(COL_Instance))?.Resolve();
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasCOL_ColMap)) {
				COL_ColMap = s.SerializeObject<Jade_Reference<COL_ColMap>>(COL_ColMap, name: nameof(COL_ColMap))?
					.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.DontUseAlreadyLoadedCallback);
			}
			NameLength = s.Serialize<uint>(NameLength, name: nameof(NameLength));
			Name = s.SerializeString(Name, NameLength, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
			if (s.GetR1Settings().Game == Game.Jade_RRR) {
				UInt_AfterName_00 = s.Serialize<uint>(UInt_AfterName_00, name: nameof(UInt_AfterName_00));
				if (UInt_AfterName_00 != 0)
					UInt_AfterName_04 = s.Serialize<uint>(UInt_AfterName_04, name: nameof(UInt_AfterName_04));
				if (!Loader.IsBinaryData) {
					UInt_AfterName_Editor_00 = s.Serialize<uint>(UInt_AfterName_Editor_00, name: nameof(UInt_AfterName_Editor_00));
					UInt_AfterName_Editor_04 = s.Serialize<uint>(UInt_AfterName_Editor_04, name: nameof(UInt_AfterName_Editor_04));
					UInt_AfterName_Editor_08 = s.Serialize<uint>(UInt_AfterName_Editor_08, name: nameof(UInt_AfterName_Editor_08));
					UInt_AfterName_Editor_0C = s.Serialize<uint>(UInt_AfterName_Editor_0C, name: nameof(UInt_AfterName_Editor_0C));
				}
			}
		}
	}
}
