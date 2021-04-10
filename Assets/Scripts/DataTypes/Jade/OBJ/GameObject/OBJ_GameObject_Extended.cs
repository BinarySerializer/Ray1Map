using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class OBJ_GameObject_Extended : BinarySerializable {
		public OBJ_GameObject_IdentityFlags FlagsIdentity { get; set; } // Set in OnPreSerialize

		public Jade_Reference<GRP_Grp> GRP { get; set; }
		public uint HasModifiers { get; set; }
		public float Float_BGE_08 { get; set; }
		public float Float_BGE_0C { get; set; }
		public uint UInt_Editor_08 { get; set; }
		public uint UInt_Editor_0C { get; set; }
		public uint UInt_Editor_10 { get; set; }
		public uint UInt_Editor_14 { get; set; }
		public uint UInt_Editor_18 { get; set; }
		public int Int_08 { get; set; }
		public byte[] Bytes_0C { get; set; }
		public uint UInt_10 { get; set; }
		public ushort UShort_Editor_12 { get; set; }
		public byte Byte_14 { get; set; }
		public byte Byte_15 { get; set; }
		public ushort UShort_16 { get; set; } // Flags

		public Jade_Reference<AI_Instance> AI_Instance { get; set; }
		public Jade_Reference<EVE_ListTracks> EVE_ListTracks { get; set; }
		public Jade_Reference<SND_UnknownBank> SND_UnknownBank { get; set; }
		public Jade_Reference<WAY_AllLinkLists> WAY_AllLinkLists { get; set; }
		public Jade_Reference<GEO_Object> Light { get; set; }
		public OBJ_GameObject_ExtendedUnknownData UnknownData { get; set; }
		public OBJ_GameObject_ExtendedXenonData XenonData { get; set; }
		public OBJ_GameObject_Modifier[] Modifiers { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			GRP = s.SerializeObject<Jade_Reference<GRP_Grp>>(GRP, name: nameof(GRP));
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.Flag23)) GRP?.Resolve();
			HasModifiers = s.Serialize<uint>(HasModifiers, name: nameof(HasModifiers));
			if (s.GetR1Settings().Game == Game.Jade_BGE) {
				Float_BGE_08 = s.Serialize<float>(Float_BGE_08, name: nameof(Float_BGE_08));
				Float_BGE_0C = s.Serialize<float>(Float_BGE_0C, name: nameof(Float_BGE_0C));
			}
			if (!Loader.IsBinaryData) {
				UInt_Editor_08 = s.Serialize<uint>(UInt_Editor_08, name: nameof(UInt_Editor_08));
				UInt_Editor_0C = s.Serialize<uint>(UInt_Editor_08, name: nameof(UInt_Editor_0C));
				UInt_Editor_10 = s.Serialize<uint>(UInt_Editor_08, name: nameof(UInt_Editor_10));
				UInt_Editor_14 = s.Serialize<uint>(UInt_Editor_08, name: nameof(UInt_Editor_14));
				UInt_Editor_18 = s.Serialize<uint>(UInt_Editor_08, name: nameof(UInt_Editor_18));
			}
			if (s.GetR1Settings().Game != Game.Jade_BGE) {
				Int_08 = s.Serialize<int>(Int_08, name: nameof(Int_08));
				int bytes_0C_count = (Int_08 != -1 && Int_08 != 0) ? 8 : 4;
				Bytes_0C = s.SerializeArray<byte>(Bytes_0C, bytes_0C_count, name: nameof(Bytes_0C));
			}
			if (Int_08 == -1 || s.GetR1Settings().Game == Game.Jade_BGE) {
				UInt_10 = s.Serialize<ushort>((ushort)UInt_10, name: nameof(UInt_10));
				if (!Loader.IsBinaryData)
					UShort_Editor_12 = s.Serialize<ushort>(UShort_Editor_12, name: nameof(UShort_Editor_12));
			} else {
				UInt_10 = s.Serialize<uint>(UInt_10, name: nameof(UInt_10));
			}
			Byte_14 = s.Serialize<byte>(Byte_14, name: nameof(Byte_14));
			Byte_15 = s.Serialize<byte>(Byte_15, name: nameof(Byte_15));
			UShort_16 = s.Serialize<ushort>(UShort_16, name: nameof(UShort_16));
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasAI)) {
				AI_Instance = s.SerializeObject<Jade_Reference<AI_Instance>>(AI_Instance, name: nameof(AI_Instance))?
					.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.Flag6);
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasEVE_ListTracks)) {
				EVE_ListTracks = s.SerializeObject<Jade_Reference<EVE_ListTracks>>(EVE_ListTracks, name: nameof(EVE_ListTracks))?.Resolve();
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasSND_UnknownBank)) {
				SND_UnknownBank = s.SerializeObject<Jade_Reference<SND_UnknownBank>>(SND_UnknownBank, name: nameof(SND_UnknownBank))?
					.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.KeepReferencesCount | LOA_Loader.ReferenceFlags.IsIrregularFileFormat);
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasWAY_AllLinkLists)) {
				WAY_AllLinkLists = s.SerializeObject<Jade_Reference<WAY_AllLinkLists>>(WAY_AllLinkLists, name: nameof(WAY_AllLinkLists))?
					.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.Flag6);
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasLight)) {
				Light = s.SerializeObject<Jade_Reference<GEO_Object>>(Light, name: nameof(Light))?.Resolve();
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasExtendedUnknownData)) {
				UnknownData = s.SerializeObject<OBJ_GameObject_ExtendedUnknownData>(UnknownData, name: nameof(UnknownData));
			}
			if (s.GetR1Settings().EngineVersion == EngineVersion.Jade_RRR_Xbox360 && Int_08 >= 2 && FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.Flag30)) {
				XenonData = s.SerializeObject<OBJ_GameObject_ExtendedXenonData>(XenonData, name: nameof(XenonData));
			}
			if (HasModifiers != 0) {
				Modifiers = s.SerializeObjectArrayUntil<OBJ_GameObject_Modifier>(Modifiers, m => m.Type == MDF_ModifierType.None, includeLastObj: true, name: nameof(Modifiers));
			}
		}
	}
}
