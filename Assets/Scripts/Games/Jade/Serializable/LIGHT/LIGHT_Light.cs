using System;
using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in LIGHT_p_CreateFromBuffer
	public class LIGHT_Light : GRO_GraphicRenderObject {
		public LightType Type { get; set; }
		public LightFlags Flags { get; set; }
		public Jade_Color Color { get; set; }
		public LIGHT_XenonData1 XenonData1 { get; set; }
		public uint UInt_Editor_00 { get; set; }
		public Jade_Color SpecularColor { get; set; }
		public float Near { get; set; }
		public float Far { get; set; }
		public float LittleAlpha { get; set; }
		public float BigAlpha { get; set; }
		public float Intensity { get; set; }
		public float OcclusionFactor { get; set; }
		public float HighIntensity { get; set; }
		public float LowIntensity { get; set; }
		public Jade_Reference<OBJ_GameObject> GameObject { get; set; }
		public LIGHT_XenonData2 XenonData2 { get; set; }

		public uint FogLinksCount { get; set; }
		public FogLink[] FogLinks { get; set; }
		public uint FogSubBVsCount { get; set; }
		public FogSubBV[] FogSubBVs { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
				s.DoBits<int>(b => {
					Type = (LightType)b.SerializeBits<int>((int)Type, 4, name: nameof(Type));
					Flags = (LightFlags)b.SerializeBits<int>((int)Flags, 28, name: nameof(Flags));
				});
			} else {
				s.DoBits<int>(b => {
					Type = (LightType)b.SerializeBits<int>((int)Type, 3, name: nameof(Type));
					// TODO: Flags are different for Montpellier
					Flags = (LightFlags)b.SerializeBits<int>((int)Flags, 29, name: nameof(Flags));
				});
			}
			s.Log(Type + " - " + Flags);
			Color = s.SerializeObject<Jade_Color>(Color, name: nameof(Color));
			if (s.GetR1Settings().EngineFlags.HasFlag(EngineFlags.Jade_Xenon) && BitHelpers.ExtractBits((int)Type, 3, 0) == 7) {
				XenonData1 = s.SerializeObject<LIGHT_XenonData1>(XenonData1, name: nameof(XenonData1));
			}
			if (!Loader.IsBinaryData && ObjectVersion >= 5)
				UInt_Editor_00 = s.Serialize<uint>(UInt_Editor_00, name: nameof(UInt_Editor_00));
			if (ObjectVersion >= 10) SpecularColor = s.SerializeObject<Jade_Color>(SpecularColor, name: nameof(SpecularColor));
			Near = s.Serialize<float>(Near, name: nameof(Near));
			Far = s.Serialize<float>(Far, name: nameof(Far));
			LittleAlpha = s.Serialize<float>(LittleAlpha, name: nameof(LittleAlpha));
			BigAlpha = s.Serialize<float>(BigAlpha, name: nameof(BigAlpha));
			if (ObjectVersion >= 1) Intensity = s.Serialize<float>(Intensity, name: nameof(Intensity));
			if (ObjectVersion >= 11) OcclusionFactor = s.Serialize<float>(OcclusionFactor, name: nameof(OcclusionFactor));
			if (ObjectVersion >= 13) {
				HighIntensity = s.Serialize<float>(HighIntensity, name: nameof(HighIntensity));
				LowIntensity = s.Serialize<float>(LowIntensity, name: nameof(LowIntensity));
			}
			if (ObjectVersion < 6 && (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier) || !Loader.IsBinaryData)) {
				GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject));
				if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) GameObject?.Resolve();
			}
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
				if (Type == LightType.Fog) {
					if (ObjectVersion >= 8) {
						FogLinksCount = s.Serialize<uint>(FogLinksCount, name: nameof(FogLinksCount));
						FogLinks = s.SerializeObjectArray<FogLink>(FogLinks, FogLinksCount, name: nameof(FogLinks));
					}
					if (ObjectVersion >= 9) {
						FogSubBVsCount = s.Serialize<uint>(FogSubBVsCount, name: nameof(FogSubBVsCount));
						FogSubBVs = s.SerializeObjectArray<FogSubBV>(FogSubBVs, FogSubBVsCount, name: nameof(FogSubBVs));
					}
				}
			}

			if (s.GetR1Settings().EngineFlags.HasFlag(EngineFlags.Jade_Xenon)) {
				if ((XenonData1 != null && BitHelpers.ExtractBits((int)XenonData1.LightFlags, 3, 0) == 5)
					|| (XenonData1 == null && s.GetR1Settings().EngineFlags.HasFlag(EngineFlags.Jade_Xenon) && BitHelpers.ExtractBits((int)Flags, 3, 0) == 5)) {
					XenonData2 = s.SerializeObject<LIGHT_XenonData2>(XenonData2, name: nameof(XenonData2));
				}
			}
		}

		public enum LightType : int {
			Omni = 0,
			Direct = 1,
			Spot = 2,
			Fog = 3,
			AddMaterial = 4,
			Ambient = 5,
			_6 = 6, // Types from 6 on were unused in the Montreal branch at least at the time of My Word Coach
			_7 = 7,
			_8 = 8,
			_9 = 9,
			_10 = 10,
			_11 = 11,
			_12 = 12,
			_13 = 13,
			_14 = 14,
			_15 = 15,
		}

		[Flags]
		public enum LightFlags : int {
			None = 0,
			Paint							= 0x00000001,
			Absorb							= 0x00000008,
			AffectStatic		 			= 0x00000010,
			AffectDynamic		 			= 0x00000020,
			DoesntCastLMShadows 			= 0x00000040,
			OnlyForLMShadows				= 0x00000080,
			UseDifferentColorForStatic		= 0x00000100,
			OmniIsDirectional				= 0x00000200,
			FogIsAbsolute					= 0x00000400,
			DoNotExcludeFromDirectionals	= 0x00000800,
			RLICastRay						= 0x00002000,
			CastShadows		   				= 0x00008000,
			OmniConst						= 0x00010000,
			Active							= 0x00020000,
			DoesNotAffectPerPixelLighting 	= 0x00040000,
			AffectPerPixelLightingOnly		= 0x00080000,
		}

		public class FogLink : BinarySerializable {
			public Jade_Reference<OBJ_GameObject> GameObject { get; set; }
			public Jade_Vector LinkPosition { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject));
				LinkPosition = s.SerializeObject<Jade_Vector>(LinkPosition, name: nameof(LinkPosition));
			}
		}

		public class FogSubBV : BinarySerializable {
			public Jade_Vector Min { get; set; }
			public Jade_Vector Max { get; set; }
			public Jade_Matrix InvFogMatrix { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Min = s.SerializeObject<Jade_Vector>(Min, name: nameof(Min));
				Max = s.SerializeObject<Jade_Vector>(Max, name: nameof(Max));
				InvFogMatrix = s.SerializeObject<Jade_Matrix>(InvFogMatrix, name: nameof(InvFogMatrix));
			}
		}
	}
}
