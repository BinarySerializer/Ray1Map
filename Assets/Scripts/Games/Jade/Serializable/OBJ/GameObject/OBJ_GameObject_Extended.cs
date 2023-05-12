using BinarySerializer;
using System;
using System.Diagnostics;

namespace Ray1Map.Jade {
	public class OBJ_GameObject_Extended : BinarySerializable {
		public OBJ_GameObject GameObject { get; set; } // Set in OnPreSerialize
		public OBJ_GameObject_IdentityFlags FlagsIdentity => GameObject.FlagsIdentity;

		public Jade_Reference<GRP_Grp> Group { get; set; }
		public uint HasModifiers { get; set; }
		public uint LightListFlags { get; set; }
		public float LODai { get; set; }
		public float DistCut { get; set; }
		public uint UInt_Editor_0C { get; set; }
		public uint UInt_Editor_10 { get; set; }
		public uint UInt_Editor_14 { get; set; }
		public uint UInt_Editor_18 { get; set; }
		public int VersionNumber { get; set; }
		public byte[] Sectos { get; set; }
		public uint Capacities { get; set; }
		public ushort UShort_Editor_12 { get; set; }
		public byte AiPrio { get; set; }
		public byte Blank { get; set; }
		public OBJ_GameObject_ExtraFlags ExtraFlags { get; set; } // Flags
		public uint ExtendedFlags { get; set; }

		public Jade_Reference<AI_Instance> AI { get; set; }
		public Jade_Reference<AI_System> AISystem { get; set; }
		public Jade_Reference<EVE_ListTracks> Events { get; set; }
		public Jade_Reference<SND_UnknownBank> Sound { get; set; }
		public DARE_SoundParam SoundDARE { get; set; }
		public Jade_Reference<WAY_AllLinkLists> Links { get; set; }
		public Jade_Reference<GEO_Object> Light { get; set; }
		public Jade_Reference<OBJ_GameObject>[] LightList { get; set; }
		public OBJ_GameObject_DesignStruct Design { get; set; }
		public OBJ_GameObject_ExtendedXenonData XenonData { get; set; }
		public OBJ_GameObject_Modifier[] Modifiers { get; set; }
		public OBJ_GameObject_Extended_CurrentStaticWind CurrentStaticWind { get; set; }
		public uint JadeCPP_V25_UInt { get; set; }
		public uint NameHash { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			Group = s.SerializeObject<Jade_Reference<GRP_Grp>>(Group, name: nameof(Group));
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.Group)) Group?.Resolve();
			HasModifiers = s.Serialize<uint>(HasModifiers, name: nameof(HasModifiers));
			if (!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong)
				&& (!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_TFS) || GameObject.Version < 0x19)
				&& (!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Fox) || GameObject.Version < 0x19)) {
				LODai = s.Serialize<float>(LODai, name: nameof(LODai));
				DistCut = s.Serialize<float>(DistCut, name: nameof(DistCut));
			}
			if (!Loader.IsBinaryData
				|| (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_T2T_20051002))) {
				LightListFlags = s.Serialize<uint>(LightListFlags, name: nameof(LightListFlags));
			}
			if (!Loader.IsBinaryData && (!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_TFS) || GameObject.Version < 0x19)) {
				UInt_Editor_0C = s.Serialize<uint>(UInt_Editor_0C, name: nameof(UInt_Editor_0C));
				UInt_Editor_10 = s.Serialize<uint>(UInt_Editor_10, name: nameof(UInt_Editor_10));
				UInt_Editor_14 = s.Serialize<uint>(UInt_Editor_14, name: nameof(UInt_Editor_14));
				UInt_Editor_18 = s.Serialize<uint>(UInt_Editor_18, name: nameof(UInt_Editor_18));
			}
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong)) {
				if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRRPrototype)) {
					VersionNumber = s.Serialize<int>(VersionNumber, name: nameof(VersionNumber));
				}
				int Sectos_count = 4;
				if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRR)) {
					if (VersionNumber != -1 && VersionNumber != 0) Sectos_count = 8;
				}
				if (Sectos != null && Sectos_count != Sectos.Length) {
					var sectos = Sectos;
					Array.Resize(ref sectos, Sectos_count);
					Sectos = sectos;
				}
				Sectos = s.SerializeArray<byte>(Sectos, Sectos_count, name: nameof(Sectos));
			}
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRRPrototype) && VersionNumber != -1) {
				Capacities = s.Serialize<uint>(Capacities, name: nameof(Capacities));
			} else {
				Capacities = s.Serialize<ushort>((ushort)Capacities, name: nameof(Capacities));
				if (!Loader.IsBinaryData)
					UShort_Editor_12 = s.Serialize<ushort>(UShort_Editor_12, name: nameof(UShort_Editor_12));
			}
			AiPrio = s.Serialize<byte>(AiPrio, name: nameof(AiPrio));
			Blank = s.Serialize<byte>(Blank, name: nameof(Blank));
			ExtraFlags = s.Serialize<OBJ_GameObject_ExtraFlags>(ExtraFlags, name: nameof(ExtraFlags));
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_CPP) && GameObject.Version >= 14) {
				ExtendedFlags = s.Serialize<uint>(ExtendedFlags, name: nameof(ExtendedFlags));
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.AI)) {
				AI = s.SerializeObject<Jade_Reference<AI_Instance>>(AI, name: nameof(AI))?
					.Resolve(flags: LOA_Loader.ReferenceFlags.MustExist | LOA_Loader.ReferenceFlags.Flag6);
			}
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_CPP) && (ExtendedFlags & 2) != 0 && GameObject.Version >= 14) {
				// If ExtendedFlags & 2 != 0 => load AI_System here, but only if it isn't loaded yet!
				AISystem = s.SerializeObject<Jade_Reference<AI_System>>(AISystem, name: nameof(AISystem))?.Resolve();
			}

			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.Events)) {
				Events = s.SerializeObject<Jade_Reference<EVE_ListTracks>>(Events, name: nameof(Events))?.Resolve();
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.Sound) && !s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_CPP)) {
				Sound = s.SerializeObject<Jade_Reference<SND_UnknownBank>>(Sound, name: nameof(Sound));
				if(s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier))
					Sound?.Resolve(flags: LOA_Loader.ReferenceFlags.MustExist | LOA_Loader.ReferenceFlags.HasUserCounter | LOA_Loader.ReferenceFlags.IsIrregularFileFormat);
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.Sound_DARE) && s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
				SoundDARE = s.SerializeObject<DARE_SoundParam>(SoundDARE, name: nameof(SoundDARE));
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.Links)) {
				Links = s.SerializeObject<Jade_Reference<WAY_AllLinkLists>>(Links, name: nameof(Links))?
					.Resolve(flags: LOA_Loader.ReferenceFlags.MustExist | LOA_Loader.ReferenceFlags.Flag6);
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.Lights)) {
				Light = s.SerializeObject<Jade_Reference<GEO_Object>>(Light, name: nameof(Light))?.Resolve();
			}
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_T2T_20051002) &&
				GameObject.Version >= 12 && (LightListFlags & 0x1) != 0) {
				LightList = s.SerializeObjectArray<Jade_Reference<OBJ_GameObject>>(LightList, 4, name: nameof(LightList))?.Resolve();
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.DesignStruct)) {
				Design = s.SerializeObject<OBJ_GameObject_DesignStruct>(Design, name: nameof(Design));
			}
			if (s.GetR1Settings().EngineFlags.HasFlag(EngineFlags.Jade_Xenon) && VersionNumber >= 2 && FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.Flag30)) {
				XenonData = s.SerializeObject<OBJ_GameObject_ExtendedXenonData>(XenonData, name: nameof(XenonData));
			}
			if (HasModifiers != 0) {
				Modifiers = s.SerializeObjectArrayUntil<OBJ_GameObject_Modifier>(Modifiers, m => m.IsNull, name: nameof(Modifiers));
			}
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal) && (GameObject.MiscFlags & 0x10) != 0) {
				CurrentStaticWind = s.SerializeObject<OBJ_GameObject_Extended_CurrentStaticWind>(CurrentStaticWind, name: nameof(CurrentStaticWind));
			}
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_CPP)) {
				if(s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_TFS) && GameObject.Version >= 0x19)
					JadeCPP_V25_UInt = s.Serialize<uint>(JadeCPP_V25_UInt, name: nameof(JadeCPP_V25_UInt));
				NameHash = s.Serialize<uint>(NameHash, name: nameof(NameHash));
			}
		}
	}
}
