using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class OBJ_GameObject_Extended : BinarySerializable {
		public OBJ_GameObject GameObject { get; set; } // Set in OnPreSerialize
		public OBJ_GameObject_IdentityFlags FlagsIdentity => GameObject.FlagsIdentity;

		public Jade_Reference<GRP_Grp> Group { get; set; }
		public uint HasModifiers { get; set; }
		public float LODai { get; set; }
		public float DistCut { get; set; }
		public uint UInt_Editor_08 { get; set; }
		public uint UInt_Editor_0C { get; set; }
		public uint UInt_Editor_10 { get; set; }
		public uint UInt_Editor_14 { get; set; }
		public uint UInt_Editor_18 { get; set; }
		public int Int_08 { get; set; }
		public byte[] Sectos { get; set; }
		public uint Capacities { get; set; }
		public ushort UShort_Editor_12 { get; set; }
		public byte AiPrio { get; set; }
		public byte Blank { get; set; }
		public ushort ExtraFlags { get; set; } // Flags

		public Jade_Reference<AI_Instance> AI { get; set; }
		public Jade_Reference<EVE_ListTracks> Events { get; set; }
		public Jade_Reference<SND_UnknownBank> Sound { get; set; }
		public Jade_Reference<WAY_AllLinkLists> Links { get; set; }
		public Jade_Reference<GEO_Object> Light { get; set; }
		public OBJ_GameObject_DesignStruct Design { get; set; }
		public OBJ_GameObject_ExtendedXenonData XenonData { get; set; }
		public OBJ_GameObject_Modifier[] Modifiers { get; set; }
		public OBJ_GameObject_Extended_CurrentStaticWind CurrentStaticWind { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			Group = s.SerializeObject<Jade_Reference<GRP_Grp>>(Group, name: nameof(Group));
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.Flag23)) Group?.Resolve();
			HasModifiers = s.Serialize<uint>(HasModifiers, name: nameof(HasModifiers));
			if (s.GetR1Settings().EngineVersion < EngineVersion.Jade_KingKong ||
				s.GetR1Settings().Jade_Version >= Jade_Version.Montreal) {
				LODai = s.Serialize<float>(LODai, name: nameof(LODai));
				DistCut = s.Serialize<float>(DistCut, name: nameof(DistCut));
			}
			if (!Loader.IsBinaryData
				|| (s.GetR1Settings().Jade_Version >= Jade_Version.Montreal && s.GetR1Settings().EngineVersion >= EngineVersion.Jade_RRRTVParty)) {
				UInt_Editor_08 = s.Serialize<uint>(UInt_Editor_08, name: nameof(UInt_Editor_08));
			}
			if (!Loader.IsBinaryData) {
				UInt_Editor_0C = s.Serialize<uint>(UInt_Editor_0C, name: nameof(UInt_Editor_0C));
				UInt_Editor_10 = s.Serialize<uint>(UInt_Editor_10, name: nameof(UInt_Editor_10));
				UInt_Editor_14 = s.Serialize<uint>(UInt_Editor_14, name: nameof(UInt_Editor_14));
				UInt_Editor_18 = s.Serialize<uint>(UInt_Editor_18, name: nameof(UInt_Editor_18));
			}
			if (s.GetR1Settings().EngineVersion >= EngineVersion.Jade_KingKong
				&& s.GetR1Settings().Jade_Version < Jade_Version.Montreal) {
				if (s.GetR1Settings().EngineVersion >= EngineVersion.Jade_RRR) {
					Int_08 = s.Serialize<int>(Int_08, name: nameof(Int_08));
				}
				int Sectos_count = (Int_08 != -1 && Int_08 != 0) ? 8 : 4;
				Sectos = s.SerializeArray<byte>(Sectos, Sectos_count, name: nameof(Sectos));
			}
			if (Int_08 == -1
				|| s.GetR1Settings().EngineVersion < EngineVersion.Jade_RRR
				|| s.GetR1Settings().Jade_Version >= Jade_Version.Montreal) {
				Capacities = s.Serialize<ushort>((ushort)Capacities, name: nameof(Capacities));
				if (!Loader.IsBinaryData)
					UShort_Editor_12 = s.Serialize<ushort>(UShort_Editor_12, name: nameof(UShort_Editor_12));
			} else {
				Capacities = s.Serialize<uint>(Capacities, name: nameof(Capacities));
			}
			AiPrio = s.Serialize<byte>(AiPrio, name: nameof(AiPrio));
			Blank = s.Serialize<byte>(Blank, name: nameof(Blank));
			ExtraFlags = s.Serialize<ushort>(ExtraFlags, name: nameof(ExtraFlags));
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasAI)) {
				AI = s.SerializeObject<Jade_Reference<AI_Instance>>(AI, name: nameof(AI))?
					.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.Flag6);
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasEvents)) {
				Events = s.SerializeObject<Jade_Reference<EVE_ListTracks>>(Events, name: nameof(Events))?.Resolve();
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasSound)) {
				Sound = s.SerializeObject<Jade_Reference<SND_UnknownBank>>(Sound, name: nameof(Sound))?
					.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.KeepReferencesCount | LOA_Loader.ReferenceFlags.IsIrregularFileFormat);
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasLinks)) {
				Links = s.SerializeObject<Jade_Reference<WAY_AllLinkLists>>(Links, name: nameof(Links))?
					.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.Flag6);
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasLight)) {
				Light = s.SerializeObject<Jade_Reference<GEO_Object>>(Light, name: nameof(Light))?.Resolve();
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasDesign)) {
				Design = s.SerializeObject<OBJ_GameObject_DesignStruct>(Design, name: nameof(Design));
			}
			if (s.GetR1Settings().Jade_Version == Jade_Version.Xenon && Int_08 >= 2 && FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.Flag30)) {
				XenonData = s.SerializeObject<OBJ_GameObject_ExtendedXenonData>(XenonData, name: nameof(XenonData));
			}
			if (HasModifiers != 0) {
				Modifiers = s.SerializeObjectArrayUntil<OBJ_GameObject_Modifier>(Modifiers, m => m.Type == MDF_ModifierType.None, name: nameof(Modifiers));
			}
			if (s.GetR1Settings().Jade_Version >= Jade_Version.Montreal && (GameObject.MiscFlags & 0x10) != 0) {
				CurrentStaticWind = s.SerializeObject<OBJ_GameObject_Extended_CurrentStaticWind>(CurrentStaticWind, name: nameof(CurrentStaticWind));
			}
		}
	}
}
