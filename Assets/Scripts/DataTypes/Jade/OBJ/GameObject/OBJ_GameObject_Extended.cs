using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Jade {
	public class OBJ_GameObject_Extended : R1Serializable {
		public OBJ_GameObject_IdentityFlags FlagsIdentity { get; set; } // Set in OnPreSerialize

		public Jade_Reference<GRP_Grp> GRP { get; set; }
		public uint HasModifiers { get; set; }
		public uint UInt_Editor_08 { get; set; }
		public uint UInt_Editor_0C { get; set; }
		public uint UInt_Editor_10 { get; set; }
		public uint UInt_Editor_14 { get; set; }
		public uint UInt_Editor_18 { get; set; }
		public int Int_08 { get; set; }
		public byte[] Bytes_0C { get; set; }
		public uint UInt_10 { get; set; }
		public ushort UShort_12 { get; set; }
		public byte Byte_14 { get; set; }
		public byte Byte_15 { get; set; }
		public ushort UShort_16 { get; set; } // Flags

		public Jade_Reference<AI_Instance> AI_Instance { get; set; }
		public Jade_Reference<EVE_ListTracks> EVE_ListTracks { get; set; }
		public Jade_Reference<SND_UnknownBank> SND_UnknownBank { get; set; }
		public Jade_Reference<WAY_AllLinkLists> WAY_AllLinkLists { get; set; }
		public Jade_Reference<GEO_Object> GEO_Object { get; set; }
		public OBJ_GameObject_Modifier[] Modifiers { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			GRP = s.SerializeObject<Jade_Reference<GRP_Grp>>(GRP, name: nameof(GRP));
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.Flag23)) GRP?.Resolve();
			HasModifiers = s.Serialize<uint>(HasModifiers, name: nameof(HasModifiers));
			if (!Loader.IsBinaryData) {
				UInt_Editor_08 = s.Serialize<uint>(UInt_Editor_08, name: nameof(UInt_Editor_08));
				UInt_Editor_0C = s.Serialize<uint>(UInt_Editor_08, name: nameof(UInt_Editor_0C));
				UInt_Editor_10 = s.Serialize<uint>(UInt_Editor_08, name: nameof(UInt_Editor_10));
				UInt_Editor_14 = s.Serialize<uint>(UInt_Editor_08, name: nameof(UInt_Editor_14));
				UInt_Editor_18 = s.Serialize<uint>(UInt_Editor_08, name: nameof(UInt_Editor_18));
			}
			Int_08 = s.Serialize<int>(Int_08, name: nameof(Int_08));
			int bytes_0C_count = (Int_08 != -1 && Int_08 != 0) ? 8 : 4;
			Bytes_0C = s.SerializeArray<byte>(Bytes_0C, bytes_0C_count, name: nameof(Bytes_0C));
			if (Int_08 == -1) {
				UInt_10 = s.Serialize<ushort>((ushort)UInt_10, name: nameof(UInt_10));
				UShort_12 = s.Serialize<ushort>(UShort_12, name: nameof(UShort_12));
			} else {
				UInt_10 = s.Serialize<uint>(UInt_10, name: nameof(UInt_10));
			}
			Byte_14 = s.Serialize<byte>(Byte_14, name: nameof(Byte_14));
			Byte_15 = s.Serialize<byte>(Byte_15, name: nameof(Byte_15));
			UShort_16 = s.Serialize<ushort>(UShort_16, name: nameof(UShort_16));
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasAI)) {
				AI_Instance = s.SerializeObject<Jade_Reference<AI_Instance>>(AI_Instance, name: nameof(AI_Instance))?.Resolve();
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasEVE_ListTracks)) {
				EVE_ListTracks = s.SerializeObject<Jade_Reference<EVE_ListTracks>>(EVE_ListTracks, name: nameof(EVE_ListTracks))?.Resolve();
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasSND_UnknownBank)) {
				SND_UnknownBank = s.SerializeObject<Jade_Reference<SND_UnknownBank>>(SND_UnknownBank, name: nameof(SND_UnknownBank))?.Resolve();
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasWAY_AllLinkLists)) {
				WAY_AllLinkLists = s.SerializeObject<Jade_Reference<WAY_AllLinkLists>>(WAY_AllLinkLists, name: nameof(WAY_AllLinkLists))?.Resolve();
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasGEO_Object)) {
				GEO_Object = s.SerializeObject<Jade_Reference<GEO_Object>>(GEO_Object, name: nameof(GEO_Object))?.Resolve();
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.Flag6)) {
				throw new NotImplementedException($"TODO: Implement {GetType()}: Flag6");
			}
			if (HasModifiers != 0) {
				Modifiers = s.SerializeObjectArrayUntil<OBJ_GameObject_Modifier>(Modifiers, m => m.Type == MDF_ModifierType.None, includeLastObj: true, name: nameof(Modifiers));
			}
		}
	}
}
